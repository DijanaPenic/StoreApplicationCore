using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.DAL.Context;
using Store.DAL.Schema.Identity;
using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Repository.Core;
using Store.Entities.Identity;
using Store.Repository.Common.Repositories.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.Repositories.Identity
{
    internal class UserRefreshTokenRepository : GenericRepository, IUserRefreshTokenRepository
    {
        public UserRefreshTokenRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        { 
        }

        public Task<IEnumerable<IUserRefreshToken>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IUserRefreshToken, UserRefreshTokenEntity>(sorting, options);
        }

        public Task<IPagedList<IUserRefreshToken>> FindAsync(IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            return FindAsync<IUserRefreshToken, UserRefreshTokenEntity>(null, paging, sorting, options);
        }

        public Task<IUserRefreshToken> FindByKeyAsync(IUserRefreshTokenKey key, IOptionsParameters options = null)
        {
            return FindByKeyAsync<IUserRefreshToken, UserRefreshTokenEntity>(options, key.ToArray());
        }

        public Task<ResponseStatus> AddAsync(IUserRefreshToken model)
        {
            return AddAsync<IUserRefreshToken, UserRefreshTokenEntity>(model);
        }

        public Task<ResponseStatus> UpdateAsync(IUserRefreshToken model)
        {
            return UpdateAsync<IUserRefreshToken, UserRefreshTokenEntity>(model, model.UserId, model.ClientId);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(IUserRefreshTokenKey key)
        {
            return DeleteByKeyAsync<UserRefreshTokenEntity>(key.ToArray());
        }

        public Task DeleteExpiredAsync()
        {
            DateTime now = DateTime.UtcNow;

            return ExecuteQueryAsync(
                sql: $@"
                    DELETE FROM {UserRefreshTokenSchema.Table}
                    WHERE {UserRefreshTokenSchema.Columns.DateExpiresUtc} < @{nameof(now)}",
                param: new { now }  // Datetime must be sent as dynamic object (instead of parameter: "The JIT compiler encountered invalid IL code or an internal limitation"). 
            );
        }
        public async Task<IUserRefreshToken> FindByValueAsync(string value)
        {
            UserRefreshTokenEntity entity = await DbContext.UserRefreshTokens.Where(urt => urt.Value == value).SingleOrDefaultAsync();

            return Mapper.Map<IUserRefreshToken>(entity);
        }
    }
}