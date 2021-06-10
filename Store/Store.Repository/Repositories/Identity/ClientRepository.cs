using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using Store.DAL.Context;
using Store.Model.Common.Models.Identity;
using Store.Repository.Core;
using Store.Repository.Common.Repositories.Identity;
using Store.Entities.Identity;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;

namespace Store.Repositories.Identity
{
    internal class ClientRepository : GenericRepository, IClientRepository
    {
        private DbSet<ClientEntity> _dbSet => DbContext.Set<ClientEntity>();

        public ClientRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        { 
        }

        public Task<IEnumerable<IClient>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IClient, ClientEntity>(sorting, options);
        }

        public Task<IClient> FindByKeyAsync(Guid key, IOptionsParameters options = null)
        {
            return FindByKeyAsync<IClient, ClientEntity>(options, key);
        }

        public async Task<IClient> FindByNameAsync(string name)
        {
            ClientEntity entity = await _dbSet.FirstOrDefaultAsync(c => c.Name == name);

            return Mapper.Map<IClient>(entity);
        }
    }
}