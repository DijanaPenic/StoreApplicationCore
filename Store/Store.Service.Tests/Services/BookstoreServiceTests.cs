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
    public class BookstoreServiceTests
    {
        private readonly IList<Bookstore> _bookstores;
        private readonly BookstoreService _bookstoreService;

        public BookstoreServiceTests()
        {
            // Setup bookstore test data
            _bookstores = InitializeBookstores();

            //Setup mock repository

            Mock<IBookstoreRepository> mockBookstoreRepository = new Mock<IBookstoreRepository>();

            mockBookstoreRepository.Setup(r => r.FindByIdWithProjectionAsync<BookstoreDto>(It.IsAny<Guid>())).ReturnsAsync(_bookstores.First());
            mockBookstoreRepository.Setup(r => r.GetWithProjectionAsync<BookstoreDto>()).ReturnsAsync(_bookstores.AsEnumerable());
            mockBookstoreRepository.Setup(r => r.FindWithProjectionAsync<BookstoreDto>(It.IsAny<Expression<Func<IBookstore, bool>>>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(_bookstores.ToPagedList(1, 1));

            mockBookstoreRepository.Setup(r => r.UpdateAsync(It.IsAny<IBookstore>())).ReturnsAsync(ResponseStatus.Success);
            mockBookstoreRepository.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<IBookstore>())).ReturnsAsync(ResponseStatus.Success);

            mockBookstoreRepository.Setup(r => r.AddAsync(It.IsAny<IBookstore>())).ReturnsAsync(ResponseStatus.Success);

            mockBookstoreRepository.Setup(r => r.DeleteByIdAsync(It.IsAny<Guid>())).ReturnsAsync(ResponseStatus.Success);

            // Setup UnitOfWork
            Mock<IUnitOfWork> mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.BookstoreRepository).Returns(mockBookstoreRepository.Object);

            // Setup service
            _bookstoreService = new BookstoreService(mockUnitOfWork.Object);
        }

        [Fact]
        [Trait("Category", "GET Bookstore")]
        public async Task BookstoreGetByIdAsync()
        {
            IBookstore result = await _bookstoreService.FindBookstoreByIdAsync(Guid.Empty);

            Assert.Equal(_bookstores.First(), result);
        }

        [Fact]
        [Trait("Category", "GET Bookstores")]
        public async Task BookstoreGetAsync()
        {
            IEnumerable<IBookstore> result = await _bookstoreService.GetBookstoresAsync();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_bookstores, result);
        }

        [Fact]
        [Trait("Category", "GET Bookstores")]
        public async Task BookstoreGetFilteredAsync()
        {
            IPagedList<IBookstore> result = await _bookstoreService.FindBookstoresAsync("searchString", true, nameof(IBookstore.Name), 1, 3);

            Assert.NotNull(result);
            Assert.Equal(1, result.TotalItemCount);
            Assert.Equal(1, result.PageCount);
            Assert.Equal(result.First(), result.FirstOrDefault());
        }

        [Fact]
        [Trait("Category", "UPDATE Bookstore")]
        public async Task BookstoreUpdateAsync()
        {
            ResponseStatus status = await _bookstoreService.UpdateBookstoreAsync(null);

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "UPDATE Bookstore")]
        public async Task BookstoreUpdateByIdAsync()
        {
            ResponseStatus status = await _bookstoreService.UpdateBookstoreAsync(Guid.Empty, null);

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "INSERT Bookstore")]
        public async Task BookstoreInsertAsync()
        {
            ResponseStatus status = await _bookstoreService.InsertBookstoreAsync(null);

            Assert.Equal(ResponseStatus.Success, status);
        }

        [Fact]
        [Trait("Category", "DELETE Bookstore")]
        public async Task BookstoreDeleteAsync()
        {
            ResponseStatus status = await _bookstoreService.DeleteBookstoreAsync(Guid.Empty);

            Assert.Equal(ResponseStatus.Success, status);
        }

        private IList<Bookstore> InitializeBookstores()
        {
            return new List<Bookstore>()
            {
                new Bookstore
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    Name = "Name",
                    Location = "Location"
                }
            };
        }
    }
}