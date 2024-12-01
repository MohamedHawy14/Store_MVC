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
    public class CategoryRepository : Repsitory<Category>, ICategory
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext) :base(dbContext)
        {
            this._dbContext = dbContext;
        }
     

        public void Update(Category category)
        {
            _dbContext.Categories.Update(category);
        }
    }
}
