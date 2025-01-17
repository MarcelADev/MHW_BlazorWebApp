using MHWApp.Library.Model;
using System.Net.Http.Json;
using System.Net.Http;

namespace MyMHWApp4.Provider
{
    public class InventoryEntryProvider(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

        public async Task<List<InventoryEntry>> GetInventoryEntriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<InventoryEntry>>("inventoryEntry");
            }
            catch (Exception ex)
            {
                return new List<InventoryEntry>();
            }
        }

        public async Task<bool> CreateInventoryEntryAsync(InventoryEntry newInventoryEntry)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("inventoryEntry", newInventoryEntry);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
