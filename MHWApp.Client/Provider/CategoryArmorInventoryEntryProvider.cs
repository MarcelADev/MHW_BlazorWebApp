using MHWApp.Library.DTO.Create.Category_Armor_InventoryEntry;
using System.Net;
using System.Net.Http.Json;

namespace MHWApp.Client.Provider
{
    public class CategoryArmorInventoryEntryProvider(IHttpClientFactory httpClientFactory, ILogger<CategoryArmorInventoryEntryProvider> logger)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AuthAndResp");

        #region CREATE

        public async Task CreateCategoryArmorInventoryEntryAsync(CreateCategoryArmorInventoryEntryDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("categoriesArmorInventoryEntry", dto);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                
            }
            if (response.StatusCode == HttpStatusCode.Created)
            {

            }
        }

        #endregion
    }
}
