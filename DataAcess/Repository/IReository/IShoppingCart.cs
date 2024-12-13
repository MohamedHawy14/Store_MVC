using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repository.IReository
{
    public interface IShoppingCart:Irepository<ShoppingCart> 
    {
        void Update(ShoppingCart shoppingCart);
    
    }
}
