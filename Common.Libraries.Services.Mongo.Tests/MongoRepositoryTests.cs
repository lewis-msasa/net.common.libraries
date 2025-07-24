using Common.Libraries.Services.Dtos;
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Mongo.BaseModel;
using Common.Libraries.Services.Mongo.Repositories;
using Mongo2Go;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Mongo.Tests
{
    public class MyEntity : MongoEntity
    {
    
        public string Name { get; set; }
    }

   
    public class MongoRepositoryIntegrationTests : IDisposable
    {
        private readonly MongoDbRunner _runner;
        private readonly IMongoDatabase _database;
        private readonly MongoRepository<MyEntity, IMongoDatabase> _repo;

        public MongoRepositoryIntegrationTests()
        {
            _runner = MongoDbRunner.Start();
            var client = new MongoClient(_runner.ConnectionString);
            _database = client.GetDatabase("TestDb");
            _repo = new MongoRepository<MyEntity, IMongoDatabase>(_database);
        }

        public void Dispose()
        {
            _runner.Dispose(); // stops MongoDB server
        }

        [Fact]
        public async Task AddAsync_Should_Insert_Entity()
        {
            var e = new MyEntity { Name = "Test" };
            var added = await _repo.AddAsync(e);
            Assert.Equal(e, added);
            var all = await _repo.GetAllAsync();
            Assert.Single(all);
        }

        [Fact]
        public async Task CountAsync_Should_Return_Correct_Count()
        {
            await _repo.AddAsync(new MyEntity { Name = "1" });
            await _repo.AddAsync(new MyEntity { Name = "2" });
            var count = await _repo.CountAsync(e => true);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Entity()
        {
            var e = new MyEntity { Name = "X" };
            await _repo.AddAsync(e);
            var deleted = await _repo.DeleteAsync(e);
            Assert.Equal(1, deleted);
            Assert.Empty(await _repo.GetAllAsync());
        }

        [Fact]
        public async Task GetPaginatedAsync_Should_Return_Page()
        {
            for (int i = 1; i <= 5; i++)
                await _repo.AddAsync(new MyEntity { Name = i.ToString() });
            var page = await _repo.GetPaginatedAsync(2, 2);
            Assert.Equal(2, page.Count);
        }
    }

}
