using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using X.PagedList;

using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Model.Common.Models.Core;

namespace Store.Repository.Common.Core
{
    public interface IGenericRepository<TDomain> where TDomain : class, IPoco
    {
        Task<IEnumerable<TDomain>> GetAsync(IOptionsParameters options);

        // TODO - remove
        Task<IEnumerable<TDomain>> GetAsync(params string[] includeProperties);

        Task<IEnumerable<TDomain>> GetWithProjectionAsync<TDestination>(IOptionsParameters options);

        Task<IPagedList<TDomain>> FindAsync(Expression<Func<TDomain, bool>> filterExpression, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);

        // TODO - remove
        Task<IPagedList<TDomain>> FindAsync(Expression<Func<TDomain, bool>> filterExpression, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize, params string[] includeProperties);

        Task<IPagedList<TDomain>> FindWithProjectionAsync<TDestination>(Expression<Func<TDomain, bool>> filterExpression, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);

        Task<IEnumerable<TDomain>> FindAsync(Expression<Func<TDomain, bool>> filterExpression, bool isDescendingSortOrder, string sortOrderProperty, params string[] includeProperties);

        Task<IEnumerable<TDomain>> FindWithProjectionAsync<TDestination>(Expression<Func<TDomain, bool>> filterExpression, bool isDescendingSortOrder, string sortOrderProperty, params string[] includeProperties);

        Task<TDomain> FindByIdAsync(Guid id, IOptionsParameters options);

        // TODO - remove
        Task<TDomain> FindByIdAsync(Guid id, params string[] includeProperties);

        Task<TDomain> FindByIdWithProjectionAsync<TDestination>(Guid id, IOptionsParameters options) where TDestination : IPoco;

        Task<ResponseStatus> AddAsync(TDomain model);

        Task<ResponseStatus> DeleteByIdAsync(Guid id);

        Task<ResponseStatus> UpdateAsync(Guid id, TDomain model);

        Task<ResponseStatus> UpdateAsync(TDomain model);
    }
}