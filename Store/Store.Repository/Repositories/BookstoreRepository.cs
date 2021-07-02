using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LinqKit;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.Models;
using Store.Model.Common.Models;
using Store.Entities;
using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Repository.Core;
using Store.Repository.Extensions;
using Store.Repository.Common.Repositories;

namespace Store.Repositories
{
    internal class BookstoreRepository : GenericRepository, IBookstoreRepository
    {
        public BookstoreRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task<IPagedList<BookstoreExtendedDto>> FindExtendedAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            ExpressionStarter<BookstoreExtendedDto> predicate = PredicateBuilder.New<BookstoreExtendedDto>(true);

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                predicate.And(b => b.Name.Contains(filter.SearchString) || b.Location.Contains(filter.SearchString));
            }

            return FindWithProjectionAsync<BookstoreExtendedDto, BookstoreEntity>(predicate, paging, sorting, options);
        }

        public Task<IEnumerable<IBookstore>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IBookstore, BookstoreEntity>(sorting, options);
        }

        public Task<IEnumerable<BookstoreExtendedDto>> GetExtendedAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetUsingProjectionAsync<BookstoreExtendedDto, BookstoreEntity>(sorting, options);
        }

        public Task<IBookstore> FindByKeyAsync(Guid key, IOptionsParameters options = null)
        {
            return FindByKeyAsync<IBookstore, BookstoreEntity>(options, key);
        }

        public Task<BookstoreExtendedDto> FindExtendedByKeyAsync(Guid key, IOptionsParameters options = null)
        {
            return DbContext.Bookstores.ProjectTo<BookstoreEntity, BookstoreExtendedDto>(Mapper, OptionsMap<BookstoreEntity, BookstoreExtendedDto>(options))
                         .FirstOrDefaultAsync(b => b.Id == key);
        }

        public Task<ResponseStatus> UpdateAsync(IBookstore model)
        {
            return UpdateAsync<IBookstore, BookstoreEntity>(model, model.Id);
        }

        public Task<ResponseStatus> AddAsync(IBookstore model)
        {
            return AddAsync<IBookstore, BookstoreEntity>(model);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(Guid key)
        {
            return DeleteByKeyAsync<BookstoreEntity>(key);
        }
    }
}