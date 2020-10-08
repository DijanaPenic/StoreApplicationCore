using System;
using System.Collections.Generic;

namespace Store.Repository.Common.Core.Dapper
{
    public interface IDapperGenericRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Get();

        TEntity FindById(Guid id);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void DeleteById(Guid id);
    }
}