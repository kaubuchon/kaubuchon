using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using RecipeService.DataSources;
using RecipeService.DomainTypes;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;


namespace RecipeService.Tests
{
    /// <summary>
    /// This test is for the FileData implemenation of interface IDataSource. These tests while useful, are fragile
    /// since refactoring of FileData may break them. 
    /// </summary>
    public class FileDataTest
    {
        const string recipeFolder = "recipes";
        Mock<ILogger<FileData>> loggerMock;
        static readonly string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(projectPath)
             .AddJsonFile("appsettings.json")
             .Build();

        string dataFolder;
        FileData sut;

        //method used in  mocking ILogger method
        string MockFormatter<TState>(TState state, Exception ex) { return "done"; }
        public FileDataTest()
        {
            //executes once per test

            dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, recipeFolder);
            loggerMock = new Mock<ILogger<FileData>>();
            loggerMock.Setup(m => m.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), MockFormatter));
            sut = new FileData(config, loggerMock.Object);

        }
        [Fact]
        public void CreateRecipe_Success()
        {
            var recipeName = new RecipeName("scrambled_eggs");
            var ingredient = new Ingredient("egg");
            var ingredient2 = new Ingredient("butter");
            var ingredient3 = new Ingredient("milk");
            var ingredient4 = new Ingredient("pepper");
            List<Ingredient> ings = new List<Ingredient>() { ingredient, ingredient2, ingredient3, ingredient4 };

            var instruction = new Instruction("mix eggs and milk");
            var instruction2 = new Instruction("heat skillet and add butter");
            var instruction3 = new Instruction("cook eggs until desiered doneness");
            List<Instruction> instructions = new List<Instruction>() { instruction, instruction2, instruction3 };

            var recip = new Recipe(recipeName, ings, instructions);
            var resultLink = sut.CreateRecipe(recip);

            Assert.NotNull(resultLink); 

        }
        [Fact]
        public void CreateRecipe_Name_Empty()
        {
            var recipeName = new RecipeName("");
            var ingredient = new Ingredient("egg");
            var ingredient2 = new Ingredient("butter");
            var ingredient3 = new Ingredient("milk");
            var ingredient4 = new Ingredient("pepper");
            List<Ingredient> ings = new List<Ingredient>() { ingredient, ingredient2, ingredient3, ingredient4 };

            var instruction = new Instruction("mix eggs and milk");
            var instruction2 = new Instruction("heat skillet and add butter");
            var instruction3 = new Instruction("cook eggs until desiered doneness");
            List<Instruction> instructions = new List<Instruction>() { instruction, instruction2, instruction3 };

            var recip = new Recipe(recipeName, ings, instructions);

            Assert.Throws<Exception>(() => sut.CreateRecipe(recip));

        }
        [Fact]
        public void CreateRecipe_Ingredients_Empty()
        {
            var recipeName = new RecipeName("scambled eggs");
            List<Ingredient> ings = new List<Ingredient>();

            var instruction = new Instruction("mix eggs and milk");
            var instruction2 = new Instruction("heat skillet and add butter");
            var instruction3 = new Instruction("cook eggs until desiered doneness");
            List<Instruction> instructions = new List<Instruction>() { instruction, instruction2, instruction3 };

            var recip = new Recipe(recipeName, ings, instructions);

         
            Assert.Throws<Exception>(() => sut.CreateRecipe(recip));

        }
        [Fact]
        public void CreateRecipe_Instructions_Empty()
        {
            var recipeName = new RecipeName("scambled eggs");
            var ingredient = new Ingredient("egg");
            var ingredient2 = new Ingredient("butter");
            var ingredient3 = new Ingredient("milk");
            var ingredient4 = new Ingredient("pepper");
            List<Ingredient> ings = new List<Ingredient>() { ingredient, ingredient2, ingredient3, ingredient4 };


            List<Instruction> instructions = new List<Instruction>();

            var recip = new Recipe(recipeName, ings, instructions);
            
            Assert.Throws<Exception>(() => sut.CreateRecipe(recip));

        }
        [Fact]
        public void GetRecipes()
        {
            var results = sut.GetRecipes();
            Assert.NotNull(results);
        }
        [Fact]
        public void GetRecipes_By_Ingredient()
        {
            var results = sut.GetRecipes("garlic");
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void ReadFileIntoString()
        {
            var files = Directory.GetFiles(dataFolder);
            Assert.NotNull(files);
            Assert.NotEmpty(files);

            string fileName = files[0];

            var resultOpt = sut.ReadFileIntoString(fileName);
            Assert.NotNull(resultOpt);
            Assert.True(resultOpt.isPresent());
          
        }
        [Fact]
        public void ParseFileToRecipe()
        {
            var results = sut.ParseFileToRecipe(kontents);
            Assert.NotNull(results);
            Assert.NotEmpty(results.ingredients);
            Assert.Equal(10, results.ingredients.Count);
            Assert.NotEmpty(results.instructions);
            Assert.True( results.instructions.Count>0);

        }
        const string kontents = @"
name
Chocolate Chunk Blondies

Ingredients

1/2 pound (2 sticks) unsalted butter, at room temperature
1 cup light brown sugar
1/2 cup granulated sugar
2 tsp pure vanilla extract
2 extra-large eggs, at room temperature
2 cups all purpose flour
1 tsp baking soda
1 tsp kosher salt
1 1/2 cup chopped walnuts
1 1/4 pounds semisweet chocolate chunks


Directions

Preheat the oven to 350F. Grease and flour an 8 x 12 x 2 inch baking pan.

In the bowl of an electric mixer fitted with the paddle attachment,
cream the butter, brown sugar, and granulated sugar on high speed 
for 3 minutes, until light and fluffy. With the mixer on low, add
the vanilla, then the eggs, one at a time, and mix well, scraping
down the bowl. In a small bowl, sift together the flour, baking soda
and salt and with the mixer still on low, slowly add flour mixture
to the butter mixture. Fold the walnuts and chocolate chunks in with
a rubber spatula.

Spread the batter into the preoared pan and smooth the top. Bake for
30 minutes exactly. Dont overbake! A toothpick may not come out
clean. Cool Completely in the pan and cut into bars.";
    }

    public class TestLogger<FileData> : ILogger, IDisposable
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
            return;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            return;
        }
    }


    public class TestConfiguration : IConfiguration
    {
        public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException();
        }
    }

}
