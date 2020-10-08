using AutoMapper;

using Store.Entities;
using Store.DAL.Context;
using Store.Repository.Core;
using Store.Repository.Common.Repositories;
using Store.Model.Common.Models;

namespace Store.Repositories
{
    public class BookstoreRepository : GenericRepository<BookstoreEntity, IBookstore>, IBookstoreRepository
    {
        public BookstoreRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}