﻿using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BaseEntities
{
    public abstract class EFOdatatityRepository<TentityType, TContextType>
        : IOdataEntityRepository<TentityType>
        where TentityType : class, IBaseEntity
        where TContextType : DbContext
    {
        protected abstract Expression<Func<TContextType, DbSet<TentityType>>> EntityDbSetSelector { get; }
        protected abstract TContextType ContextType { get; }
        private Func<TContextType, DbSet<TentityType>> entityDbSetFunc;
        private Func<TContextType, DbSet<TentityType>> EntityDbSetFunc => entityDbSetFunc ??= EntityDbSetSelector.Compile();



        public async Task AddEntity(TentityType entity)
        {
            entity.CreatedOn = DateTime.Now;
            TContextType contextType = ContextType;
            await EntityDbSetFunc(contextType).AddAsync(entity);
            BeforeEntityAdd(entity);
            await contextType.SaveChangesAsync();
            var eventTriggerTask = AfterEntityAdd(entity);
            var disposingTask = contextType.DisposeAsync().AsTask();
            Task.WaitAll(eventTriggerTask, disposingTask);
        }

        public async Task DeleteEntity(Guid id)
        {
            TContextType contextType = ContextType;
            DbSet<TentityType> entityDbSet = EntityDbSetFunc(contextType);
            TentityType entity = await entityDbSet.SingleAsync(x => x.Id == id);
            entityDbSet.Remove(entity);
            BeforeEntityDelete(entity);
            await contextType.SaveChangesAsync();
            var eventTriggerTask = AfterEntityDelete(entity);
            var disposingTask = contextType.DisposeAsync().AsTask();
            Task.WaitAll(eventTriggerTask, disposingTask);
        }

        public IQueryable<TentityType> GetEntities(Expression<Func<TentityType, bool>> predicate)
        {
            TContextType contextType = ContextType;
            DbSet<TentityType> entityDbSet = EntityDbSetFunc(contextType);
            return entityDbSet.Where(predicate).AsNoTracking();

        }

        public async Task UpdateEntity(TentityType entity)
        {
            TContextType contextType = ContextType;
            contextType.Attach(entity);
            entity.ModifiedOn = DateTime.Now;
            BeforeEntityUpdate(entity);
            await contextType.SaveChangesAsync();
            var eventTriggerTask = AfterEntityUpdate(entity);
            var disposingTask = contextType.DisposeAsync().AsTask();
            Task.WaitAll(eventTriggerTask, disposingTask);
        }

        protected virtual void BeforeEntityUpdate(TentityType entity) { }
        protected virtual Task AfterEntityUpdate(TentityType entity) => Task.CompletedTask;
        protected virtual void BeforeEntityAdd(TentityType entity) { }
        protected virtual Task AfterEntityAdd(TentityType entity) => Task.CompletedTask;
        protected virtual void BeforeEntityDelete(TentityType entity) { }
        protected virtual Task AfterEntityDelete(TentityType entity) => Task.CompletedTask;
    }
}
