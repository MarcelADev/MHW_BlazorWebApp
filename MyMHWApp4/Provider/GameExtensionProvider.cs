using MHWApp.Library.DTO;
using MHWApp.Library.Model;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using static MHWApp.Library.StaticVariables;

namespace MyMHWApp4.Provider
{
    public class GameExtensionProvider(IHttpClientFactory httpClientFactory, ILogger<GameExtensionProvider> logger)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AuthAndResp");

        public async Task<List<GameExtension>> GetGameExtensionsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "gameextensions");
            var response = await _httpClient.SendAsync(request);
            var gameExtensions = await response.Content.ReadFromJsonAsync<List<GameExtension>>();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                logger.LogInformation(LOG_UNAUTHORIZED);
                return new List<GameExtension>();
            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                logger.LogInformation(nameof(GameExtension) + LOG_GET_MANY_SUCCESS);
                return gameExtensions ?? new List<GameExtension>();
            }

            logger.LogInformation(LOG_UNEXPECTED_HTTP_STATUSCODE);
            return new List<GameExtension>();
        }

        public async Task<int> CreateGameExtensionAsync(GameExtension gameExtension)
        {
            var response = await _httpClient.PostAsJsonAsync("gameextensions", gameExtension);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                logger.LogInformation(LOG_UNAUTHORIZED);
                return -1;
            }
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                logger.LogInformation(LOG_WARNING + nameof(GameExtension) + LOG_CREATE_FAILURE);
                return -1;
            }
            if (response.StatusCode == HttpStatusCode.Created)
            {
                logger.LogInformation(nameof(GameExtension) + LOG_CREATE_SUCCESS);
                var newGameExtension = await response.Content.ReadFromJsonAsync<GameExtension>();
                return newGameExtension?.Id ?? -1;
            }

            logger.LogInformation(LOG_UNEXPECTED_HTTP_STATUSCODE);
            return -1;
        }

        public async Task<bool> UpdateGameExtensionAsync(GameExtension updatedGameExtension)
        {
            var response = await _httpClient.PutAsJsonAsync($"gameextensions/{updatedGameExtension.Id}", updatedGameExtension);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                logger.LogInformation(LOG_UNAUTHORIZED);
                return false;
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation(LOG_WARNING + nameof(GameExtension) + LOG_UPDATE_FAILURE);
                return false;
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                logger.LogInformation(nameof(GameExtension) + LOG_UPDATE_SUCCESS);
                return true;
            }

            logger.LogInformation(LOG_UNEXPECTED_HTTP_STATUSCODE);
            return false;
        }

        public async Task<bool> RemoveGameExtensionAsync(int gameExtensionId)
        {
            var response = await _httpClient.DeleteAsync($"gameextensions/{gameExtensionId}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                logger.LogInformation(LOG_UNAUTHORIZED);
                return false;
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation(LOG_WARNING + nameof(GameExtension) + LOG_DELETE_FAILURE);
                return false;
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                logger.LogInformation(nameof(GameExtension) + LOG_DELETE_SUCCESS);
                return true;
            }

            logger.LogInformation(LOG_UNEXPECTED_HTTP_STATUSCODE);
            return false;
        }
    }
}
