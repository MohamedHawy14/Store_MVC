using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repository.IReository
{
    public interface IUnitOfWork
    {
        ICategory category { get;  }
        IProduct product { get; }
        ICompany company { get; }
        IShoppingCart shoppingCart { get; }
        IApplicationUser applicationUser { get; }
        IOrderDetials orderDetials { get; }
        IOrderHeader orderHeader { get; }
        IProductImage productImage { get; }

        void Save();

    }
}
