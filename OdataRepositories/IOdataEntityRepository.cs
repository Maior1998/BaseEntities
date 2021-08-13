using BaseEntities;

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

namespace OdataRepositories
{
    /// <summary>
    /// Представляет репозиторий, содержащий необходимые для поддержки протокола OData методы.
    /// </summary>
    /// <typeparam name="T">Тип сущности, которая будет обрабатываться этим репозиторием.</typeparam>
    public interface IOdataEntityRepository<T> where T : class, IBaseEntity
    {
        /// <summary>
        /// Получает все эекземпляры данной сущности в виде коллекции, поддерживающей запросы.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetEntities(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Обновляет одну сущность.
        /// </summary>
        /// <param name="entity">Сущность, которую необходимо обновить в хранилище.</param>
        /// <returns>Задача обновления сущности.</returns>
        public Task UpdateEntity(T entity);
        /// <summary>
        /// Добавляет новую сущность в хранилище.
        /// </summary>
        /// <param name="entity">Сущность, которую нужно добавить в хранилище.</param>
        /// <returns>Задача обновления сущности.</returns>
        public Task AddEntity(T entity);
        /// <summary>
        /// Удаляет указанную сущность из хранилища.
        /// </summary>
        /// <param name="id">Id записи сущности, которую нужно удалить из хранилища.</param>
        /// <returns>Задача удаления сущности.</returns>
        public Task DeleteEntity(Guid id);

    }

    public static class OdataEntityRepositoryExtensions
    {
        /// <summary>
        /// Возвращает все записи данной сущности из хранилища.
        /// </summary>
        /// <typeparam name="T">Тип сущности, которую необходимо использовать.</typeparam>
        /// <param name="repository">Репозиторий, из которого необходимо вытащить все сущности.</param>
        /// <returns>Коллекция всех сущностей из хранилища, поддерживающая запросы.</returns>
        public static IQueryable<T> GetAllEntities<T>(this IOdataEntityRepository<T> repository) where T : class, IBaseEntity
        {
            return repository.GetEntities(x => true);
        }
        /// <summary>
        /// Возвращает коллекцию из всех записей, Id которых будет равен указанному. По факту, будет прилетать одна сущность, но поддержка запросов нужна для работы OData.
        /// </summary>
        /// <typeparam name="T">Тип сущности, которую необходимо использовать.</typeparam>
        /// <param name="repository">Репозиторий, который необходимо использовать при получении сущности.</param>
        /// <param name="id">Id записи, которой должна соответствовать сущность.</param>
        /// <returns>Коллекция (из одной сущности), поддерживающая запросы.</returns>
        public static IQueryable<T> GetSingleEntityById<T>(this IOdataEntityRepository<T> repository, Guid id) where T : class, IBaseEntity
        {
            return repository.GetEntities(x => x.Id == id);
        }

        /// <summary>
        /// Получает один экзампляр сущности по ее Id.
        /// </summary>
        /// <typeparam name="T">Тип сущности, которую необходимо получить.</typeparam>
        /// <param name="repository">Репозиторий, из которого необходимо вытащить сущность.</param>
        /// <param name="id">Id записи.</param>
        /// <returns>Задача получения одной сущности из хранилища.</returns>
        public static async Task<T?> GetEntityById<T>(this IOdataEntityRepository<T> repository, Guid id) where T : class, IBaseEntity
        {
            return await repository.GetEntities(x => x.Id == id).SingleOrDefaultAsync();
        }
    }


}
