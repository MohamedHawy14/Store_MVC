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
        public IProduct product { get ;private set; }
        public ICompany company { get ;private set; }
        public IShoppingCart shoppingCart { get ;private set; }
        public IApplicationUser applicationUser { get ;private set; }
        public IOrderHeader orderHeader { get ;private set; }
        public IOrderDetials orderDetials { get ;private set; }



        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
            category = new CategoryRepository(_dbContext);
            product = new ProductRepository(_dbContext);
            company = new CompanyRepository(_dbContext);
            shoppingCart = new ShoppingCartRepository(_dbContext);
            applicationUser = new ApplicationUserRepository(_dbContext);
            orderHeader = new OrderHeaderRepository(_dbContext);
            orderDetials = new OrderDetialsRepository(_dbContext);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
