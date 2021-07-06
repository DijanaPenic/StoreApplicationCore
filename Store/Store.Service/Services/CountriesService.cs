using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

using Store.Model.Common.Models;
using Store.Service.Serialization;
using Store.Service.Common.Services;

namespace Store.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly string _countriesUrl;
        private HttpClient _httpClient;

        private HttpClient HttpClient => _httpClient ??= new HttpClient();

        public CountriesService(IConfiguration configuration)
        {
            _countriesUrl = configuration.GetSection("RESTCountriesURL").Value;
        }

        public async Task<IList<ICountry>> GetCountriesAsync()
        {
            try
            {
                HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Get, $"{_countriesUrl}all");
                HttpResponseMessage responseMsg = await HttpClient.SendAsync(requestMsg);

                string jsonResponse = await responseMsg.Content.ReadAsStringAsync();
                IList<ICountry> objResponse = JsonSerializer.Deserialize<IList<ICountry>>(jsonResponse, JsonSerializerInit.GetJsonSerializerOptions());

                return objResponse;
            }
            catch
            {
                return default;
            }
        }

        public async Task<ICountry> GetCountryByIsoCodeAsync(string code)
        {
            try
            {
                HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Get, $"{_countriesUrl}alpha/{code}");
                HttpResponseMessage responseMsg = await HttpClient.SendAsync(requestMsg);

                string jsonResponse = await responseMsg.Content.ReadAsStringAsync();
                ICountry objResponse = JsonSerializer.Deserialize<ICountry>(jsonResponse, JsonSerializerInit.GetJsonSerializerOptions());

                return objResponse;
            }
            catch
            {
                return default;
            }
        }
    }
}