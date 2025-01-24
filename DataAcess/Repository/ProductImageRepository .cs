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
    public class ProductImageRepository : Repsitory<ProductImage>, IProductImage
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductImageRepository(ApplicationDbContext dbContext) :base(dbContext)
        {
            this._dbContext = dbContext;
        }
     

        public void Update(ProductImage productImage)
        {
            _dbContext.ProductImages.Update(productImage);
        }
    }
}
