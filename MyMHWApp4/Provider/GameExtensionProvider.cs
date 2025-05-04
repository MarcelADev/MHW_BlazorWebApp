using MHWApp.Library.DTO;
using MHWApp.Library.Model;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using static MHWApp.Library.StaticVariables;
using MHWApp.Library.Exceptions;

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

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return gameExtensions ?? throw new EntityWasNullException();
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task<int> CreateGameExtensionAsync(GameExtension gameExtension)
        {
            var response = await _httpClient.PostAsJsonAsync("gameextensions", gameExtension);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                logger.LogInformation($"Entity already exists: {gameExtension.Name}");
                throw new EntityAlreadyExistsException();
            }
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var newGameExtension = await response.Content.ReadFromJsonAsync<GameExtension>();

                if (newGameExtension is null)
                {
                    logger.LogInformation($"Received 201 HTTP Statuscode, but the entity was null");
                    throw new EntityWasNullException();
                }

                logger.LogInformation($"Enttity was successfully created: {newGameExtension.Name}");
                return newGameExtension.Id;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task<bool> UpdateGameExtensionAsync(GameExtension updatedGameExtension)
        {
            var response = await _httpClient.PutAsJsonAsync($"gameextensions/{updatedGameExtension.Id}", updatedGameExtension);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Entity couldnt be updated, because it wasn't found: {updatedGameExtension.Name}");
                return false;
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task<bool> RemoveGameExtensionAsync(int gameExtensionId)
        {
            var response = await _httpClient.DeleteAsync($"gameextensions/{gameExtensionId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Entity couldnt be deleted, because it wasn't found: Id={gameExtensionId}");
                return false;
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
    }
}
