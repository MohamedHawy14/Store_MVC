using DataAcess.Repository.IReository;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Data.Contexts;

namespace DataAcess.Repository
{
    public class ProductRepository : Repsitory<Product>, IProduct
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext) :base(dbContext)
        {
            this._dbContext = dbContext;
        }
     

        public void Update(Product product)
        {
            var ObjFromDb = _dbContext.Products.FirstOrDefault(e => e.Id == product.Id);
            if (ObjFromDb != null)
            {
                ObjFromDb.Title = product.Title;
                ObjFromDb.Description = product.Description;
                ObjFromDb.PriceList = product.PriceList;
                ObjFromDb.Price = product.Price;
                ObjFromDb.Price50 = product.Price50;
                ObjFromDb.Price100 = product.Price100;
                ObjFromDb.ISBN = product.ISBN;
                ObjFromDb.Author = product.Author;
                ObjFromDb.CategoryId = product.CategoryId;
                if (product.ImageUrl != null)
                {
                    ObjFromDb.ImageUrl = product.ImageUrl;
                }

            }
        }
    }
}
