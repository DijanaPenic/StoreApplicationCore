using AutoMapper;
using System.Threading.Tasks;

using Store.DAL.Context;
using Store.Common.Enums;
using Store.Repositories;
using Store.Repository.Common.Core;
using Store.Repository.Common.Repositories;

namespace Store.Repository.Core
{
    internal class UnitOfWork : IUnitOfWork
    {
        #region Fields

        private readonly StoreDbContext _context;
        private readonly IMapper _mapper;

        private IBookRepository _bookRepository;

        #endregion


        #region Constructors

        public UnitOfWork(StoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #endregion

        #region IUnitOfWork Members

        public IBookRepository BookRepository => _bookRepository ?? (_bookRepository = new BookRepository(_context, _mapper));

        public async Task<ResponseStatus> SaveChangesAsync(ResponseStatus currentStatus)
        {
            // Don't save changes to the context if the current status is indicating an erroneous state (example: Error, NotFound).
            if (currentStatus != ResponseStatus.Success)
                return currentStatus;

            return await _context.SaveChangesAsync() > 0 ? ResponseStatus.Success : ResponseStatus.Error;
        }

        #endregion
    }
}