using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using LinqKit;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Extensions;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Repository.Core;
using Store.Repository.Common.Repositories.Identity;
using Store.Model.Common.Models.Identity;
using Store.Entities.Identity;
using Store.Repository.Extensions;

namespace Store.Repositories.Identity
{
    internal class RoleRepository : GenericRepository, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        { 
        }

        public Task<IPagedList<IRole>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            ExpressionStarter<IRole> predicate = PredicateBuilder.New<IRole>(true);

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                predicate.And(r => r.Name.Contains(filter.SearchString));
            }

            return FindAsync<IRole, RoleEntity>(predicate, paging, sorting, options);
        }

        public Task<IEnumerable<IRole>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IRole, RoleEntity>(sorting, options);
        }

        public Task<IRole> FindByKeyAsync(Guid key, IOptionsParameters options = null)
        {
            return FindByKeyAsync<IRole, RoleEntity>(options, key);
        }

        public Task<ResponseStatus> UpdateAsync(IRole model)
        {
            return UpdateAsync<IRole, RoleEntity>(model, model.Id);
        }

        public Task<ResponseStatus> AddAsync(IRole model)
        {
            return AddAsync<IRole, RoleEntity>(model);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(Guid key)
        {
            return DeleteByKeyAsync<RoleEntity>(key);
        }

        public async Task<IPagedList<IRole>> FindBySectionAsync(IPermissionFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting)
        {
            string sectionType = $"{filter.SectionType}.";

            IPagedList<RoleEntity> entityPagedList = await DbContext.Roles.Filter(string.IsNullOrEmpty(filter.SearchString) ? null : r => r.Name.Contains(filter.SearchString))
                                                                 .Filter(r => r.Claims.Any(rc => rc.ClaimValue.StartsWith(sectionType)))
                                                                 .Include(r => r.Claims.Where(rc => rc.ClaimValue.StartsWith(sectionType)))
                                                                 .OrderBy(SortingMap<IRole, RoleEntity>(sorting))
                                                                 .ToPagedListAsync(paging.PageNumber, paging.PageSize);

            return entityPagedList.ToPagedList<RoleEntity, IRole>(Mapper);
        }

        public async Task<IRole> FindByNameAsync(string roleName)
        {
            RoleEntity entity = await DbContext.Roles.Where(r => r.NormalizedName == roleName).SingleOrDefaultAsync();

            return Mapper.Map<IRole>(entity);
        }

        public async Task<IEnumerable<IRole>> FindByNameAsync(string[] roleNames)
        {
            IEnumerable<RoleEntity> entities = await DbContext.Roles.Where(r => roleNames.Contains(r.NormalizedName)).ToListAsync();

            return Mapper.Map<IEnumerable<IRole>>(entities);
        }
    }
}