using RecipeService.DomainTypes;
using RecipeService.Interfaces;
using System.Linq;

namespace RecipeService.DataSources
{
    /// <summary>
    /// This implementation gets recipe data from text files in a folder. One recipe per file. The file has a required
    /// format for parsing. 
    /// </summary>
    public class FileData : IDataSource
    {
        static char[] delims = { ' ' };
        string recipeFolder;
        Dictionary<RecipeID,Recipe> _recipes;
        ILogger<FileData> _logger;
        /// <summary>
        /// ctor for testing
        /// </summary>
        /// <param name="dFolder"></param>
        public FileData(string dFolder) 
        {
            recipeFolder = dFolder;
            _recipes = LoadRecipeData();
        }
        /// <summary>
        /// ctor for app usage via Dependency Injection
        /// </summary>
        /// <param name="config"></param>
        public FileData(IConfiguration config, ILogger<FileData> logger)
        {
            try
            {
                _logger = logger;
                recipeFolder = config.GetValue<string>("RecipeFolder");
                _logger.LogInformation("FileData:IDataSource created, RecipeFolder={0}", recipeFolder);
                _recipes = LoadRecipeData();
            }
            catch (Exception ex) 
            {
                if (_logger!=null)
                    _logger.LogError(ex, "FileData:IDataSource error, RecipeFolder={0}", recipeFolder);
                throw;
            }

        }

        #region interface impl
        public Optional<Recipe> GetRecipe(RecipeID id)
        {
            Optional<Recipe> result;

            if (_recipes.ContainsKey(id))
                result = Optional<Recipe>.of(_recipes[id]);
            else
                result = Optional<Recipe>.empty();

            return result;
        }

        public List<RecipeLink> GetRecipes()
        {
            List<RecipeLink> links = new List<RecipeLink>();
            var it = _recipes.Keys.GetEnumerator();
            while (it.MoveNext())
            {
                var key = it.Current;
                var localRecipe = _recipes[key];
                var u = new Uri(String.Format("recipe//{0}", key.Val), UriKind.Relative);
                var rl = new RecipeLink(localRecipe.namen, u);
                links.Add(rl);
            }
            return links;
        }

        public List<RecipeLink> GetRecipes(string ingredientName)
        {
            List<RecipeLink> links = new List<RecipeLink>();
            var it = _recipes.Keys.GetEnumerator();
            while (it.MoveNext())
            {
                var key = it.Current;
                var localRecipe = _recipes[key];

                var result = localRecipe.ingredients.Any(ing =>
                {
                    return ing.Val.IndexOf(ingredientName, StringComparison.OrdinalIgnoreCase) >= 0 ? true : false;
                });
                if (result)
                {
                    var u = new Uri(String.Format("recipe//{0}", key.Val), UriKind.Relative);
                    var rl = new RecipeLink(localRecipe.namen, u);
                    links.Add(rl);
                }
            }
            return links;
        }
        #endregion
        #region implementation details
        internal Dictionary<RecipeID, Recipe> LoadRecipeData()
        {
            var files = Directory.GetFiles(recipeFolder);

            long baseIndex = 1000L;
            Dictionary < RecipeID, Recipe > dict = new Dictionary<RecipeID, Recipe> ();
            foreach (string file in files)
            {
                ReadFileIntoString(file)
                .map<Recipe>(fileContents =>
                {
                    return ParseFileToRecipe(fileContents);

                }).ifPresent(recipeItem =>
                {
                    dict.Add(new RecipeID(baseIndex++), recipeItem);
                });              
            }
            return dict;
        }
        internal Optional<string> ReadFileIntoString(string fileName)
        {
            try
            {
                var input = File.OpenRead(fileName);
                string fileContents = null;
                using (StreamReader reader = new StreamReader(input))
                {
                    fileContents = reader.ReadToEnd();
                    reader.Close();
                }
                return Optional<string>.of(fileContents);
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);  
            }
            return Optional<string>.empty();
        }

        internal string getRecipeName(StringReader sr)
        {
            while (sr.Peek() != -1)
            {
                string singleLine = sr.ReadLine();
                if (string.IsNullOrEmpty(singleLine))
                    continue;
                if (singleLine.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                   return sr.ReadLine();
                }
            }
            return String.Empty;
        }
        internal void advanceToIngredients(StringReader sr)
        {
            while (sr.Peek() != -1)
            {
                string singleLine = sr.ReadLine();
                if (string.IsNullOrEmpty(singleLine))
                    continue;
                if (singleLine.Equals("ingredients", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
        }
        internal List<string> getIngredients(StringReader sr)
        {
            List<string> ingredientNames = new List<string>();
            while (sr.Peek() != -1)
            {
                string? singleLine = sr.ReadLine();
                if (string.IsNullOrEmpty(singleLine))
                    continue;
                if (singleLine.Equals("directions", StringComparison.OrdinalIgnoreCase) || singleLine.Equals("instructions", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                ingredientNames.Add(singleLine);
            }
            return ingredientNames;
        }
        internal List<string> getInstructions(StringReader sr)
        {
            List<string> instructions = new List<string>();
            while (sr.Peek() != -1)
            {
                string? singleLine = sr.ReadLine();
                if (string.IsNullOrEmpty(singleLine))
                    continue;
                instructions.Add(singleLine);
            }
            return instructions;
        }
        internal Recipe ParseFileToRecipe(string fileContents)
        {
            List<string> ingredientNames = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(fileContents))
                    throw new NullReferenceException();

                string recipeName = string.Empty;
                List<string> ingredients = new List<string>();
                List<string> instructions = new List<string>();
                StringReader sr = new StringReader(fileContents);

                recipeName = getRecipeName(sr);

                advanceToIngredients(sr);
              
                ingredients = getIngredients(sr);

                instructions = getInstructions(sr);

                var rIngredients = ingredients.Select(s => new Ingredient(s)).ToList();
                var rInstructions = instructions.Select(s => new Instruction(s)).ToList();
                var r = new Recipe(new RecipeName(recipeName), rIngredients, rInstructions);
                return r;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }           
         }
        internal static string removePunctuation(string s)
        {
            try
            {
                var ca = s.ToLower().ToCharArray();
                var cad = Array.FindAll(ca, (c) => Char.IsLetter(c) || c == '-' || c == '_');
                if (cad.Length == 0)
                    return string.Empty;

                if (!Array.Exists(cad, c => Char.IsLetter(c)))
                    return string.Empty;

                int i = 0;
                while (cad[i] == '-')
                    i++;
                int l = cad.Length - 1;
                while (l > 0 && cad[l] == '-')
                    l--;
                if (i != 0 || l != cad.Length - 1)
                {
                    var sli = new ArraySegment<char>(cad);
                    sli.Slice(i, l - i).ToArray();
                    return String.Join(null, sli.Slice(i, l - i + 1).ToArray());
                }
                return String.Join(null, cad);
            }
            catch
            {
            }
            return null;
        }
        //internal static bool excludeWord(string s)
        //{
        //    if (String.IsNullOrEmpty(s) || s.Length < 4 || s.Length > 20)
        //        return true;
        //    if (s.IndexOf("http") > -1)
        //        return true;


        //    return false;
        //}
        //internal static Dictionary<string, int> parseLine(string singleLine)
        //{
        //    Dictionary<string, int> db = new Dictionary<string, int>();
        //    if (string.IsNullOrEmpty(singleLine))
        //        return db;
        //    var words = singleLine.Split(delims);
        //    for (int i = 0; i < words.Length; i++)
        //    {
        //        var key = removePunctuation(words[i]);
        //        if (String.IsNullOrEmpty(key))
        //            continue;

        //        if (db.ContainsKey(key))
        //        {
        //            var c = db[key];
        //            db[key] = ++c;
        //        }
        //        else
        //        {
        //            db.Add(key, 1);
        //        }
        //    }
        //    return db;
        //}
        //internal static string extractName(string fileName)
        //{
        //    return Path.GetFileName(fileName);
        //}
        #endregion

       
    }
}
