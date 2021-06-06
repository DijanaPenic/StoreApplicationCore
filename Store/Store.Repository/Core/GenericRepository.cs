using AutoMapper;

using Store.DAL.Context;

namespace Store.Repository.Core
{
    internal abstract partial class GenericRepository
    {
        protected ApplicationDbContext DbContext { get; }

        protected IMapper Mapper { get; }

        public GenericRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            Mapper = mapper;
        }
    }
}