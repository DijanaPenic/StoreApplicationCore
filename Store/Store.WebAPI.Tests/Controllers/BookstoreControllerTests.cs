using Moq;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Store.Models;
using Store.Models.Api;
using Store.Models.Api.Bookstore;
using Store.Model.Common.Models;
using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Cache.Common;
using Store.WebAPI.Controllers;
using Store.WebAPI.Mapper.Profiles;
using Store.Service.Common.Services;

namespace Store.WebAPI.Tests.Controllers
{
    public class BookstoreControllerTests
    {
        private readonly IList<Bookstore> _bookstores;
        private readonly BookstoreController _bookstoreController;

        public BookstoreControllerTests()
        {
            // Setup bookstore test data
            _bookstores = InitializeBookstores();
            IBookstore firstBookstore = _bookstores.First();

            // Setup service mock
            Mock<IBookstoreService> mockBookstoreService = new Mock<IBookstoreService>();

            mockBookstoreService.Setup(s => s.FindBookstoreByIdAsync(firstBookstore.Id, null)).ReturnsAsync(firstBookstore);
            mockBookstoreService.Setup(s => s.FindBookstoresAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), null)).ReturnsAsync(_bookstores.ToPagedList(1, 1));

            mockBookstoreService.Setup(s => s.UpdateBookstoreAsync(firstBookstore.Id, firstBookstore)).ReturnsAsync(ResponseStatus.Success);
            mockBookstoreService.Setup(s => s.UpdateBookstoreAsync(It.Is<Guid>(id => id != firstBookstore.Id), It.IsAny<IBookstore>())).ReturnsAsync(ResponseStatus.NotFound);
            mockBookstoreService.Setup(s => s.UpdateBookstoreAsync(It.IsAny<Guid>(), null)).ReturnsAsync(ResponseStatus.Error);

            mockBookstoreService.Setup(s => s.DeleteBookstoreAsync(firstBookstore.Id)).ReturnsAsync(ResponseStatus.Success);
            mockBookstoreService.Setup(s => s.DeleteBookstoreAsync(It.Is<Guid>(id => id != firstBookstore.Id))).ReturnsAsync(ResponseStatus.NotFound);

            // Setup mapper
            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperWebApiProfile>();
            });
            IMapper mapper = mapperConfiguration.CreateMapper();

            // Setup cache manager
            Mock<ICacheProvider> cacheProvider = new Mock<ICacheProvider>();
            cacheProvider.Setup(cp => cp.Remove(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            Mock<ICacheManager> cacheManager = new Mock<ICacheManager>();
            cacheManager.Setup(cm => cm.CacheProvider).Returns(cacheProvider.Object);

            // Setup controller
            _bookstoreController = new BookstoreController(mockBookstoreService.Object, mapper, cacheManager.Object);
        }

        [Fact]
        [Trait("Category", "GET Bookstore")]
        public async Task BookstoreGetByIdBadRequestAsync()
        {
            StatusCodeResult response = await _bookstoreController.GetAsync(Guid.Empty, null) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "GET Bookstore")]
        public async Task BookstoreGetByIdNotFoundAsync()
        {
            StatusCodeResult response = await _bookstoreController.GetAsync(GuidHelper.NewSequentialGuid(), null) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "GET Bookstore")]
        public async Task BookstoreGetByIdSuccessAsync()
        {
            IBookstore expectedBookstore = _bookstores.First();

            ObjectResult response = await _bookstoreController.GetAsync(expectedBookstore.Id, null) as ObjectResult;
            BookstoreGetApiModel responseBookstore = response.Value as BookstoreGetApiModel;

            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal("Name", responseBookstore.Name);
            Assert.Equal("Location", responseBookstore.Location);
            Assert.Equal(expectedBookstore.Id, responseBookstore.Id);
        }

        [Fact]
        [Trait("Category", "GET Bookstores")]
        public async Task BookstoreGetFilteredSuccessAsync()
        {
            ObjectResult response = await _bookstoreController.GetAsync(null, null) as ObjectResult;
            PagedResponse<BookstoreGetApiModel> responseBookstores = response.Value as PagedResponse<BookstoreGetApiModel>;

            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

            // Check Items
            Assert.NotNull(responseBookstores.Items);
            Assert.Single(responseBookstores.Items);

            BookstoreGetApiModel responseFirstBookstore = responseBookstores.Items.FirstOrDefault();
            IBookstore expectedBookstore = _bookstores.First();

            Assert.Equal(expectedBookstore.Name, responseFirstBookstore.Name);
            Assert.Equal(expectedBookstore.Location, responseFirstBookstore.Location);
            Assert.Equal(expectedBookstore.Id, responseFirstBookstore.Id);

            // Check MetaData
            Assert.NotNull(responseBookstores.MetaData);
            Assert.Equal(1, responseBookstores.MetaData.TotalItemCount);
        }

        [Fact]
        [Trait("Category", "DELETE Bookstore")]
        public async Task BookstoreDeleteByIdBadRequestAsync()
        {
            StatusCodeResult response = await _bookstoreController.DeleteAsync(Guid.Empty) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "DELETE Bookstore")]
        public async Task BookstoreDeleteByIdNotFoundAsync()
        {
            StatusCodeResult response = await _bookstoreController.DeleteAsync(GuidHelper.NewSequentialGuid()) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "DELETE Bookstore")]
        public async Task BookstoreDeleteByIdSuccessAsync()
        {
            StatusCodeResult response = await _bookstoreController.DeleteAsync(_bookstores.First().Id) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "PATCH Bookstore")]
        public async Task BookstorePatchBadRequestAsync()
        {
            StatusCodeResult response = await _bookstoreController.PatchAsync(Guid.Empty, new BookstorePatchApiModel()) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "PATCH Bookstore")]
        public async Task BookstorePatchNotFoundAsync()
        {
            StatusCodeResult response = await _bookstoreController.PatchAsync(GuidHelper.NewSequentialGuid(), new BookstorePatchApiModel()) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "PATCH Bookstore")]
        public async Task BookstorePatchSuccessAsync()
        {
            StatusCodeResult response = await _bookstoreController.PatchAsync(_bookstores.First().Id, new BookstorePatchApiModel()) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "PATCH Bookstore")]
        public async Task BookstorePatchErrorAsync()
        {
            StatusCodeResult response = await _bookstoreController.PatchAsync(GuidHelper.NewSequentialGuid(), null) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "POST Bookstore")]
        public async Task BookstorePostBadRequestAsync()
        {
            InvalidateModelState();

            StatusCodeResult response = await _bookstoreController.PostAsync(null) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "POST Bookstore")]
        public async Task BookstorePostSuccessAsync()
        {
            StatusCodeResult response = await _bookstoreController.PostAsync(new BookstoreApiPostModel()) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
        }

        private void InvalidateModelState()
        {
            _bookstoreController.ModelState.AddModelError("mockErrorKey", "mockErrorMessage");
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