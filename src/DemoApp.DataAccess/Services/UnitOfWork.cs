using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.DataAccess.Services
{
    #region Interfaces

    public interface IUnitOfWork : IDisposable
    {
        IDataRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        int SaveChanges();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }

        TContext GetContext();
    }

    #endregion

    public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork, IUnitOfWork<TContext>
        where TContext : DbContext, IDisposable
    {
        private Dictionary<Type, object> _repositories;
        public TContext Context { get; }

        public UnitOfWork(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IDataRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new DataRepository<TEntity>(Context);
            return (IDataRepository<TEntity>)_repositories[type];
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public TContext GetContext()
        {
            return Context;
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
