using DataAcess.Repository.IReository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Data.Contexts;

namespace DataAcess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public ICategory category { get ;private set; }
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
            category = new CategoryRepository(_dbContext);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
