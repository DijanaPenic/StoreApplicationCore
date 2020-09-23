using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using X.PagedList;

using Store.Common.Enums;
using Store.Model.Common.Models.Core;

namespace Store.Repository.Common.Core
{
    public interface IGenericRepository<TDomain> where TDomain : class, IPoco
    {
        Task<IEnumerable<TDomain>> GetAsync(params string[] includeProperties);

        Task<IPagedList<TDomain>> FindAsync(Expression<Func<TDomain, bool>> filterExpression, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, params string[] includeProperties);

        Task<IEnumerable<TDomain>> FindAsync(Expression<Func<TDomain, bool>> filterExpression, string sortOrderProperty, bool isDescendingSortOrder, params string[] includeProperties);

        Task<TDomain> FindByIdAsync(Guid id, params string[] includeProperties);

        Task<ResponseStatus> AddAsync(TDomain model);

        Task<ResponseStatus> DeleteByIdAsync(Guid id);

        Task<ResponseStatus> UpdateAsync(TDomain model, Guid id);
    }
}