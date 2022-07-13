using Microsoft.AspNetCore.Mvc;
using RecipeService.DomainTypes;
using RecipeService.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeService.Controllers
{

    [ApiController]
    public class Recipe : ControllerBase
    {
        IDataSource _data;
        ILogger _logger;
        public Recipe(IDataSource dataSource, ILogger<Recipe> logger)
        {
            _data = dataSource;
            _logger = logger;
        }
        [HttpPost]
        [Route("recipe")]
        public IActionResult CreateRecipe(RecipeService.DomainTypes.Recipe newRecipe)
        {
            try
            {
                _logger.LogInformation("ENTER Recipe.Create()");
                RecipeLink link = _data.CreateRecipe(newRecipe);
              
                _logger.LogInformation("Recipe.CreateRecipe() {0} returned", link.recipeLink);

                return new OkObjectResult(link);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "/recipe (CreateRecipe)");
                return BadRequest(ex.Message);
            }
            finally
            {
                _logger.LogInformation("EXIT Recipe.Create()");
            }
        }
       
        [HttpGet]
        [Route("recipe")]
        public IActionResult Get()
        {
            try
            {
                _logger.LogInformation("ENTER Recipe.Get()");
                List<RecipeLink> links = _data.GetRecipes();
                if (links.Count < 1)
                    return NotFound();
                _logger.LogInformation("Recipe.Get() {0} recipes returned",links.Count);
                return new OkObjectResult(links);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "recipe");
                return BadRequest(ex.Message);
            }
            finally
            {
                _logger.LogInformation("EXIT Recipe");
            }
        }
        [HttpGet]
        [Route("recipe/{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                _logger.LogInformation("ENTER Recipe.Get({0})",id);
                var optResult = _data.GetRecipe(new RecipeID(id));
                if (optResult.isPresent())
                {
                    _logger.LogInformation("Recipe.Get({0}) recipe returned", id);
                    return new OkObjectResult(optResult.get());
                }
                else
                {
                    _logger.LogInformation("Recipe.Get({0}) recipe not found", id);
                    return new NotFoundResult();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "recipe/{0}", id);
                return BadRequest(ex.Message);
            }
          
        }
        [HttpGet]
        [Route("recipe/ingredient/{ingredientName}")]
        public IActionResult Get(string ingredientName)
        {
            try
            {
                _logger.LogInformation("ENTER Recipe.Get({0})", ingredientName);
                List<RecipeLink> links = _data.GetRecipes(ingredientName);
                _logger.LogInformation("Recipe.Get() {0} recipes returned", links.Count);
                return new OkObjectResult(links);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "recipe/ingredient/{0}",string.IsNullOrEmpty(ingredientName)?"null":ingredientName);
                return BadRequest(ex.Message);
            }

        }
    }
}