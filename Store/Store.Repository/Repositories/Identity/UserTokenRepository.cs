using LinqKit;
using AutoMapper;
using X.PagedList;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Repository.Core;
using Store.Entities.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repositories.Identity
{
    internal class UserTokenRepository : GenericRepository, IUserTokenRepository
    {
        public UserTokenRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        { 
        }

        public Task<IPagedList<IUserToken>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            ExpressionStarter<IUserToken> predicate = PredicateBuilder.New<IUserToken>(true);

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                predicate.And(ut => ut.Name.Contains(filter.SearchString) || ut.LoginProvider.Contains(filter.SearchString));
            }

            return FindAsync<IUserToken, UserTokenEntity>(predicate, paging, sorting, options);
        }

        public Task<IEnumerable<IUserToken>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IUserToken, UserTokenEntity>(sorting, options);
        }

        public Task<IUserToken> FindByKeyAsync(IUserTokenKey key, IOptionsParameters options = null)
        {
            return FindByKeyAsync<IUserToken, UserTokenEntity>(options, key);
        }

        public Task<ResponseStatus> UpdateAsync(IUserToken model)
        {
            return UpdateAsync<IUserToken, UserTokenEntity>(model, model.UserId, model.LoginProvider, model.Name);
        }

        public Task<ResponseStatus> AddAsync(IUserToken model)
        {
            return AddAsync<IUserToken, UserTokenEntity>(model);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(IUserTokenKey key)
        {
            return DeleteByKeyAsync<IUserToken, UserTokenEntity>(key);
        }
    }
}