using DataAcess.Repository.IReository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebStore.Data.Contexts;

namespace DataAcess.Repository
{
    public class Repsitory<T> : Irepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        internal DbSet<T> _dbset;

        public Repsitory(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
            _dbset = dbContext.Set<T>();
        }
        public void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entity)
        {
            _dbset.RemoveRange(entity);

        }

        public T Get(Expression<Func<T, bool>> Filter)
        {
            IQueryable<T> query = _dbset;
            query = query.Where(Filter);
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> values = _dbset;
            return values.ToList();
        }
    }
}
