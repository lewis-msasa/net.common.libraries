using Common.Libraries.Services.Dtos;
using Common.Libraries.Services.EFCore.Repositories;
using Common.Libraries.Services.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.EFCore.Tests
{
    public class MyEntity : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MyDto : IDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<MyEntity> MyEntities { get; set; }
    }


    public class EFRepositoryIntegrationTests : IDisposable
    {
        private readonly TestDbContext _context;
        private readonly EFRepository<MyEntity, TestDbContext> _repo;

        public EFRepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh db
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureCreated();
            _repo = new EFRepository<MyEntity, TestDbContext>(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AddAsync_Should_Save_Entity()
        {
            var e = new MyEntity { Name = "Test" };
            var added = await _repo.AddAsync(e);

            Assert.Equal(e, added);
            Assert.Single(await _context.MyEntities.ToListAsync());
        }

        [Fact]
        public async Task GetOneAsync_WithPredicate_Returns_Correct()
        {
            var e1 = new MyEntity { Name = "A" };
            var e2 = new MyEntity { Name = "B" };
            await _repo.AddAsync(e1);
            await _repo.AddAsync(e2);

            var result = await _repo.GetOneAsync(ent => ent.Name == "B");
            Assert.NotNull(result);
            Assert.Equal("B", result.Name);
        }

        [Fact]
        public async Task GetPaginatedAsync_Returns_Correct_Page()
        {
            for (int i = 0; i < 10; i++)
                await _repo.AddAsync(new MyEntity { Name = $"E{i}" });

            var page2 = await _repo.GetPaginatedAsync(2, 3); // page=2,size=3 => skip 3, take 3
            Assert.Equal(3, page2.Count);
            Assert.Equal("E3", page2[0].Name);
        }

        [Fact]
        public async Task UpdateAsync_Changes_Entity()
        {
            var e = await _repo.AddAsync(new MyEntity { Name = "Old" });
            e.Name = "New";

            var count = await _repo.UpdateAsync(e);
            Assert.Equal(1, count);

            var fromDb = await _context.MyEntities.FindAsync(e.Id);
            Assert.Equal("New", fromDb.Name);
        }

        [Fact]
        public async Task DeleteAsync_Removes_Entity()
        {
            var e = await _repo.AddAsync(new MyEntity { Name = "X" });
            var result = await _repo.DeleteAsync(e);

            Assert.Equal(1, result);
            Assert.Empty(await _context.MyEntities.ToListAsync());
        }

        [Fact]
        public async Task CountAsync_Returns_Correct_Number()
        {
            await _repo.AddAsync(new MyEntity { Name = "1" });
            await _repo.AddAsync(new MyEntity { Name = "2" });

            var count = await _repo.CountAsync();
            Assert.Equal(2, count);
        }
    }

}
