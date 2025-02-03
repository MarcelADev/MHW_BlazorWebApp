using MHWApp.Library.Model;
using System.Net.Http.Json;

namespace MyMHWApp4.Provider
{
    public class PlayerProvider(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

        public async Task<List<Player>> GetPlayersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Player>>($"players");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
