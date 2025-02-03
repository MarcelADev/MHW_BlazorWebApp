using MHWApp.Library.DTO;
using MHWApp.Library.Model;
using System.Net.Http.Json;
using System.Net.Http;

namespace MyMHWApp4.Provider
{
    public class GameExtensionProvider(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

        public async Task<List<GameExtension>> GetGameExtensionsAsync()
        {
            try
            {
                var gameExtensions = await _httpClient.GetFromJsonAsync<List<GameExtension>>($"gameextensions");

                if (gameExtensions != null) return gameExtensions;

                return new List<GameExtension>();
            }
            catch (Exception ex)
            {
                return new List<GameExtension>();
            }
        }

        public async Task<bool> CreateGameExtensionAsync(GameExtension gameExtension)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("gameextensions", gameExtension);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
