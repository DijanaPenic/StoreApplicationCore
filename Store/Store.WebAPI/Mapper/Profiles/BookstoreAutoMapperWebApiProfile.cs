using AutoMapper;

using Store.Models;
using Store.Model.Common.Models;
using Store.WebAPI.Models.Book;
using Store.WebAPI.Models.Bookstore;

namespace Store.WebAPI.Mapper.Profiles
{
    public class BookstoreAutoMapperWebApiProfile : Profile
    {
        public BookstoreAutoMapperWebApiProfile()
        {
            CreateMap<BookstoreGetApiModel, BookstoreExtendedDto>().ReverseMap();
            CreateMap<BookstorePostApiModel, IBookstore>().ReverseMap();
            CreateMap<BookstorePatchApiModel, IBookstore>().ReverseMap();
            CreateMap<BookstoreApiModel, IBookstore>().ReverseMap();
        }
    }
}