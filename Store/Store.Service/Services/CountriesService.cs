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
        private readonly string _countriesURL;
        private HttpClient _httpClient = null;

        public HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }

                return _httpClient;
            }
            private set
            {
                _httpClient = value;
            }
        }

        public CountriesService(IConfiguration configuration)
        {
            _countriesURL = configuration.GetSection("RESTCountriesURL").Value;
        }

        public async Task<IList<ICountry>> GetCountriesAsync()
        {
            try
            {
                HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Get, $"{_countriesURL}all");
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
                HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Get, $"{_countriesURL}alpha/{code}");
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