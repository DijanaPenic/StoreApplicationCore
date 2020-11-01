using Xunit;
using System;
using X.PagedList;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore;

using Store.Models;
using Store.Model.Common.Models;
using Store.Entities;
using Store.DAL.Context;
using Store.Repositories;
using Store.Repository.Mapper;
using Store.Common.Enums;
using Store.Common.Helpers;

namespace Store.Repository.Tests
{
    public class BookstoreRepositoryTests
    {
        private readonly IList<BookstoreEntity> _bookstores;
        private readonly IMapper _mapper;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public BookstoreRepositoryTests()
        {
            // Intialize books
            _bookstores = InitializeBookstores();

            // Setup auto mapper
            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperRepositoryProfile>();
                cfg.AddExpressionMapping();
            });
            _mapper = mapperConfiguration.CreateMapper();

            // Setup database context - different DbContext instances are used to seed the database and run the tests. 
            // This ensures that the test is not using (or tripping over) entities tracked by the context when seeding. 
            // It also better matches what happens in web apps and services. 
            // Source: https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/testing-sample
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "UnitTestsBookstoresDatabase").Options;

            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            dbContext.Database.EnsureDeleted();
            dbContext.Bookstores.AddRange(_bookstores);
            dbContext.SaveChanges();
        }

        [Fact]
        [Trait("Category", "GET Bookstore")]
        public async Task BookstoreGetByIdAsync()
        {
            BookstoreEntity bookstore = _bookstores.First();

            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookstoreRepository _repository = new BookstoreRepository(dbContext, _mapper);

            IBookstore result = await _repository.FindByIdAsync(bookstore.Id);

            Assert.Equal(result.Id, bookstore.Id);
        }

        [Fact]
        [Trait("Category", "GET Bookstores")]
        public async Task BookstoreGetAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookstoreRepository _repository = new BookstoreRepository(dbContext, _mapper);

            IEnumerable<IBookstore> result = await _repository.GetAsync();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_bookstores.Count, result.Count());
        }

        [Fact]
        [Trait("Category", "GET Bookstores")]
        public async Task BookstoreGetPagedFilteredAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookstoreRepository _repository = new BookstoreRepository(dbContext, _mapper);

            string searchString = "Name";
            Expression<Func<IBookstore, bool>> filterExpression = b => b.Name.Contains(searchString) || b.Location.Contains(searchString);

            IPagedList<IBookstore> result = await _repository.FindAsync(filterExpression, true, nameof(IBookstore.Name), 1, 3);

            Assert.NotNull(result);
            Assert.Equal(1, result.TotalItemCount);
        }

        [Fact]
        [Trait("Category", "GET Bookstores")]
        public async Task BookstoreGetCollectionFilteredAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookstoreRepository _repository = new BookstoreRepository(dbContext, _mapper);

            string searchString = "Name";
            Expression<Func<IBookstore, bool>> filterExpression = b => b.Name.Contains(searchString) || b.Location.Contains(searchString);

            IEnumerable<IBookstore> result = await _repository.FindAsync(filterExpression, true, nameof(IBookstore.Name));

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        [Trait("Category", "UPDATE Bookstore")]
        public async Task BookstoreUpdateAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookstoreRepository _repository = new BookstoreRepository(dbContext, _mapper);

            ResponseStatus status = await _repository.UpdateAsync(new Bookstore { Id = _bookstores.First().Id, Name = "Name - updated", Location = "Location - updated" });

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "UPDATE Bookstore")]
        public async Task BookstoreUpdateByIdAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookstoreRepository _repository = new BookstoreRepository(dbContext, _mapper);

            ResponseStatus status = await _repository.UpdateAsync(_bookstores.First().Id, new Bookstore { Name = "Name - updated", Location = "Location - updated" });

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "INSERT Bookstore")]
        public async Task BookstoreInsertAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookstoreRepository _repository = new BookstoreRepository(dbContext, _mapper);

            ResponseStatus status = await _repository.AddAsync(new Bookstore { Name = "Name - created", Location = "Location - created" });

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "DELETE Bookstore")]
        public async Task BookstoreDeleteAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookstoreRepository _repository = new BookstoreRepository(dbContext, _mapper);

            ResponseStatus status = await _repository.DeleteByIdAsync(_bookstores.First().Id);

            Assert.Equal(ResponseStatus.Success, status);
        }

        private IList<BookstoreEntity> InitializeBookstores()
        {
            return new List<BookstoreEntity>()
            {
                new BookstoreEntity
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    Name = "Name",
                    Location = "Location"
                }
            };
        }
    }
}