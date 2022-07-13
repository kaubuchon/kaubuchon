using RecipeService.DataSources;
using RecipeService.Interfaces;
using System;
using System.IO;
using Xunit;

namespace RecipeService.Tests;
/// <summary>
/// These tests are for the IDataSource interface. The should pass regardless of the implementation that 
/// is used.They can be used for regression testing if an implementation is refactored, or a new implementation 
/// is created.
/// </summary>
public class IDataSourceTests
{
    const string recipeFolder = "recipes";

    string dataFolder;
    IDataSource dataSource;

    public IDataSourceTests()
    {
        dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, recipeFolder);
        dataSource = new FileData(dataFolder);
    }
   
    [Fact]
    public void GetRecipes_All()
    {       
        var result = dataSource.GetRecipes();
        Assert.NotNull(result);
    }
    [Fact]
    public void GetRecipe_By_ID()
    {       
        var result = dataSource.GetRecipe(new DomainTypes.RecipeID(577L));
        Assert.NotNull(result);
    }
    [Fact]
    public void GetRecipe_By_Name()
    {       
        var result = dataSource.GetRecipes("sausage");
        Assert.NotNull(result);
    }
}