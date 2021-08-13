using BaseEntities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

using OdataBaseReporitories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OdataControllerBase.Controllers
{
    /// <summary>
    /// Представляет собой вспомогательный контроллер для организации и поддержания работы сервиса OData.
    /// </summary>
    /// <typeparam name="T">Тип сущности, которую будет предоставлять данный контроллер.</typeparam>
    public abstract class HelperODataController<T> : ODataController where T : class, IBaseEntity
    {
        public IOdataEntityRepository<T> repository { get; }
        protected HelperODataController(
            IOdataEntityRepository<T> repository) : base()
        {
            this.repository = repository;
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
            BeforeAddEntity(entity);
            entity.CreatedOn = DateTime.Now;
            await repository.AddEntity(entity);
            _ = AfterAddEntity(entity);
            return Created(entity);
        }

        public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<T> product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            T entity = await repository.GetEntityById(key);
            if (entity == null)
            {
                return NotFound();
            }
            BeforeSaveEntity(entity);
            product.Patch(entity);
            entity.ModifiedOn = DateTime.Now;
            await repository.UpdateEntity(entity);
            _ = AfterSaveEntity(entity);
            return Updated(entity);
        }

        public async Task<IActionResult> Put([FromODataUri] Guid key, T update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            BeforeSaveEntity(update);
            update.ModifiedOn = DateTime.Now;
            await repository.UpdateEntity(update);
            _ = AfterSaveEntity(update);
            return Updated(update);
        }

        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            T entity = await repository.GetEntityById(key);
            if (entity == null)
            {
                return NotFound();
            }
            BeforeDeleteEntity(entity);
            await repository.DeleteEntity(key);
            _ = AfterDeleteEntity(entity);
            return NoContent();
        }

        protected virtual void BeforeAddEntity(T entity) { }
        public virtual Task AfterAddEntity(T entity) => Task.CompletedTask;
        protected virtual void BeforeSaveEntity(T entity) { }
        public virtual Task AfterSaveEntity(T entity) => Task.CompletedTask;
        protected virtual void BeforeDeleteEntity(T entity) { }
        public virtual Task AfterDeleteEntity(T entity) => Task.CompletedTask;
    }
}
