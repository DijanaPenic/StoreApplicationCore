using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;

using Store.DAL.Context;
using Store.DAL.Schema.Identity;
using Store.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Core;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repositories.Identity
{
    internal class ClientRepository : GenericRepository, IClientRepository
    {
        public ClientRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        { 
        }

        public async Task<IEnumerable<IClient>> GetAsync()
        {
            return await QueryAsync<Client>(
                sql: $"SELECT * FROM {ClientSchema.Table}"
            );
        }

        public async Task<IClient> FindByKeyAsync(Guid key)
        {
            return await QuerySingleOrDefaultAsync<Client>(
                sql: $"SELECT * FROM {ClientSchema.Table} WHERE {ClientSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public async Task<IClient> FindByNameAsync(string name)
        {
            return await QuerySingleOrDefaultAsync<Client>(
                sql: $"SELECT * FROM {ClientSchema.Table} WHERE {ClientSchema.Columns.Name} = @{nameof(name)}",
                param: new { name }
            );
        }
    }
}