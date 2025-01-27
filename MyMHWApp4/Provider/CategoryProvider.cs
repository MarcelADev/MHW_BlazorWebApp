using MHWApp.Library.Model;
using System.Net.Http.Json;
using System.Net.Http;

namespace MyMHWApp4.Provider
{
    public class CategoryProvider(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Category>>($"categories");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Category>> GetInventoryEntryCategoriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Category>>($"categories/inventoryentries");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Category>> GetWeaponCategoriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Category>>($"categories/weapons");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Category>> GetArmorCategoriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Category>>($"categories/armor");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
