using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CoreBackend.Api.Dto;

namespace CoreBackend.Api.Service
{
    public class ProductService
    {
        public static ProductService Current { get; } = new ProductService();

        public List<Product> Products { get; set; }

        public ProductService()
        {
            Products = new List<Product> {
                new Product{
                    Id = 1,
                    Name ="p1",
                    Price =12,
                    Description = "Description1",
                    Materials = new List<Material>{
                        new Material{
                            Id=1,
                            Name="m1"
                        },
                        new Material{
                            Id=2,
                            Name="m2"
                        }
                    }
                },
                new Product{
                    Id = 2,
                    Name ="p2",
                    Price =55,
                    Description = "Description2",
                    Materials = new List<Material>{
                        new Material{
                            Id=3,
                            Name="m3"
                        },
                        new Material{
                            Id=4,
                            Name="m4"
                        }
                    }
                }
            };
        }
    }
}
