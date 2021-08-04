using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BaseEntities
{
    public interface IOdataEntityRepository<T> where T : IBaseEntity
    {
        public IQueryable<T> GetEntities(Expression<Func<T, bool>> predicate);
        public IQueryable<T> GetSingleEntity(Expression<Func<T, bool>> predicate);
        public Task<T> GetEntityById(Guid id);
        public Task UpdateEntity(T entity);
        public Task AddEntity(T entity);
        public Task DeleteEntity(Guid id);

    }

    public static class OdataEntityRepositoryExtensions
    {
        public static IQueryable<T> GetAllEntities<T>(this IOdataEntityRepository<T> repository) where T : IBaseEntity
        {
            return repository.GetEntities(x => true);
        }

        public static IQueryable<T> GetSingleEntityById<T>(this IOdataEntityRepository<T> repository, Guid id) where T : IBaseEntity
        {
            return repository.GetEntities(x => x.Id == id);
        }
    }
}
