using Microsoft.EntityFrameworkCore;
using Pronia.Models;

namespace Pronia.DAL
{
    public class AppDbContext: DbContext
    {

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }

        public AppDbContext(DbContextOptions options):base(options)
        {
            
        }
    }
}
