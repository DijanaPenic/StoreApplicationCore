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
    public class BookRepositoryTests
    {
        private readonly IList<BookEntity> _books;
        private readonly IMapper _mapper;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public BookRepositoryTests()
        {
            // Intialize books
            _books = InitializeBooks();

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
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "UnitTestsBooksDatabase").Options;

            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            dbContext.Database.EnsureDeleted();
            dbContext.Books.AddRange(_books);
            dbContext.SaveChanges();
        }

        [Fact]
        [Trait("Category", "GET Book")]
        public async Task BookGetByIdAsync()
        {
            BookEntity book = _books.First();

            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookRepository _repository = new BookRepository(dbContext, _mapper);

            IBook result = await _repository.FindByIdAsync(book.Id);

            Assert.Equal(result.Id, book.Id);
        }

        [Fact]
        [Trait("Category", "GET Books")]
        public async Task BookGetAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookRepository _repository = new BookRepository(dbContext, _mapper);

            IEnumerable<IBook> result = await _repository.GetAsync();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_books.Count, result.Count());
        }

        [Fact]
        [Trait("Category", "GET Books")]
        public async Task BookGetPagedFilteredAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookRepository _repository = new BookRepository(dbContext, _mapper);

            string searchString = "Name";
            Expression<Func<IBook, bool>> filterExpression = b => b.Name.Contains(searchString) || b.Author.Contains(searchString) || b.Bookstore.Name.Contains(searchString);
            
            IPagedList<IBook> result = await _repository.FindAsync(filterExpression, true, nameof(IBook.Name), 1, 3);

            Assert.NotNull(result);
            Assert.Equal(1, result.TotalItemCount);
        }

        [Fact]
        [Trait("Category", "GET Books")]
        public async Task BookGetCollectionFilteredAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookRepository _repository = new BookRepository(dbContext, _mapper);

            string searchString = "Name";
            Expression<Func<IBook, bool>> filterExpression = b => b.Name.Contains(searchString) || b.Author.Contains(searchString) || b.Bookstore.Name.Contains(searchString);

            IEnumerable<IBook> result = await _repository.FindAsync(filterExpression, true, nameof(IBook.Name));

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        [Trait("Category", "UPDATE Book")]
        public async Task BookUpdateAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookRepository _repository = new BookRepository(dbContext, _mapper);

            ResponseStatus status = await _repository.UpdateAsync(new Book { Id = _books.First().Id, Name = "Name - updated", Author = "Author - updated" });

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "UPDATE Book")]
        public async Task BookUpdateByIdAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookRepository _repository = new BookRepository(dbContext, _mapper);

            ResponseStatus status = await _repository.UpdateAsync(_books.First().Id, new Book { Name = "Name - updated", Author = "Author - updated" });

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "INSERT Book")]
        public async Task BookInsertAsync()
        {
            using ApplicationDbContext dbContext = new ApplicationDbContext(_dbContextOptions);
            BookRepository _repository = new BookRepository(dbContext, _mapper);

            ResponseStatus status = await _repository.AddAsync(new Book { Name = "Name - created", Author = "Author - created" });

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "DELETE Book")]
        public async Task BookDeleteAsync()
        {
            using ApplicationDbContext dbContextDelete = new ApplicationDbContext(_dbContextOptions);
            BookRepository _repository = new BookRepository(dbContextDelete, _mapper);

            ResponseStatus status = await _repository.DeleteByIdAsync(_books.First().Id);

            Assert.Equal(ResponseStatus.Success, status);
        }

        private IList<BookEntity> InitializeBooks()
        {
            return new List<BookEntity>()
            {
                new BookEntity
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    Name = "Name",
                    Author = "Author",
                    Bookstore = new BookstoreEntity()
                    {
                        Id = GuidHelper.NewSequentialGuid(),
                        Name = "Name",
                        Location = "Location"
                    }
                }
            };
        }
    }
}