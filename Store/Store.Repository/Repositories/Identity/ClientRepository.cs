﻿using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.DAL.Schema.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repositories.Identity
{
    internal class ClientRepository : DapperRepositoryBase, IClientRepository
    {
        public ClientRepository(IDbTransaction transaction) : base(transaction)
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