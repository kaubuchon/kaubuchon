using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecipeService.DataSources;
using RecipeService.Interfaces;
using System;
using Xunit;
using Moq;

namespace RecipeService.Tests;
/// <summary>
/// These tests are for the IDataSource interface (i.e no internal implementation methods). These tests should pass regardless of the implementation that 
/// is used. They can be used for regression testing if an implementation is refactored, or a new implementation 
/// is created. (Replace the FileData type with other implementation when created.)
/// </summary>
public class IDataSourceTests
{  

   // string dataFolder;
    IDataSource dataSource;
    static readonly string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];

    Mock<ILogger<FileData>> loggerMock;

    //method used in mocking ILogger
    string MockFormatter<TState> (TState state, Exception ex) { return "done"; }

    public IDataSourceTests()
    {
       
        IConfiguration config = new ConfigurationBuilder()
           .SetBasePath(projectPath)
           .AddJsonFile("appsettings.json")
           .Build();

        loggerMock = new Mock<ILogger<FileData>>();
        loggerMock.Setup(m => m.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), MockFormatter));
       
        dataSource = new FileData(config, loggerMock.Object);
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