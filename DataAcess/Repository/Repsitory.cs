using DataAcess.Repository.IReository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebStore.Data.Contexts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            _dbContext.Products.Include(e=>e.Category).Include(e=>e.CategoryId);
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

        public T Get(Expression<Func<T, bool>> Filter,string? IncludeProperties =null)
        {
            IQueryable<T> query = _dbset;
            query = query.Where(Filter);
            if (!string.IsNullOrEmpty(IncludeProperties))
            {
                foreach (var IncludeProperty in IncludeProperties.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(IncludeProperty);
                    
                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(string? IncludeProperties = null)
        {
            IQueryable<T> values = _dbset;
            if (!string.IsNullOrEmpty(IncludeProperties))
            {
                foreach (var IncludeProperty in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    values = values.Include(IncludeProperty);

                }
            }
            return values.ToList();
        }
    }
}
