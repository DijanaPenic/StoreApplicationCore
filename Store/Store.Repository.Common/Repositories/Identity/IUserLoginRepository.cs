using System;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserLoginRepository : IDapperGenericRepository<IUserLogin, IUserLoginKey>
    {
        IEnumerable<IUserLogin> FindByUserId(Guid userId);
    }
}