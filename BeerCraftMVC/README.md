# BeerCraftMVC

## Обща концепция

Приложението BeerCraftMVC е платформа за любители на домашното пивоварство, която позволява:
- създаване и споделяне на рецепти за бира,
- управление на собствени съставки и наличности,
- оценяване и харесване на рецепти,
- използване на калкулатор за правилното им изпълнение,
- проследяване на характеристиките на различни съставки.

## База данни

### Основни таблици

- **User** (пази информация за потребителите)
  - user:recipe (1:M)
  - user:inventory (1:M)
  - user:likedRecipes (M:N)

- **Recipe** (пази информация за рецептите)
  - recipe:userAuthor (M:1)
  - recipe:ingredient (M:N)
  - recipe:likedRecipes (M:N)

- **Ingredient** (пази информация за съставките)
  - ingredient:characteristics (M:N)
  - ingredient:recipe (M:N)
  - ingredient:inventory (M:N)
  - ingredient:ingredientType (M:1)

- **IngredientType**
  - ingredientType:ingredient (1:M)

- **Inventory** (пази информация за наличностите на потребителите)

- **Characteristic** (пази информация за характеристиките на съставките)
  - characteristic:ingredient (M:N)

### Свързващи таблици (M:N)

- **LikedRecipes** (пази информация за харесаните рецепти от потребителите)
- **RecipeIngredient** (пази информация за съставките в рецептите)
- **IngredientCharacteristic** (свързва конкретни съставки с техните характеристики)

## Технологии
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- HTML, CSS

## Design Patterns

## Repository Pattern за CRUD операциите
## Singleton Pattern за управление на конфигурацията на локалния езиков модел
## DTO Pattern за пренос на данни между слоевете
