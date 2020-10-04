using AutoMapper;
using System.Threading.Tasks;

using Store.DAL.Context;
using Store.Common.Enums;
using Store.Repositories;
using Store.Repository.Common.Core;
using Store.Repository.Common.Repositories;

namespace Store.Repository.Core
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly StoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public IBookRepository BookRepository => new BookRepository(_dbContext, _mapper);
        public IBookstoreRepository BookstoreRepository => new BookstoreRepository(_dbContext, _mapper);

        public UnitOfWork(StoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ResponseStatus> SaveChangesAsync(ResponseStatus currentStatus)
        {
            // Don't save changes to the context if the current status is indicating an erroneous state (example: Error, NotFound).
            if (currentStatus != ResponseStatus.Success)
                return currentStatus;

            return await _dbContext.SaveChangesAsync() > 0 ? ResponseStatus.Success : ResponseStatus.Error;
        }
    }
}