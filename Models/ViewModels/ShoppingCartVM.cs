using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ShoppingCartVM
    { 
        public IEnumerable<ShoppingCart> shoppingCartlist { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
