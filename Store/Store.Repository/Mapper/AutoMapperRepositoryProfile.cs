﻿using AutoMapper;

using Store.Models;
using Store.Model.Common.Models;
using Store.Entities;

namespace Store.Repository.Mapper
{
    public class AutoMapperRepositoryProfile : Profile
    {
        public AutoMapperRepositoryProfile()
        {
            // Bookstore and Book mappings
            CreateMap<IBookstore, BookstoreEntity>().ReverseMap();
            CreateMap<IBookstore, BookstoreDto>().ReverseMap();

            CreateMap<IBook, BookEntity>().ReverseMap();
            CreateMap<IBook, BookDto>().ReverseMap();

            // Need DTO objects because AutoMapper projection doesn't work for interface destinations
            CreateMap<BookstoreEntity, BookstoreDto>()
                    .ForMember(dst => dst.Books, opt => opt.ExplicitExpansion())
                    .ForMember(dst => dst.BooksCount, opt => opt.MapFrom(src => src.Books.Count))
                    .ReverseMap();
            CreateMap<BookEntity, BookDto>()
                    .ForMember(dst => dst.Bookstore, opt => opt.ExplicitExpansion())
                    .ReverseMap();

            // Identity mappings - not needed as Dapper is used for direct mapping to domain models

            // Email Template mappings
            CreateMap<IEmailTemplate, EmailTemplateEntity>().ReverseMap();
        }
    }
}