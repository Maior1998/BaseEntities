using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BaseEntities
{
    public abstract class EFOdatatityRepository<TentityType, TContextType> : IOdataEntityRepository<TentityType> where TentityType : class, IBaseEntity where TContextType : DbContext
    {
        protected abstract Expression<Func<TContextType, DbSet<TentityType>>> EntityDbSetSelector { get; }
        protected abstract TContextType ContextType { get; }
        private Func<TContextType, DbSet<TentityType>> entityDbSetFunc;
        private Func<TContextType, DbSet<TentityType>> EntityDbSetFunc => entityDbSetFunc ??= EntityDbSetSelector.Compile();

        public async Task AddEntity(TentityType entity)
        {
            TContextType contextType = ContextType;
            await EntityDbSetFunc(contextType).AddAsync(entity);
            await contextType.SaveChangesAsync();
        }

        public async Task DeleteEntity(Guid id)
        {
            TContextType contextType = ContextType;
            var entityDbSet = EntityDbSetFunc(contextType);
            var entity = await entityDbSet.SingleAsync(x => x.Id == id);
            entityDbSet.Remove(entity);
            await contextType.SaveChangesAsync();
        }

        public IQueryable<TentityType> GetEntities(Expression<Func<TentityType, bool>> predicate)
        {
            TContextType contextType = ContextType;
            var entityDbSet = EntityDbSetFunc(contextType);
            return entityDbSet.Where(predicate);
        }

        public async Task UpdateEntity(TentityType entity)
        {
            TContextType contextType = ContextType;
            contextType.Attach(entity);
            await contextType.SaveChangesAsync();
        }
    }
}
