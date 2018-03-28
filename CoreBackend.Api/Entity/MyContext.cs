using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace CoreBackend.Api.Entity
{
    public class MyContext : DbContext
    {
        /// <summary>
        /// method2
        /// </summary>
        /// <param name="options"></param>
        public MyContext(DbContextOptions<MyContext> options) :base(options)
        {
            //Database.EnsureCreated();
            Database.Migrate();
        }

        public DbSet<Products> Products { get; set; }

        public DbSet<Materials> Materials { get; set; }

        /// <summary>
        /// method1
        /// </summary>
        /// <param name="optionsBuilder"></param>
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("xxxxxxxxxxx");
        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //method 1
            //modelBuilder.Entity<Products>().HasKey(x=>x.Id);
            //modelBuilder.Entity<Products>().Property(x=>x.Name).IsRequired().HasMaxLength(20);
            //modelBuilder.Entity<Products>().Property(x => x.Price).HasColumnType("decimal(8,2)");

            //method 2
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new MaterialConfiguration());
        }
    }

    public static class MyContextExtensions
    {
        public static void EnsureSeedDataForContext(this MyContext context)
        {
            if (context.Products.Any())
            {
                return;
            }
            var products = new List<Products>
            {
                new Products
                {
                    Name = "牛奶",
                    Price = 23,
                    Description = "这是牛奶啊",
                    Materials = new List<Materials>{
                        new Materials{
                            Name = "水"
                        },
                        new Materials{
                            Name = "奶粉"
                        }
                    }
                },
                new Products
                {
                    Name = "面包",
                    Price = 45,
                    Description = "这是面包啊",
                    Materials = new List<Materials>
                    {
                        new Materials
                        {
                            Name = "面粉"
                        },
                        new Materials
                        {
                            Name = "糖"
                        }
                    }
                },
                new Products
                {
                    Name = "啤酒",
                    Price = 75,
                    Description = "这是啤酒啊",
                    Materials = new List<Materials>
                    {
                        new Materials
                        {
                            Name = "麦芽"
                        },
                        new Materials
                        {
                            Name = "地下水"
                        }
                    }
                }
            };
            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
