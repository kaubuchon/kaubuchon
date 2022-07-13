using RecipeService.DomainTypes;

namespace RecipeService.Interfaces
{
    public interface IDataSource
    {
        List<RecipeLink> GetRecipes();
        Optional<Recipe> GetRecipe(RecipeID id);
        List<RecipeLink> GetRecipes(string ingredientName);
    }
}