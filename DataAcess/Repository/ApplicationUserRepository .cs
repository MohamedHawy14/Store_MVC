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
    public class ApplicationUserRepository : Repsitory<ApplicationUser>, IApplicationUser
    {
        private readonly ApplicationDbContext _dbContext;

        public ApplicationUserRepository(ApplicationDbContext dbContext) :base(dbContext)
        {
            this._dbContext = dbContext;
        }
     
    }
}
