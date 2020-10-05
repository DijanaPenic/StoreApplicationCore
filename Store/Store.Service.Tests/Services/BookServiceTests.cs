using Moq;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using X.PagedList;

using Store.Models;
using Store.Services;
using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Model.Common.Models;
using Store.Repository.Common.Core;
using Store.Repository.Common.Repositories;

namespace Store.Service.Tests.Services
{
    public class BookServiceTests
    {
        private readonly IList<Book> _books;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            // Setup book test data
            _books = InitializeBooks();

            // Setup mock repository
            Mock<IBookRepository> mockBookRepository = new Mock<IBookRepository>();

            mockBookRepository.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_books.First());
            mockBookRepository.Setup(r => r.GetAsync()).ReturnsAsync(_books.AsEnumerable());
            mockBookRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<IBook, bool>>>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(_books.ToPagedList(1, 1));

            mockBookRepository.Setup(r => r.UpdateAsync(It.IsAny<IBook>())).ReturnsAsync(ResponseStatus.Success);
            mockBookRepository.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IBook>())).ReturnsAsync(ResponseStatus.Success);

            mockBookRepository.Setup(r => r.AddAsync(It.IsAny<IBook>())).ReturnsAsync(ResponseStatus.Success);

            mockBookRepository.Setup(r => r.DeleteByIdAsync(It.IsAny<Guid>())).ReturnsAsync(ResponseStatus.Success);

            // Setup UnitOfWork
            Mock<IUnitOfWork> mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.BookRepository).Returns(mockBookRepository.Object);

            // Setup service
            _bookService = new BookService(mockUnitOfWork.Object);
        }

        [Fact]
        [Trait("Category", "GET Book")]
        public async Task BookGetByIdAsync()
        {
            IBook result = await _bookService.FindBookByIdAsync(Guid.Empty);

            Assert.Equal(_books.First(), result);
        }

        [Fact]
        [Trait("Category", "GET Books")]
        public async Task BookGetAsync()
        {
            IEnumerable<IBook> result = await _bookService.GetBooksAsync();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_books, result);
        }

        [Fact]
        [Trait("Category", "GET Books")]
        public async Task BookGetFilteredAsync()
        {
            IPagedList<IBook> result = await _bookService.FindBooksAsync("searchString", true, nameof(IBook.Name), 1, 3);

            Assert.NotNull(result);
            Assert.Equal(1, result.TotalItemCount);
            Assert.Equal(1, result.PageCount);
            Assert.Equal(result.First(), result.FirstOrDefault());
        }

        [Fact]
        [Trait("Category", "UPDATE Book")]
        public async Task BookUpdateAsync()
        {
            ResponseStatus status = await _bookService.UpdateBookAsync(null);

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "UPDATE Book")]
        public async Task BookUpdateByIdAsync()
        {
            ResponseStatus status = await _bookService.UpdateBookAsync(Guid.Empty, null);

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "INSERT Book")]
        public async Task BookInsertAsync()
        {
            ResponseStatus status = await _bookService.AddBookAsync(null);

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "DELETE Book")]
        public async Task BookDeleteAsync()
        {
            ResponseStatus status = await _bookService.DeleteBookAsync(Guid.Empty);

            Assert.Equal(ResponseStatus.Success, status);
        }

        private IList<Book> InitializeBooks()
        {
            return new List<Book>()
            { 
                new Book
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    Name = "Name",
                    Author = "Author",
                    Bookstore = new Bookstore()
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