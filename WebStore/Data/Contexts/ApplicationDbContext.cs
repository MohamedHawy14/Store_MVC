using WebStore.Models;

namespace WebStore.Data.Contexts
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id=1 ,Name = "phone", DisplayOrder = 1 },
                new Category { Id =3, Name = "Iphone", DisplayOrder = 3 },
                new Category { Id=2, Name = "TVs", DisplayOrder = 2 },
                new Category { Id=4, Name = "Microwive", DisplayOrder = 5 }
                );
        }
    }
}
