using System.Threading.Tasks;

using Store.Common.Enums;
using Store.Repository.Common.Repositories;

namespace Store.Repository.Common.Core
{
    public interface IUnitOfWork
    {
        #region Properties

        IBookRepository BookRepository { get; }
        IBookstoreRepository BookstoreRepository { get; }

        #endregion

        #region Methods

        Task<ResponseStatus> SaveChangesAsync(ResponseStatus status);

        #endregion
    }
}