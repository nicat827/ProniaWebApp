﻿using Microsoft.EntityFrameworkCore;
using Pronia.Models;

namespace Pronia.DAL
{
    public class AppDbContext: DbContext
    {

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Product> Products { get; set; }
        public AppDbContext(DbContextOptions options):base(options)
        {
            
        }
    }
}
