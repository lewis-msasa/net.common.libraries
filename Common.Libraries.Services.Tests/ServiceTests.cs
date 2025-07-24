using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Tests
{
    using Common.Libraries.Services.Dtos;
    using Common.Libraries.Services.Entities;
    using Common.Libraries.Services.Repositories;
    using Common.Libraries.Services.Services;
    using Moq;
    using System.Linq.Expressions;
    using Xunit;

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
    public class ServiceTests
    {
        private readonly Mock<IRepository<MyEntity>> _repoMock;
        private readonly Service<MyEntity, MyDto> _service;

        public ServiceTests()
        {
            _repoMock = new Mock<IRepository<MyEntity>>();
            _service = new Service<MyEntity, MyDto>(_repoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Calls_AddAsync_And_Returns_Entity()
        {
            var ent = new MyEntity { Id = 1, Name = "Test" };
            _repoMock.Setup(r => r.AddAsync(ent)).ReturnsAsync(ent);

            var result = await _service.CreateAsync(ent);

            Assert.Equal(ent, result);
            _repoMock.Verify(r => r.AddAsync(ent), Times.Once);
        }

        [Fact]
        public async Task CreateManyAsync_Adds_All_Entities_In_Order()
        {
            var list = new[]
            {
            new MyEntity { Id = 1 }, new MyEntity { Id = 2 }
        };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<MyEntity>()))
                .ReturnsAsync((MyEntity e) => e);

            var result = await _service.CreateManyAsync(list);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Id == 1);
            Assert.Contains(result, e => e.Id == 2);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<MyEntity>()), Times.Exactly(2));
        }

        [Fact]
        public async Task DeleteAsync_Delegates_To_Repository()
        {
            var ent = new MyEntity { Id = 3 };
            _repoMock.Setup(r => r.DeleteAsync(ent)).ReturnsAsync(1);

            var result = await _service.DeleteAsync(ent);

            Assert.Equal(1, result);
            _repoMock.Verify(r => r.DeleteAsync(ent), Times.Once);
        }

        [Fact]
        public async Task DeleteManyAsync_Deletes_All_Matching()
        {
            var list = new List<MyEntity>
        {
            new MyEntity { Id = 1 },
            new MyEntity { Id = 2 }
        };

            Expression<Func<MyEntity, bool>> predicate = e => e.Id > 0;
            _repoMock.Setup(r => r.GetAsync(predicate)).ReturnsAsync(list);
            _repoMock.Setup(r => r.DeleteAsync(It.IsAny<MyEntity>())).ReturnsAsync(1);

            var deletedCount = await _service.DeleteManyAsync(predicate);

            Assert.Equal(2, deletedCount);
            _repoMock.Verify(r => r.GetAsync(predicate), Times.Once);
            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<MyEntity>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetAsync_Returns_All()
        {
            var all = new List<MyEntity> { new MyEntity { Id = 5 } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(all);

            var result = await _service.GetAsync();

            Assert.Single(result);
            Assert.Equal(5, result.First().Id);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetOneAsync_Delegates_To_Repo()
        {
            MyEntity ent = new MyEntity { Id = 42 };
            _repoMock.Setup(r => r.GetOneAsync(
                    It.IsAny<Expression<Func<MyEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<MyEntity>, IOrderedQueryable<MyEntity>>>(),
                    null,
                    true))
                .ReturnsAsync(ent);

            var result = await _service.GetOneAsync(e => e.Id == 42);

            Assert.Equal(42, result.Id);
            _repoMock.Verify(r => r.GetOneAsync(
                It.IsAny<Expression<Func<MyEntity, bool>>>(),
                null, null, true), Times.Once);
        }

        [Fact]
        public async Task UpdateManyAsync_Adds_Counts_Correctly()
        {
            var list = new[]
            {
            new MyEntity { Id = 1 },
            new MyEntity { Id = 2 }
        };
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<MyEntity>())).ReturnsAsync(1);

            var total = await _service.UpdateManyAsync(list);

            Assert.Equal(2, total);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<MyEntity>()), Times.Exactly(2));
        }

        [Fact]
        public async Task CountAsync_Delegates_To_Repo()
        {
            _repoMock.Setup(r => r.CountAsync(null)).ReturnsAsync(42);

            var count = await _service.CountAsync();

            Assert.Equal(42, count);
            _repoMock.Verify(r => r.CountAsync(null), Times.Once);
        }
    }

    public class ServiceWithMappingTests
    {
        private readonly Mock<IRepository<MyEntity>> _repoMock;
        private readonly ServiceWithMapping<MyEntity, MyDto> _service;

        public ServiceWithMappingTests()
        {
            _repoMock = new Mock<IRepository<MyEntity>>();
            _service = new ServiceWithMapping<MyEntity, MyDto>(_repoMock.Object);
        }

        private MyDto Map(MyEntity e) => new() { Id = e.Id, Name = e.Name };
        private ICollection<MyDto> MapMany(ICollection<MyEntity> list) =>
            list.Select(e => Map(e)).ToList();
        [Fact]
        public async Task CreateAsync_WithMapping_ReturnsMappedDto()
        {
            var ent = new MyEntity { Id = 1, Name = "A" };
            _repoMock.Setup(r => r.AddAsync(ent)).ReturnsAsync(ent);

            var dto = await _service.CreateAsync(ent, Map);

            Assert.Equal(ent.Id, dto.Id);
            Assert.Equal(ent.Name, dto.Name);
            _repoMock.Verify(r => r.AddAsync(ent), Times.Once);
        }
        [Fact]
        public async Task CreateManyAsync_Mapping_ReturnsEntitiesAsDtos()
        {
            var list = new List<MyEntity>
            {
                new() { Id = 1 }, new() { Id = 2 }
            };
            _repoMock.Setup(r => r.AddAsync(It.IsAny<MyEntity>()))
                     .ReturnsAsync((MyEntity e) => e);

            var result = await _service.CreateManyAsync(list, MapMany);

            Assert.Equal(2, result.Count);
            Assert.IsType<MyDto>(result.First()); 
        }
        [Fact]
        public async Task GetAsync_WithMapping_ReturnsMappedDtos()
        {
            var data = new List<MyEntity> {
                new() { Id = 10, Name = "X" },
                new() { Id = 20, Name = "Y" }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(data);

            var result = await _service.GetAsync(MapMany);

            Assert.Collection(result,
                dto => {
                    Assert.Equal(10, dto.Id);
                    Assert.Equal("X", dto.Name);
                },
                dto => {
                    Assert.Equal(20, dto.Id);
                    Assert.Equal("Y", dto.Name);
                });

            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
        [Fact]
        public async Task GetOneAsync_NullMapping_ReturnsDefault()
        {
            var ent = new MyEntity { Id = 99 };
            _repoMock.Setup(r => r.GetOneAsync(
                It.IsAny<Expression<Func<MyEntity, bool>>>(),
                null, null, true))
                .ReturnsAsync(ent);

            var result = await _service.GetOneAsync(e => e.Id == 99, null, null);

            Assert.Null(result); // default(TD) since mapping is null
        }
        [Fact]
        public async Task DeleteManyAsync_DeletesAllMatching()
        {
            var data = new List<MyEntity> { new() { Id = 1 }, new() { Id = 2 } };
            Expression<Func<MyEntity, bool>> pred = e => true;
            _repoMock.Setup(r => r.GetAsync(pred)).ReturnsAsync(data);
            _repoMock.Setup(r => r.DeleteAsync(It.IsAny<MyEntity>()))
                     .ReturnsAsync(1);

            var count = await _service.DeleteManyAsync(pred);

            Assert.Equal(2, count);
        }




    }
}
