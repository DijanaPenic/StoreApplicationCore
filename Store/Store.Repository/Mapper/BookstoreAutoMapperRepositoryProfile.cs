using AutoMapper;

using Store.Models;
using Store.Model.Common.Models;
using Store.Entities;

namespace Store.Repository.Mapper
{
    public class BookstoreAutoMapperRepositoryProfile : Profile
    {
        public BookstoreAutoMapperRepositoryProfile()
        {
            CreateMap<IBookstore, BookstoreEntity>()
                .ForMember(dst => dst.Books, opt => opt.Ignore())
                .ReverseMap();

            // Need DTO objects because AutoMapper projection doesn't work for interface destinations
            CreateMap<BookstoreEntity, BookstoreExtendedDto>()
                    .ForMember(dst => dst.Books, opt => opt.ExplicitExpansion())
                    .ForMember(dst => dst.BooksCount, opt => opt.MapFrom(src => src.Books.Count));
        }
    }
}