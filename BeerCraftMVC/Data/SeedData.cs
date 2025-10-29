using BeerCraftMVC.Models;
using BeerCraftMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BeerCraftMVC.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new BeerCraftDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<BeerCraftDbContext>>()))
            {
                if (context.Recipes.Any())
                {
                    return;   
                }

                var user1 = new User
                {
                    Username = "brewmaster",
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword("password123"), 
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(user1);
                await context.SaveChangesAsync();

                var maltType = new IngredientType { Name = "Malt" };
                var hopType = new IngredientType { Name = "Hop" };
                var yeastType = new IngredientType { Name = "Yeast" };
                var additiveType = new IngredientType { Name = "Additive" };
                context.IngredientTypes.AddRange(maltType, hopType, yeastType, additiveType);
                await context.SaveChangesAsync(); 

                var paleMalt = new Ingredient { Name = "Pale Malt (2 Row)", IngredientTypeId = maltType.Id };
                var crystalMalt = new Ingredient { Name = "Crystal Malt (60L)", IngredientTypeId = maltType.Id };
                var cascadeHops = new Ingredient { Name = "Cascade Hops", IngredientTypeId = hopType.Id };
                var magnumHops = new Ingredient { Name = "Magnum Hops", IngredientTypeId = hopType.Id };
                var us05Yeast = new Ingredient { Name = "SafAle US-05", IngredientTypeId = yeastType.Id };
                var irishMoss = new Ingredient { Name = "Irish Moss", IngredientTypeId = additiveType.Id };
                context.Ingredients.AddRange(paleMalt, crystalMalt, cascadeHops, magnumHops, us05Yeast, irishMoss);
                await context.SaveChangesAsync(); 


                var recipe1 = new Recipe
                {
                    Name = "Classic American Pale Ale",
                    Description = "A balanced and refreshing Pale Ale featuring Cascade hops.",
                    CreatedByUserId = user1.Id, 
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    RecipeIngredients = new List<RecipeIngredient>
                    {
                        new RecipeIngredient { IngredientId = paleMalt.Id, Quantity = 5.0, Unit = "kg" },
                        new RecipeIngredient { IngredientId = crystalMalt.Id, Quantity = 0.25, Unit = "kg" },
                        new RecipeIngredient { IngredientId = cascadeHops.Id, Quantity = 30, Unit = "g" }, 
                        new RecipeIngredient { IngredientId = magnumHops.Id, Quantity = 15, Unit = "g" },
                        new RecipeIngredient { IngredientId = us05Yeast.Id, Quantity = 1, Unit = "packet" },
                        new RecipeIngredient { IngredientId = irishMoss.Id, Quantity = 1, Unit = "tsp" } 
                    }
                };

                var recipe2 = new Recipe
                {
                    Name = "Simple Blonde Ale",
                    Description = "An easy-drinking, light ale perfect for beginners.",
                    CreatedByUserId = user1.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    RecipeIngredients = new List<RecipeIngredient>
                    {
                        new RecipeIngredient { IngredientId = paleMalt.Id, Quantity = 4.0, Unit = "kg" },
                        new RecipeIngredient { IngredientId = cascadeHops.Id, Quantity = 20, Unit = "g" },
                        new RecipeIngredient { IngredientId = us05Yeast.Id, Quantity = 1, Unit = "packet" }
                    }
                };

                context.Recipes.AddRange(recipe1, recipe2);

                await context.SaveChangesAsync();
            }
        }
    }
}