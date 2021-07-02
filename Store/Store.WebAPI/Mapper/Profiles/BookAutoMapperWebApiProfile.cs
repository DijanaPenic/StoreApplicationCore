using AutoMapper;

using Store.Models;
using Store.Model.Common.Models;
using Store.WebAPI.Models.Book;

namespace Store.WebAPI.Mapper.Profiles
{
    public class BookAutoMapperWebApiProfile : Profile
    {
        public BookAutoMapperWebApiProfile()
        {
            CreateMap<BookGetApiModel, BookDto>().ReverseMap();
            CreateMap<BookGetApiModel, IBook>().ReverseMap();
            CreateMap<BookPostApiModel, IBook>().ReverseMap();
            CreateMap<BookPatchApiModel, IBook>().ReverseMap();
        }
    }
}