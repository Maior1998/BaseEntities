using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;

namespace BaseEntities
{
    public interface IOdataEntityRepository<T> where T : class, IBaseEntity
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
        public static IQueryable<T> GetAllEntities<T>(this IOdataEntityRepository<T> repository) where T : class, IBaseEntity
        {
            return repository.GetEntities(x => true);
        }

        public static IQueryable<T> GetSingleEntityById<T>(this IOdataEntityRepository<T> repository, Guid id) where T : class, IBaseEntity
        {
            return repository.GetEntities(x => x.Id == id);
        }
    }

    public abstract class HelperODataController<T> : ODataController where T : class, IBaseEntity
    {
        public IOdataEntityRepository<T> repository { get; }
        private IHttpContextAccessor httpContextAccessor { get; set; }
        protected HelperODataController(
            IOdataEntityRepository<T> repository,
            IHttpContextAccessor httpContextAccessor) : base()
        {
            this.repository = repository;
            this.httpContextAccessor = httpContextAccessor;
        }

        [EnableQuery]
        public IQueryable<T> Get()
        {
            return repository.GetAllEntities();
        }

        [EnableQuery]
        public SingleResult<T> Get(Guid key)
        {
            return SingleResult.Create(repository.GetEntities(x => x.Id == key));
        }

        public async Task<IActionResult> Post(T entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await repository.AddEntity(entity);
            return Created(entity);
        }

        public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<T> product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await repository.GetEntityById(key);
            if (entity == null)
            {
                return NotFound();
            }
            product.Patch(entity);
            await repository.UpdateEntity(entity);
            return Updated(entity);
        }

        public async Task<IActionResult> Put([FromODataUri] Guid key, T update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await repository.UpdateEntity(update);
            return Updated(update);
        }

        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            var product = await repository.GetEntityById(key);
            if (product == null)
            {
                return NotFound();
            }
            await repository.DeleteEntity(key);
            return NoContent();
        }
    }
}
