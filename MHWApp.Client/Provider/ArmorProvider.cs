using MHWApp.Library.Model;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net;
using MHWApp.Library.Exceptions;
using MHWApp.Library.DTO;

namespace MHWApp.Client.Provider
{
    public class ArmorProvider(IHttpClientFactory httpClientFactory, ILogger<ArmorProvider> logger)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AuthAndResp");

        public async Task<List<Armor>> GetArmorsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "armors");
            var response = await _httpClient.SendAsync(request);
            var armors = await response.Content.ReadFromJsonAsync<List<Armor>>();

            if (response.StatusCode == HttpStatusCode.OK) return armors ?? throw new EntityWasNullException();

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task CreateArmorAsync(Armor newArmor)
        {
            var response = await _httpClient.PostAsJsonAsync("armors", newArmor);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                logger.LogInformation($"Entity already exists: {newArmor.Name}");
                throw new EntityAlreadyExistsException();
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                throw new BadRequestException(errorMessage);
            }
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var armor = await response.Content.ReadFromJsonAsync<Armor>();

                if (armor is null)
                {
                    logger.LogInformation($"Received 201 HTTP Statuscode, but the entity was null");
                    throw new EntityWasNullException();
                }

                logger.LogInformation($"Entity was successfully created: {armor.Name}");
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task UpdateArmorAsync(Armor updatedArmor)
        {
            var response = await _httpClient.PutAsJsonAsync($"armors/{updatedArmor.Id}", updatedArmor);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Entity couldn't be updated, because it wasn't found: {updatedArmor.Name}");
                throw new EntityWasNotFoundException();
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task RemoveArmorAsync(int armorId)
        {
            var response = await _httpClient.DeleteAsync($"armors/{armorId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Armor couldnt be deleted, because it wasn't found: Id={armorId}");
                throw new EntityWasNotFoundException();
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
    }
}
