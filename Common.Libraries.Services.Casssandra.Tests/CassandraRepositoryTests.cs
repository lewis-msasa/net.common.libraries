using Common.Libraries.Services.Entities;
using Xunit;
using Cassandra;
using Cassandra.Mapping;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Configurations;
using System;
using System.Threading.Tasks;
using System.Linq;
using Common.Libraries.Services.Cassandra.Repositories;
using Testcontainers.Cassandra;
using Moq;
using Cassandra;
using Testcontainers.Cassandra;
using Xunit;

namespace Common.Libraries.Services.Casssandra.Tests
{
    public class MyEntity : IEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    
   
//requires docker
public class CassandraRepositoryIntegrationTests : IAsyncLifetime
    {
        private readonly CassandraContainer _cassandra = new CassandraBuilder()
            .WithImage("cassandra:3.11")
            .WithCleanUp(true)
            .Build();
        private ISession _session;
        private CassandraRepository<MyEntity, ISession> _repo;

        public async Task InitializeAsync()
        {
            await _cassandra.StartAsync();
            var cluster = Cluster.Builder()
                .AddContactPoint(_cassandra.Hostname)
                .WithPort(_cassandra.GetMappedPublicPort(9042))
                .Build();
            _session = await cluster.ConnectAsync();
            await _session.ExecuteAsync(
                new SimpleStatement("CREATE KEYSPACE IF NOT EXISTS testks WITH replication = " +
                "{'class':'SimpleStrategy','replication_factor':1};"));
            _session.ChangeKeyspace("testks");
            await _session.ExecuteAsync(
                new SimpleStatement("CREATE TABLE IF NOT EXISTS MyEntity (" +
                "Id uuid PRIMARY KEY, Name text);"));

            _repo = new CassandraRepository<MyEntity, ISession>(_session);
        }

        public async Task DisposeAsync()
        {
            _session?.Dispose();
            await _cassandra.DisposeAsync();
        }

        [Fact]
        public async Task AddAndGetAllAsync_WorksCorrectly()
        {
            var e = new MyEntity { Id = Guid.NewGuid().ToString(), Name = "A" };
            await _repo.AddAsync(e);

            var all = await _repo.GetAllAsync();
            Assert.Single(all);
            Assert.Equal("A", all[0].Name);
        }

        [Fact]
        public async Task DeleteAndCountAsync_WorkProperly()
        {
            var e = new MyEntity { Id = Guid.NewGuid().ToString(), Name = "X" };
            await _repo.AddAsync(e);
            var countBefore = await _repo.CountAsync();
            var deleted = await _repo.DeleteAsync(e);
            var countAfter = await _repo.CountAsync();

            Assert.Equal(1, countBefore);
            Assert.Equal(1, deleted);
            Assert.Equal(0, countAfter);
        }

        [Fact]
        public async Task GetPaginatedAsync_Returns_CorrectSubset()
        {
            for (int i = 0; i < 5; i++)
                await _repo.AddAsync(new MyEntity { Id = Guid.NewGuid().ToString(), Name = $"N{i}" });

            var page2 = await _repo.GetPaginatedAsync(2, 2);
            Assert.Equal(2, page2.Count);
        }
    }

    


}