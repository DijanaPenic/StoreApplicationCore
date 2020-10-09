using System;
using System.Collections.Generic;

namespace Store.Repository.Common.Core.Dapper
{
    public interface IDapperGenericRepository<TEntity, TKey> where TEntity : class
    {
        IEnumerable<TEntity> Get();

        TEntity FindByKey(TKey key);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void DeleteByKey(TKey key);
    }
}