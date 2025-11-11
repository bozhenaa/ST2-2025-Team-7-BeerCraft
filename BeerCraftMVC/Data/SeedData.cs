using BeerCraftMVC.Models;
using BeerCraftMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
                // --- 1. СЪЗДАВАНЕ/ИЗВЛИЧАНЕ НА ПОТРЕБИТЕЛИ ---

                // Проверяваме за user1 ("brewmaster")
                var user1 = await context.Users.FirstOrDefaultAsync(u => u.Username == "brewmaster");
                if (user1 == null)
                {
                    user1 = new User
                    {
                        Username = "brewmaster",
                        Name = "John Doe",
                        Email = "john.doe@example.com",
                        HashedPassword = BCrypt.Net.BCrypt.HashPassword("password123"),
                        CreatedAt = DateTime.UtcNow.AddDays(-10) // По-стара дата
                    };
                    context.Users.Add(user1);
                }

                // Проверяваме за user2 ("boji")
                var bojiUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "boji");
                if (bojiUser == null)
                {
                    bojiUser = new User
                    {
                        Username = "boji",
                        Name = "Boji Petrov",
                        Email = "boji@example.com",
                        HashedPassword = BCrypt.Net.BCrypt.HashPassword("boji123"), // Смени паролата, ако искаш
                        CreatedAt = DateTime.UtcNow
                    };
                    context.Users.Add(bojiUser);
                }

                // Запазваме потребителите, за да сме сигурни, че имат ID-та
                await context.SaveChangesAsync();

                // --- 2. СЪЗДАВАНЕ НА ТИПОВЕ СЪСТАВКИ (Ако липсват) ---
                if (!context.IngredientTypes.Any())
                {
                    context.IngredientTypes.AddRange(
                        new IngredientType { Name = "Malt" },
                        new IngredientType { Name = "Hop" },
                        new IngredientType { Name = "Yeast" },
                        new IngredientType { Name = "Additive" }
                    );
                    await context.SaveChangesAsync();
                }

                // Взимаме ID-тата на типовете, които ще ни трябват
                var maltType = await context.IngredientTypes.FirstAsync(it => it.Name == "Malt");
                var hopType = await context.IngredientTypes.FirstAsync(it => it.Name == "Hop");
                var yeastType = await context.IngredientTypes.FirstAsync(it => it.Name == "Yeast");
                var additiveType = await context.IngredientTypes.FirstAsync(it => it.Name == "Additive");

                // --- 3. СЪЗДАВАНЕ НА СЪСТАВКИ (Ако липсват) ---

                // Взимаме имената на всички съществуващи съставки
                var existingIngredientNames = await context.Ingredients
                                                           .Select(i => i.Name.ToLower())
                                                           .ToHashSetAsync();

                // Дефинираме всички нужни съставки
                var ingredientsToSeed = new List<Ingredient>
                {
                    // Оригинални
                    new Ingredient { Name = "Pale Malt (2 Row)", IngredientTypeId = maltType.Id },
                    new Ingredient { Name = "Crystal Malt (60L)", IngredientTypeId = maltType.Id },
                    new Ingredient { Name = "Cascade Hops", IngredientTypeId = hopType.Id },
                    new Ingredient { Name = "Magnum Hops", IngredientTypeId = hopType.Id },
                    new Ingredient { Name = "SafAle US-05", IngredientTypeId = yeastType.Id },
                    new Ingredient { Name = "Irish Moss", IngredientTypeId = additiveType.Id },
                    
                    // Нови за рецептите на Boji
                    new Ingredient { Name = "Roasted Barley", IngredientTypeId = maltType.Id },
                    new Ingredient { Name = "Fuggle Hops", IngredientTypeId = hopType.Id },
                    new Ingredient { Name = "SafAle S-04", IngredientTypeId = yeastType.Id },
                    new Ingredient { Name = "Citra Hops", IngredientTypeId = hopType.Id },
                    new Ingredient { Name = "Munich Malt", IngredientTypeId = maltType.Id },
                    new Ingredient { Name = "Hallertau Hops", IngredientTypeId = hopType.Id }
                };

                // Добавяме само тези, които липсват
                foreach (var ingredient in ingredientsToSeed)
                {
                    if (!existingIngredientNames.Contains(ingredient.Name.ToLower()))
                    {
                        context.Ingredients.Add(ingredient);
                    }
                }
                await context.SaveChangesAsync(); // Запазваме всички НОВИ съставки

                // Взимаме всички съставки (стари и нови) в речник за лесен достъп
                var ingredientsDict = await context.Ingredients.ToDictionaryAsync(i => i.Name, i => i);

                // --- 4. СЪЗДАВАНЕ НА РЕЦЕПТИ (Ако липсват) ---

                // Взимаме имената на всички съществуващи рецепти
                var existingRecipeNames = await context.Recipes
                                                       .Select(r => r.Name.ToLower())
                                                       .ToHashSetAsync();

                var recipesToAdd = new List<Recipe>();

                // Оригинална рецепта 1 (Проверка по име)
                if (!existingRecipeNames.Contains("Classic American Pale Ale".ToLower()))
                {
                    recipesToAdd.Add(new Recipe
                    {
                        Name = "Classic American Pale Ale",
                        Description = "A balanced and refreshing Pale Ale featuring Cascade hops.",
                        CreatedByUserId = user1.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        RecipeIngredients = new List<RecipeIngredient>
                        {
                            new RecipeIngredient { IngredientId = ingredientsDict["Pale Malt (2 Row)"].Id, Quantity = 5.0, Unit = "kg" },
                            new RecipeIngredient { IngredientId = ingredientsDict["Crystal Malt (60L)"].Id, Quantity = 0.25, Unit = "kg" },
                            new RecipeIngredient { IngredientId = ingredientsDict["Cascade Hops"].Id, Quantity = 30, Unit = "g" },
                            new RecipeIngredient { IngredientId = ingredientsDict["Magnum Hops"].Id, Quantity = 15, Unit = "g" },
                            new RecipeIngredient { IngredientId = ingredientsDict["SafAle US-05"].Id, Quantity = 1, Unit = "packet" },
                            new RecipeIngredient { IngredientId = ingredientsDict["Irish Moss"].Id, Quantity = 1, Unit = "tsp" }
                        }
                    });
                }

                // Оригинална рецепта 2 (Проверка по име)
                if (!existingRecipeNames.Contains("Simple Blonde Ale".ToLower()))
                {
                    recipesToAdd.Add(new Recipe
                    {
                        Name = "Simple Blonde Ale",
                        Description = "An easy-drinking, light ale perfect for beginners.",
                        CreatedByUserId = user1.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        RecipeIngredients = new List<RecipeIngredient>
                        {
                            new RecipeIngredient { IngredientId = ingredientsDict["Pale Malt (2 Row)"].Id, Quantity = 4.0, Unit = "kg" },
                            new RecipeIngredient { IngredientId = ingredientsDict["Cascade Hops"].Id, Quantity = 20, Unit = "g" },
                            new RecipeIngredient { IngredientId = ingredientsDict["SafAle US-05"].Id, Quantity = 1, Unit = "packet" }
                        }
                    });
                }

                // --- НОВИ РЕЦЕПТИ ОТ "boji" ---

                // Рецепта 3 (от Boji)
                if (!existingRecipeNames.Contains("Boji's Dry Stout".ToLower()))
                {
                    recipesToAdd.Add(new Recipe
                    {
                        Name = "Boji's Dry Stout",
                        Description = "A classic roasty stout with a dry finish, from Boji's collection.",
                        CreatedByUserId = bojiUser.Id, // ID-то на "boji"
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        RecipeIngredients = new List<RecipeIngredient>
                        {
                            new RecipeIngredient { IngredientId = ingredientsDict["Pale Malt (2 Row)"].Id, Quantity = 3.5, Unit = "kg" },
                            new RecipeIngredient { IngredientId = ingredientsDict["Roasted Barley"].Id, Quantity = 0.5, Unit = "kg" },
                            new RecipeIngredient { IngredientId = ingredientsDict["Fuggle Hops"].Id, Quantity = 30, Unit = "g" },
                            new RecipeIngredient { IngredientId = ingredientsDict["SafAle S-04"].Id, Quantity = 1, Unit = "packet" }
                        }
                    });
                }

                // Рецепта 4 (от Boji)
                if (!existingRecipeNames.Contains("Boji's Citra IPA".ToLower()))
                {
                    recipesToAdd.Add(new Recipe
                    {
                        Name = "Boji's Citra IPA",
                        Description = "A modern, juicy IPA exploding with Citra hops. A Boji special.",
                        CreatedByUserId = bojiUser.Id, // ID-то на "boji"
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        RecipeIngredients = new List<RecipeIngredient>
                        {
                            new RecipeIngredient { IngredientId = ingredientsDict["Pale Malt (2 Row)"].Id, Quantity = 6.0, Unit = "kg" },
                            new RecipeIngredient { IngredientId = ingredientsDict["Citra Hops"].Id, Quantity = 100, Unit = "g" },
                            new RecipeIngredient { IngredientId = ingredientsDict["SafAle US-05"].Id, Quantity = 1, Unit = "packet" }
                        }
                    });
                }

                // Рецепта 5 (от Boji)
                if (!existingRecipeNames.Contains("Boji's Munich Helles".ToLower()))
                {
                    recipesToAdd.Add(new Recipe
                    {
                        Name = "Boji's Munich Helles",
                        Description = "A clean, malty German lager-style ale. Smooth and refreshing.",
                        CreatedByUserId = bojiUser.Id, // ID-то на "boji"
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        RecipeIngredients = new List<RecipeIngredient>
                        {
                            new RecipeIngredient { IngredientId = ingredientsDict["Pale Malt (2 Row)"].Id, Quantity = 2.5, Unit = "kg" },
                            new RecipeIngredient { IngredientId = ingredientsDict["Munich Malt"].Id, Quantity = 2.5, Unit = "kg" },
                            new RecipeIngredient { IngredientId = ingredientsDict["Hallertau Hops"].Id, Quantity = 20, Unit = "g" },
                            new RecipeIngredient { IngredientId = ingredientsDict["SafAle US-05"].Id, Quantity = 1, Unit = "packet" }
                        }
                    });
                }

                // Добавяме всички НОВИ рецепти към context-а
                if (recipesToAdd.Any())
                {
                    context.Recipes.AddRange(recipesToAdd);
                    await context.SaveChangesAsync(); // Запазваме само ако има какво ново да се добави
                }
            }
        }
    }
}