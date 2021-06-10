using System;

using Store.Model.Common.Models;

namespace Store.Repository.Common.Repositories
{
    public interface IBookstoreRepository : IRepository<IBookstore, Guid>
    {
    }
}