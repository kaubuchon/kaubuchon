
## Introduction
Hello, I'm Kevin Aubuchon. This repo is a simple example of a web service that demonstrates some of my development thoughts.

## RecipeService
This RecipeService project is a web service that wraps a data store of recipes. The API allows for searchinga and creating a recipe.

### RecipeService endpoints

GET

`/recipe` -> return a list of recipe name and unique URL pairs

`/recipe/{recipe-id}` -> return a recipe

`/recipe/ingredient/{ingredient-name}` -> return a list of recipe name and URL pairs that use the ingredient in the request

POST

`/recipe` -> creates a recipe in the data store, returns the unique URL for it

## RecipeService projects

### RecipeService.DomainTypes
The datatypes used by the service and client. In this small project there aren't any internal only datatypes so I have only 
one project for all datatypes. A more realistic service may separate client datatypes into a separate DLL to help a .NET client
send/recieve requests. (A non .NET client would need to send the correct json request)

### RecipeService.Interfaces
The interfaces used by the RecipeService. The data source that contains the recipes is abstracted from the service via
the IDataSource interface. This interface must be implemented for each datastore (i.e. database, blobs, etc). This
sample application stores the recipes as text files in a local folder.

### RecipeService
This project is the web service that receives the API calls and then calls the appropriate IDataSource method to fulfill 
the request.

### RecipeService.Tests
The test project for unit testing and interface testing.

### Interface IDataSource
This interface is the central functionality of the service.

```
  public interface IDataSource
    {
        List<RecipeLink> GetRecipes();
        Optional<Recipe> GetRecipe(RecipeID id);
        List<RecipeLink> GetRecipes(string ingredientName);
        RecipeLink CreateRecipe(Recipe recipe);
    }
```

**List<RecipeLink> GetRecipes();**

    Returns all recipes.

**Optional<Recipe> GetRecipe(RecipeID id);**

    Returns one recipe indicated by the RecipeID object. To handle the case where the RecipeID is not found an Optional instance is returned.

**List<RecipeLink> GetRecipes(string ingredientName);**

    Returns a list of recipes that use the ingredient passed in the request.

**RecipeLink CreateRecipe(Recipe recipe);**

    This method is used to create a new Recipe object in the data store, using the Recipe object passed from the caller.


### Testing
**Unit Testing**

I define unit testing as code that tests classes and sometimes their methods. Ideally class details should be encapsulated
and tested via their published (or public) methods. However sometimes it is more practical to directly test individual
methods. Like most programming decisions it often depends on the circumstances. I am testing class FileData by calling its
internal methods.

**Interface Testing**

Interface testing only tests the published interface methods and not the internal methods of any implementation. 
Class IDataSourceTest tests the interface methods. 

**What's the difference?**

There can be some overlap between interface testing and unit testing. Obviously in test class IDataSourceTest an instance of 
FileData is created. However if a new implementation is created, it could be used instead of FileData and all the tests should
still pass. That point is key. You have a known baseline for making changes. The interface test can be used as a regression test. 
The caveat here is the purpose of the interface does not change, only the implementation.

If FileData has a bug, or needs to be refactored, then the interface tests should still pass. This ensures fixing one
item did not break something else.

Since FileData does have some methods with non-trivial logic, I exposed them to the testing project so they could be
tested. I put these tests in a separate test class because the tests could  be changed in the future if the 
FileData implementation changed (e.g.  a developer combines some of the methods or similar change).

