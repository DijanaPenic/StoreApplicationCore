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
using Store.Models.Api.Book;
using Store.Model.Common.Models;
using Store.Common.Enums;
using Store.Common.Helpers;
using Store.WebAPI.Controllers;
using Store.WebAPI.Mapper.Profiles;
using Store.Service.Common.Services;

namespace Store.WebAPI.Tests.Controllers
{
    public class BookControllerTests
    {
        private readonly IList<Book> _books;
        private readonly BookController _bookController;

        public BookControllerTests()
        {
            // Setup book test data
            _books = InitializeBooks();
            IBook firstBook = _books.First();

            // Setup mock service
            Mock<IBookService> mockBookService = new Mock<IBookService>();

            mockBookService.Setup(s => s.FindBookByIdAsync(firstBook.Id, null)).ReturnsAsync(firstBook);
            mockBookService.Setup(s => s.FindBooksAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), null)).ReturnsAsync(_books.ToPagedList(1, 1));

            mockBookService.Setup(s => s.UpdateBookAsync(firstBook.Id, firstBook)).ReturnsAsync(ResponseStatus.Success);
            mockBookService.Setup(s => s.UpdateBookAsync(It.Is<Guid>(id => id != firstBook.Id), It.IsAny<IBook>())).ReturnsAsync(ResponseStatus.NotFound);
            mockBookService.Setup(s => s.UpdateBookAsync(It.IsAny<Guid>(), null)).ReturnsAsync(ResponseStatus.Error);

            mockBookService.Setup(s => s.DeleteBookAsync(firstBook.Id)).ReturnsAsync(ResponseStatus.Success);
            mockBookService.Setup(s => s.DeleteBookAsync(It.Is<Guid>(id => id != firstBook.Id))).ReturnsAsync(ResponseStatus.NotFound);

            // Setup mapper
            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperWebApiProfile>();
            });
            IMapper mapper = mapperConfiguration.CreateMapper();

            // Setup controller
            _bookController = new BookController(mockBookService.Object, mapper);
        }

        [Fact]
        [Trait("Category", "GET Book")]
        public async Task BookGetByIdBadRequestAsync()
        {
            StatusCodeResult response = await _bookController.GetAsync(Guid.Empty, null) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "GET Book")]
        public async Task BookGetByIdNotFoundAsync()
        {
            StatusCodeResult response = await _bookController.GetAsync(GuidHelper.NewSequentialGuid(), null) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "GET Book")]
        public async Task BookGetByIdSuccessAsync()
        {
            IBook expectedBook = _books.First();

            ObjectResult response = await _bookController.GetAsync(expectedBook.Id, null) as ObjectResult;
            BookGetApiModel responseBook = response.Value as BookGetApiModel;

            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(expectedBook.Name, responseBook.Name);
            Assert.Equal(expectedBook.Author, responseBook.Author);
            Assert.Equal(expectedBook.Bookstore.Name, responseBook.Bookstore.Name);
            Assert.Equal(expectedBook.Bookstore.Location, responseBook.Bookstore.Location);
            Assert.Equal(expectedBook.Id, responseBook.Id);
        }

        [Fact]
        [Trait("Category", "GET Books")]
        public async Task BookGetFilteredSuccessAsync()
        {
            ObjectResult response = await _bookController.GetAsync(null) as ObjectResult;
            PagedApiResponse<BookGetApiModel> responseBooks = response.Value as PagedApiResponse<BookGetApiModel>;

            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

            // Check Items
            Assert.NotNull(responseBooks.Items);
            Assert.Single(responseBooks.Items);

            BookGetApiModel responseFirstBook = responseBooks.Items.FirstOrDefault();
            IBook expectedBook = _books.First();

            Assert.Equal(expectedBook.Name, responseFirstBook.Name);
            Assert.Equal(expectedBook.Author, responseFirstBook.Author);
            Assert.Equal(expectedBook.Bookstore.Name, responseFirstBook.Bookstore.Name);
            Assert.Equal(expectedBook.Bookstore.Location, responseFirstBook.Bookstore.Location);
            Assert.Equal(expectedBook.Id, responseFirstBook.Id);

            // Check MetaData
            Assert.NotNull(responseBooks.MetaData);
            Assert.Equal(1, responseBooks.MetaData.TotalItemCount);
        }

        [Fact]
        [Trait("Category", "DELETE Book")]
        public async Task BookDeleteByIdBadRequestAsync()
        {
            StatusCodeResult response = await _bookController.DeleteAsync(Guid.Empty) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "DELETE Book")]
        public async Task BookDeleteByIdNotFoundAsync()
        {
            StatusCodeResult response = await _bookController.DeleteAsync(GuidHelper.NewSequentialGuid()) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "DELETE Book")]
        public async Task BookDeleteByIdSuccessAsync()
        {
            StatusCodeResult response = await _bookController.DeleteAsync(_books.First().Id) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "PATCH Book")]
        public async Task BookPatchBadRequestAsync()
        {
            StatusCodeResult response = await _bookController.PatchAsync(Guid.Empty, new BookPatchApiModel()) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "PATCH Book")]
        public async Task BookPatchNotFoundAsync()
        {
            StatusCodeResult response = await _bookController.PatchAsync(GuidHelper.NewSequentialGuid(), new BookPatchApiModel()) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "PATCH Book")]
        public async Task BookPatchSuccessAsync()
        {
            StatusCodeResult response = await _bookController.PatchAsync(_books.First().Id, new BookPatchApiModel()) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "PATCH Book")]
        public async Task BookPatchErrorAsync()
        {
            StatusCodeResult response = await _bookController.PatchAsync(GuidHelper.NewSequentialGuid(), null) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "POST Book")]
        public async Task BookPostBadRequestAsync()
        {
            InvalidateModelState();

            StatusCodeResult response = await _bookController.PostAsync(null) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "POST Book")]
        public async Task BookPostSuccessAsync()
        {
            StatusCodeResult response = await _bookController.PostAsync(new BookPostApiModel()) as StatusCodeResult;

            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
        }

        private void InvalidateModelState()
        {
            _bookController.ModelState.AddModelError("mockErrorKey", "mockErrorMessage");
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