using RecipeService.DomainTypes;
using RecipeService.Interfaces;

namespace RecipeService.DataSources
{
    /// <summary>
    /// If using a database to store the recipes, implement this class.
    /// </summary>
    public class RecipeDatabase : IDataSource
    {
        public RecipeLink CreateRecipe(Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public Optional<Recipe> GetRecipe(RecipeID id)
        {
            throw new NotImplementedException();
        }

        public List<RecipeLink> GetRecipes()
        {
            throw new NotImplementedException();
        }

        public List<RecipeLink> GetRecipes(string ingredientName)
        {
            throw new NotImplementedException();
        }
    }
}
