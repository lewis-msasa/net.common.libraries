using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Mongo.BaseModel;
using Common.Libraries.Services.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Mongo.Repositories
{
    public class MongoRepository<T, Database> : IRepository<T> where T : MongoEntity where Database: IMongoDatabase
    {
        private IMongoCollection<T> _collection;
        private readonly Database _database;

        public MongoRepository(Database database)
        {
          
            _database = database;
            _collection = _database.GetCollection<T>(nameof(T));
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default!)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default!)
        {
            var result = await _collection.CountDocumentsAsync(predicate);
            return Convert.ToInt32(result);
        }

        public async Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default!)
        {

            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            var result = await _collection.DeleteOneAsync(filter);
            return Convert.ToInt32(result.DeletedCount);

        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default!)
        {
            return await _collection.AsQueryable().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default!)
        {
            var result = await _collection.FindAsync(predicate);
            return await result.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string[] includeString = null, bool disableTracking = true, CancellationToken cancellationToken = default!)
        {
            var result = await _collection.FindAsync(predicate);
            return await result.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true, CancellationToken cancellationToken = default!)
        {
            var result = await _collection.FindAsync(predicate);
            return await result.ToListAsync();
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string[] includeString = null, bool disableTracking = true, CancellationToken cancellationToken = default!)
        {
            var result = await _collection.FindAsync(predicate);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetPaginatedAsync(int page, int size, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string[] includeString = null, bool disableTracking = true, CancellationToken cancellationToken = default!)
        {
            page = page != 0 ? page - 1 : page;
            var result =   _collection.Find(f => f!= null);
            return await result.Skip(page).Limit(size).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetPaginatedByCondtionAsync(Expression<Func<T, bool>> predicate, int page, int size, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string[] includeString = null, bool disableTracking = true, CancellationToken cancellationToken = default!)
        {
           
            page = page != 0 ? page - 1 : page;
            var result = _collection.Find(predicate);
            return await result.Skip(page).Limit(size).ToListAsync();
        }

        public async Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default!)
        {
            var result = await _collection.ReplaceOneAsync(x => x == entity,entity);
            return Convert.ToInt32(result.MatchedCount);

        }
    }
}
