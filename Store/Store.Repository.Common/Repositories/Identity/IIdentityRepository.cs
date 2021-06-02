using System.Threading.Tasks;
using System.Collections.Generic;

namespace Store.Repository.Common
{
    public interface IIdentityRepository<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAsync();

        Task<TEntity> FindByKeyAsync(TKey key);

        Task AddAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteByKeyAsync(TKey key);
    }
}