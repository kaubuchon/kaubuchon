using RecipeService.DataSources;
using System;
using System.IO;
using Xunit;


namespace RecipeService.Tests
{
    /// <summary>
    /// This test is for the FileData implemenation of interface IDataSource. These tests while useful, are fragile
    /// since refactoring of FileData will break them. 
    /// </summary>
    public class FileDataTest
    {
        const string recipeFolder = "recipes";

        string dataFolder;
        public FileDataTest()
        {
            dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, recipeFolder);
        }

        [Fact]
        public void GetRecipes()
        {           

            var sut = new FileData(dataFolder);
            var results = sut.GetRecipes();
            Assert.NotNull(results);
        }
        [Fact]
        public void GetRecipes_By_Ingredient()
        {
            var sut = new FileData(dataFolder);
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

            var sut = new FileData(dataFolder);

            var resultOpt = sut.ReadFileIntoString(fileName);
            Assert.NotNull(resultOpt);
            Assert.True(resultOpt.isPresent());
          
        }
        [Fact]
        public void ParseFileToRecipe()
        {
            var sut = new FileData(dataFolder);
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
}
