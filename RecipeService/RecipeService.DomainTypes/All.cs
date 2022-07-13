namespace RecipeService.DomainTypes
{
    public  record RecipeName ( string Name);
    public record RecipeID (long Val);
    public record Ingredient (string Val);
    public record Instruction (string Val);
    public record Recipe (RecipeName namen,List<Ingredient> ingredients, List<Instruction> instructions);
    public record RecipeLink(RecipeName recipeName, Uri recipeLink);

}
