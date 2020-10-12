using System.Threading.Tasks;
using System.Collections.Generic;

namespace Store.Repository.Common.Core.Dapper
{
    public interface IDapperGenericRepository<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAsync();

        Task<TEntity> FindByKeyAsync(TKey key);

        Task AddAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteByKeyAsync(TKey key);
    }
}