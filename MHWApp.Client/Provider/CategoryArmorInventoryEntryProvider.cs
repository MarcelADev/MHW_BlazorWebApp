using MHWApp.Library.DTO.Create.Category_Armor_InventoryEntry;
using MHWApp.Library.Exceptions;
using MHWApp.Library.Model;
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
            var response = await _httpClient.PostAsJsonAsync("categoriesArmorsInventoryEntries", dto);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                logger.LogInformation("Category already exists: {Category name}", dto.Name);
                throw new EntityAlreadyExistsException();
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                logger.LogInformation("Category was successfully created: {Category name}", dto.Name);
                return;
            }

            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        #endregion
    }
}
