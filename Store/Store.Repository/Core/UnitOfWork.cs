using AutoMapper;
using System.Threading.Tasks;

using Store.DAL.Context;
using Store.Common.Enums;
using Store.Repositories;
using Store.Repository.Common.Repositories;

namespace Store.Repository.Core
{
    class UnitOfWork
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

        public async Task<ResponseStatus> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0 ? ResponseStatus.Success : ResponseStatus.Error;
        }

        #endregion
    }
}