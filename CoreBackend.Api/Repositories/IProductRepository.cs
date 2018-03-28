using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CoreBackend.Api.Entity;
using Microsoft.EntityFrameworkCore;

namespace CoreBackend.Api.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Products> GetProducts();
        Products GetProduct(int productId, bool includeMaterials);
        IEnumerable<Materials> GetMaterialsForProduct(int productId);
        Materials GetMaterialForProduct(int productId, int materialId);
        bool ProductExist(int pid);
        void AddProduct(Products products);
        bool Save();
        void DeleteProduct(Products products);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly MyContext _myContext;

        public ProductRepository(MyContext myContext)
        {
            _myContext = myContext;
        }

        public Materials GetMaterialForProduct(int productId, int materialId)
        {
            return _myContext.Materials.FirstOrDefault(x => x.ProductId == productId && x.Id == materialId);
        }

        public IEnumerable<Materials> GetMaterialsForProduct(int productId)
        {
            return _myContext.Materials.Where(x => x.ProductId == productId).ToList();
        }

        public Products GetProduct(int productId, bool includeMaterials)
        {
            if (includeMaterials)
            {
                return _myContext.Products.Include(x => x.Materials).FirstOrDefault(x => x.Id == productId);
            }
            return _myContext.Products.Find(productId);
        }

        public IEnumerable<Products> GetProducts()
        {
            return _myContext.Products.OrderBy(x=>x.Name).ToList();
        }

        public bool ProductExist(int pid)
        {
            return _myContext.Products.Any(x => x.Id == pid);
        }

        public void AddProduct(Products products)
        {
            _myContext.Products.Add(products);
        }

        public void DeleteProduct(Products products)
        {
            _myContext.Products.Remove(products);
        }

        public bool Save()
        {
            return _myContext.SaveChanges()>0;
        }
    }
}
