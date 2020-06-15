using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DemoApp.DataAccess.ExtensionMethods;
using DemoApp.DataAccess.Models.DataTable;
using DemoApp.DataAccess.Models.Paging;
using DemoApp.DataAccess.Models.SearchTable;
using DemoApp.Models.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NLog;

namespace DemoApp.DataAccess.Services
{
    #region Interface

    public interface IDataRepository<TEntity> : IDisposable
        where TEntity : class
    {
        #region DBSet

        DbSet<TEntity> GetDbSet();
        DbContext GetDbContext();

        #endregion

        #region Add Entity

        bool Add(TEntity entity);
        bool Add(params TEntity[] entities);
        bool Add(IEnumerable<TEntity> entities);

        #endregion

        #region Delete Entity

        bool Delete(int id);
        bool Delete(TEntity entity);
        bool Delete(object id);
        bool Delete(params TEntity[] entities);
        bool Delete(IEnumerable<TEntity> entities);

        #endregion

        #region Update Entity

        bool Update(TEntity entity);
        bool Update(params TEntity[] entities);
        bool Update(IEnumerable<TEntity> entities);

        #endregion

        #region Retrieve All Entities

        IEnumerable<TEntity> GetAll();

        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Retrieve Pagewise

        IPaginate<TEntity> GetPage(int page = 1,
            int size = AppConstants.DEFAULT_PAGESIZE,
            string[] relatedEntities = null,
            bool disableTracking = true);

        IPaginate<TEntity> GetPage(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            string[] relatedEntities = null,
            int page = 1,
            int size = AppConstants.DEFAULT_PAGESIZE,
            bool disableTracking = true);

        IPaginate<TResult> GetPage<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            string[] relatedEntities = null,
            int page = 1,
            int size = AppConstants.DEFAULT_PAGESIZE,
            bool disableTracking = true) where TResult : class;

        #endregion

        #region Retrieve Specific

        TEntity GetById(int id);

        IQueryable<TEntity> Query(string sql, params object[] parameters);

        DataSet GetDataSet(string sql, params object[] parameters);

        DataTable GetDataTable(string sql, params object[] parameters);

        Dictionary<string, dynamic> GetDictionaryForFirstRow(string sql, params object[] parameters);

        T GetScalerValue<T>(string sql, params object[] parameters);

        SearchTableResponse<T> GetGridData<T>(string sql, Action<DataTable, List<T>> callbackAction, params object[] parameters);

        TEntity Search(params object[] keyValues);

        TEntity Single(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true);

        #endregion

        #region Retrieve with Specification filters

        Task<IEnumerable<TEntity>> GetBySpecsAsync(ISpecification<TEntity> specs);
        Task<TEntity> GetFirstBySpecsAsync(ISpecification<TEntity> specs);
        Task<DataTableResponse> GetOptionResponse(DataTableOptions options);
        Task<DataTableResponse> GetOptionResponseWithSpec(DataTableOptions options, ISpecification<TEntity> specs);

        #endregion
    }

    #endregion

    public class DataRepository<TEntity> : IDataRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DataRepository(DbContext context)
        {
            _dbContext = context ?? throw new ArgumentException(nameof(context));
            _dbSet = _dbContext.Set<TEntity>();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        #region DBSet

        public DbSet<TEntity> GetDbSet()
        {
            return _dbSet;
        }

        public DbContext GetDbContext()
        {
            return _dbContext;
        }

        #endregion

        #region Add Entity

        public bool Add(TEntity entity)
        {
            try
            {
                _dbSet.Add(entity);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public bool Add(params TEntity[] entities)
        {
            try
            {
                _dbSet.AddRange(entities);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public bool Add(IEnumerable<TEntity> entities)
        {
            try
            {
                _dbSet.AddRange(entities);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        #endregion

        #region Delete Entity

        public bool Delete(int id)
        {
            try
            {
                var entity = _dbSet.Find(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    _dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public bool Delete(TEntity entity)
        {
            try
            {
                var existing = _dbSet.Find(entity);
                if (existing != null)
                {
                    _dbSet.Remove(existing);
                    _dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public bool Delete(object id)
        {
            try
            {
                var typeInfo = typeof(TEntity).GetTypeInfo();
                var key = _dbContext.Model.FindEntityType(typeInfo).FindPrimaryKey().Properties.FirstOrDefault();
                var property = typeInfo.GetProperty(key?.Name);
                if (property != null)
                {
                    var entity = Activator.CreateInstance<TEntity>();
                    property.SetValue(entity, id);
                    _dbContext.Entry(entity).State = EntityState.Deleted;
                    _dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    var entity = _dbSet.Find(id);
                    if (entity != null)
                        return Delete(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public bool Delete(params TEntity[] entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public bool Delete(IEnumerable<TEntity> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        #endregion

        #region Update Entity

        public bool Update(TEntity entity)
        {
            try
            {
                _dbSet.Update(entity);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public bool Update(params TEntity[] entities)
        {
            try
            {
                _dbSet.UpdateRange(entities);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public bool Update(IEnumerable<TEntity> entities)
        {
            try
            {
                _dbSet.UpdateRange(entities);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        #endregion

        #region Retrieve All Entities

        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.AsEnumerable();
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsEnumerable();
        }

        #endregion

        #region Retrieve Pagewise

        public IPaginate<TEntity> GetPage(int page = 1,
            int size = AppConstants.DEFAULT_PAGESIZE,
            string[] relatedEntities = null,
            bool disableTracking = true)
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;
                if (disableTracking)
                    query = query.AsNoTracking();

                if (relatedEntities != null && relatedEntities.Length != 0)
                {
                    foreach (string entity in relatedEntities)
                        query = query.Include(entity);
                }

                int index = page - 1;
                if (index < 0)
                    index = 0;

                return query.ToPaginate(index, size);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return default(IPaginate<TEntity>);
            }
        }

        public IPaginate<TEntity> GetPage(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            string[] relatedEntities = null,
            int page = 1,
            int size = AppConstants.DEFAULT_PAGESIZE,
            bool disableTracking = true)
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;
                if (disableTracking) query =
                    query.AsNoTracking();

                if (include != null)
                    query = include(query);

                if (predicate != null)
                    query = query.Where(predicate);

                if (relatedEntities != null && relatedEntities.Length != 0)
                {
                    foreach (string entity in relatedEntities)
                        query = query.Include(entity);
                }

                int index = page - 1;
                if (index < 0)
                    index = 0;

                return orderBy != null
                    ? orderBy(query).ToPaginate(index, size)
                    : query.ToPaginate(index, size);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return default(IPaginate<TEntity>);
            }
        }

        public IPaginate<TResult> GetPage<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            string[] relatedEntities = null,
            int page = 1,
            int size = AppConstants.DEFAULT_PAGESIZE,
            bool disableTracking = true) where TResult : class
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;
                if (disableTracking)
                    query = query.AsNoTracking();

                if (include != null)
                    query = include(query);

                if (predicate != null)
                    query = query.Where(predicate);

                if (relatedEntities != null && relatedEntities.Length != 0)
                {
                    foreach (string entity in relatedEntities)
                        query = query.Include(entity);
                }

                int index = page - 1;
                if (index < 0)
                    index = 0;

                return orderBy != null
                    ? orderBy(query).Select(selector).ToPaginate(index, size)
                    : query.Select(selector).ToPaginate(index, size);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return default(IPaginate<TResult>);
            }
        }

        #endregion

        #region Retrieve Specific

        public TEntity GetById(int id)
        {
            try
            {
                return _dbSet.Find(id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return default(TEntity);
            }
        }

        //public TEntity GetById(int id, params Type[] relatedEntities)
        //{
        //    try
        //    {
        //        IQueryable<TEntity> query = _dbSet;
        //        foreach (Type entityType in relatedEntities)
        //            query = query.Include(entityType.Name);
        //        return query.Find(x => x.Id == id);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex);
        //        return default(TEntity);
        //    }
        //}

        public IQueryable<TEntity> Query(string sql, params object[] parameters)
        {
            try
            {
                return _dbSet.FromSqlRaw(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return default(IQueryable<TEntity>);
            }
        }

        public DataSet GetDataSet(string sql, params object[] parameters)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddRange(parameters);
                    command.CommandType = CommandType.StoredProcedure;
                    _logger.Debug(command.CommandAsSql());

                    SqlDataAdapter da = new SqlDataAdapter(command as SqlCommand);
                    da.Fill(dataSet);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return dataSet;
        }

        public DataTable GetDataTable(string sql, params object[] parameters)
        {
            DataTable table = new DataTable();

            try
            {
                using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddRange(parameters);
                    command.CommandType = CommandType.StoredProcedure;
                    _logger.Debug(command.CommandAsSql());

                    _dbContext.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        table.Load(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return table;
        }

        public Dictionary<string, dynamic> GetDictionaryForFirstRow(string sql, params object[] parameters)
        {
            DataTable dataTable = GetDataTable(sql, parameters);
            Dictionary<string, dynamic> returnData = new Dictionary<string, dynamic>();

            if (dataTable?.Rows.Count > 0)
            {
                returnData = dataTable.GetDictionaryForFirstRow();
            }

            return returnData;
        }

        public T GetScalerValue<T>(string sql, params object[] parameters)
        {
            T returnData = default(T);

            try
            {
                using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddRange(parameters);
                    command.CommandType = CommandType.StoredProcedure;
                    _logger.Debug(command.CommandAsSql());

                    _dbContext.Database.OpenConnection();
                    returnData = (T)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return returnData;
        }

        public SearchTableResponse<T> GetGridData<T>(string sql, Action<DataTable, List<T>> callbackAction, params object[] parameters)
        {
            SearchTableResponse<T> response = new SearchTableResponse<T>();
            DataTable table = new DataTable();

            try
            {
                using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);
                    _logger.Debug(command.CommandAsSql());

                    _dbContext.Database.OpenConnection();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            // Set Total
                            response.TotalRecords = reader.GetInt32(0);
                            reader.NextResult();

                            // Obtain DataTable
                            table.Load(reader);

                            // Convert data table to list
                            response.Data = new List<T>();
                            if (callbackAction != null && table?.Rows.Count != 0)
                            {
                                callbackAction.Invoke(table, response.Data);
                            }

                            // Update status
                            response.Status = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return response;
        }

        public TEntity Search(params object[] keyValues)
        {
            try
            {
                return _dbSet.Find(keyValues);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return default(TEntity);
            }
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate = null,
                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                    bool disableTracking = true)
        {
            try
            {
                IQueryable<TEntity> query = _dbSet;
                if (disableTracking) query = query.AsNoTracking();

                if (include != null) query = include(query);

                if (predicate != null) query = query.Where(predicate);

                if (orderBy != null)
                    return orderBy(query).FirstOrDefault();
                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return default(TEntity);
            }
        }

        #endregion

        #region Retrieve with Specification filters

        public async Task<IEnumerable<TEntity>> GetBySpecsAsync(ISpecification<TEntity> spec)
        {
            return await _dbSet.ProcessSpecification(spec).ToListAsync();
        }

        public async Task<TEntity> GetFirstBySpecsAsync(ISpecification<TEntity> spec)
        {
            return await _dbSet.ProcessSpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<DataTableResponse> GetOptionResponse(DataTableOptions options)
        {
            return await _dbSet.GetOptionResponseAsync(options);
        }

        public async Task<DataTableResponse> GetOptionResponseWithSpec(DataTableOptions options, ISpecification<TEntity> spec)
        {
            return await _dbSet.ProcessSpecification(spec).GetOptionResponseAsync(options);
        }

        #endregion
    }
}
