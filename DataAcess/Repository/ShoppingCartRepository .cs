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
    public class ShoppingCartRepository : Repsitory<ShoppingCart>, IShoppingCart
    {
        private readonly ApplicationDbContext _dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext) :base(dbContext)
        {
            this._dbContext = dbContext;
        }
     

        public void Update(ShoppingCart shoppingCart)
        {
            _dbContext.ShoppingCarts.Update(shoppingCart);
        }
    }
}
