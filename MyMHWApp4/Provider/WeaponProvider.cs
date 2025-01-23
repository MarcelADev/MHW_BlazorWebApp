using MHWApp.Library.Model;
using System.Net.Http.Json;
using System.Net.Http;

namespace MyMHWApp4.Provider
{
    public class WeaponProvider(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

        public async Task<List<WeaponType>> GetWeaponTypesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<WeaponType>>($"weapontypes");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
