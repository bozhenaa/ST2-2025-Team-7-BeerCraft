using Microsoft.EntityFrameworkCore;
using BeerCraftMVC.Models.Entities;

namespace BeerCraftMVC.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class BeerCraftDbContext : DbContext
    {
        public BeerCraftDbContext(DbContextOptions<BeerCraftDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientType> IngredientTypes { get; set; }
        public DbSet<Characteristic> Characteristics { get; set; }
        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<LikedRecipe> LikedRecipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<IngredientCharacteristic> IngredientCharacteristics { get; set; }


        //конфигурация

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //композитни ключове - Primary Key, които са съставени от две полета

            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

            modelBuilder.Entity<LikedRecipe>()
                .HasKey(lr=>new {lr.UserId, lr.RecipeId });

            modelBuilder.Entity<IngredientCharacteristic>()
                .HasKey(ic => new {ic.IngredientId, ic.CharacteristicId });

            modelBuilder.Entity<Inventory>()
                .HasKey(i => new { i.UserId, i.IngredientId });

            //дефиниране на връзките

            //RecipeIngredient
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId);
        
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId);

            //IngredientCharacteristic
            modelBuilder.Entity<IngredientCharacteristic>()
                .HasOne(ic => ic.Ingredient)
                .WithMany(i => i.IngredientCharacteristics)
                .HasForeignKey(ic => ic.IngredientId);

            modelBuilder.Entity<IngredientCharacteristic>()
                .HasOne(ic => ic.Characteristics)
                .WithMany(c => c.IngredientCharacteristics)
                .HasForeignKey(ic => ic.CharacteristicId);

           //LikedRecipe
            modelBuilder.Entity<LikedRecipe>()
                .HasOne(lr => lr.Recipe)
                .WithMany(r => r.LikedByUsers)
                .HasForeignKey(lr => lr.RecipeId);  

            modelBuilder.Entity<LikedRecipe>()
                .HasOne(lr => lr.User)
                .WithMany(u => u.LikedRecipes)
                .HasForeignKey(lr => lr.UserId);

            //Recipes 
            modelBuilder.Entity<Recipe>()
                .HasOne<User>()
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict); //не трий потребителя, ако има създадени рецепти

            //Inventory
            modelBuilder.Entity<Inventory>()
            .HasOne(inv => inv.User)             
            .WithMany(u => u.Inventory)   
            .HasForeignKey(inv => inv.UserId);

            modelBuilder.Entity<Inventory>()
            .HasOne(inv => inv.Ingredient)     
            .WithMany(ing => ing.UserInventories)
            .HasForeignKey(inv => inv.IngredientId);


        }
    }
}
