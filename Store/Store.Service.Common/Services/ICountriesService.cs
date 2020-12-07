using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models;

namespace Store.Service.Common.Services
{
    public interface ICountriesService
    {
        Task<IList<ICountry>> GetCountriesAsync();

        Task<ICountry> GetCountryByIsoCodeAsync(string code);
    }
}