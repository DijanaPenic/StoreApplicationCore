using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Common.Parameters;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models;
using Store.Repository.Common.Core;
using Store.Service.Common.Services;

namespace Store.Services
{
    internal class GlobalSearchService : ParametersService, IGlobalSearchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GlobalSearchService
        (
            IUnitOfWork unitOfWork,
            IQueryUtilityFacade queryUtilityFacade
        ) : base (queryUtilityFacade)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<ISearchItem>> FindAsync(IGlobalFilteringParameters filtering)
        {
            return _unitOfWork.GlobalSearchRepository.FindAsync(filtering);
        }
    }
}