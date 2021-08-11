using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BaseEntities
{
    public interface IOdataEntityRepository<T> where T : class, IBaseEntity
    {
        public IQueryable<T> GetEntities(Expression<Func<T, bool>> predicate);
        public Task UpdateEntity(T entity);
        public Task AddEntity(T entity);
        public Task DeleteEntity(Guid id);

    }

    public static class OdataEntityRepositoryExtensions
    {
        public static IQueryable<T> GetAllEntities<T>(this IOdataEntityRepository<T> repository) where T : class, IBaseEntity
        {
            return repository.GetEntities(x => true);
        }

        public static IQueryable<T> GetSingleEntityById<T>(this IOdataEntityRepository<T> repository, Guid id) where T : class, IBaseEntity
        {
            return repository.GetEntities(x => x.Id == id);
        }
        public static async Task<T?> GetEntityById<T>(this IOdataEntityRepository<T> repository, Guid id) where T : class, IBaseEntity
        {
            return await repository.GetEntities(x => x.Id == id).SingleOrDefaultAsync();
        }
    }


}
