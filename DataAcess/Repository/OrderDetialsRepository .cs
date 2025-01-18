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
    public class OrderDetialsRepository : Repsitory<OrderDetial>, IOrderDetials
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderDetialsRepository(ApplicationDbContext dbContext) :base(dbContext)
        {
            this._dbContext = dbContext;
        }
     

        public void Update(OrderDetial orderDetial)
        {
            _dbContext.OrderDetials.Update(orderDetial);
        }
    }
}
