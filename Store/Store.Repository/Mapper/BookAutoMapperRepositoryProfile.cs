using AutoMapper;

using Store.Models;
using Store.Model.Common.Models;
using Store.Entities;

namespace Store.Repository.Mapper
{
    public class BookAutoMapperRepositoryProfile : Profile
    {
        public BookAutoMapperRepositoryProfile()
        {
            CreateMap<IBook, BookEntity>()
                .ForMember(dst => dst.Bookstore, opt => opt.Ignore())
                .ReverseMap();

            // Need DTO objects because AutoMapper projection doesn't work for interface destinations
            CreateMap<BookEntity, BookDto>();
        }
    }
}