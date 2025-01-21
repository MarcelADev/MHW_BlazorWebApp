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
    }
}
