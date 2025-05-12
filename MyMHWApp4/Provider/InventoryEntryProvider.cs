using MHWApp.Library.Model;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net;
using MHWApp.Library.Exceptions;
using MHWApp.Library.DTO;

namespace MyMHWApp4.Provider
{
    public class InventoryEntryProvider(IHttpClientFactory httpClientFactory, ILogger<InventoryEntryProvider> logger)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AuthAndResp");

        public async Task<List<InventoryEntry>> GetInventoryEntriesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "inventoryEntries");
            var response = await _httpClient.SendAsync(request);
            var inventoryEntries = await response.Content.ReadFromJsonAsync<List<InventoryEntry>>();

            if (response.StatusCode == HttpStatusCode.OK) return inventoryEntries ?? throw new EntityWasNullException();

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task CreateInventoryEntryAsync(InventoryEntry newInventoryEntry)
        {
            var response = await _httpClient.PostAsJsonAsync("inventoryEntries", newInventoryEntry);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                logger.LogInformation($"Entity already exists: {newInventoryEntry.Name}");
                throw new EntityAlreadyExistsException();
            }
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var newCategory = await response.Content.ReadFromJsonAsync<InventoryEntry>();

                if (newCategory is null)
                {
                    logger.LogInformation($"Received 201 HTTP Statuscode, but the entity was null");
                    throw new EntityWasNullException();
                }

                logger.LogInformation($"Entity was successfully created: {newInventoryEntry.Name}");
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task<bool> UpdateInventoryEntryAsync(InventoryEntry updatedInventoryEntry)
        {
            var response = await _httpClient.PutAsJsonAsync($"inventoryEntries/{updatedInventoryEntry.Id}", updatedInventoryEntry);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Entity couldn't be updated, because it wasn't found: {updatedInventoryEntry.Name}");
                return false;
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task RemoveInventoryEntryAsync(int inventoryEntryId)
        {
            var response = await _httpClient.DeleteAsync($"inventoryEntries/{inventoryEntryId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Category couldnt be deleted, because it wasn't found: Id={inventoryEntryId}");
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
