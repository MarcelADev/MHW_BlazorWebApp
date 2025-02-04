using MHWApp.Library.Model;
using System.Net.Http.Json;
using System.Net.Http;
using MHWApp.Library.DTO;
using System.Net;

namespace MyMHWApp4.Provider
{
    public class CategoryProvider(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                var categories = await _httpClient.GetFromJsonAsync<List<Category>>($"categories");

                if (categories != null)
                    return categories;

                return new List<Category>();
            }
            catch (Exception ex)
            {
                return new List<Category>();
            }
        }

        public async Task<List<InventoryEntryCategory>> GetInventoryEntryCategoriesAsync()
        {
            try
            {
                var invetoryEntryCategories = await _httpClient.GetFromJsonAsync<List<InventoryEntryCategory>>($"categories/inventoryentries");

                if (invetoryEntryCategories != null)
                    return invetoryEntryCategories;

                return new List<InventoryEntryCategory>();
            }
            catch (Exception ex)
            {
                return new List<InventoryEntryCategory>();
            }
        }

        public async Task<List<WeaponCategory>> GetWeaponCategoriesAsync()
        {
            try
            {
                var weaponCategories = await _httpClient.GetFromJsonAsync<List<WeaponCategory>>($"categories/weapons");

                if (weaponCategories != null)
                    return weaponCategories;

                return new List<WeaponCategory>();
            }
            catch (Exception ex)
            {
                return new List<WeaponCategory>();
            }
        }

        public async Task<List<ArmorCategory>> GetArmorCategoriesAsync()
        {
            try
            {
                var armorCategories = await _httpClient.GetFromJsonAsync<List<ArmorCategory>>($"categories/armors");

                if (armorCategories != null)
                    return armorCategories;

                return new List<ArmorCategory>();
            }
            catch (Exception ex)
            {
                return new List<ArmorCategory>();
            }
        }

        public async Task<bool> CreateAllCategoriesAsync(CategoryDTO categoryDTO)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("categories", categoryDTO);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RemoveCategoryAsync(int categoryId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"categories/{categoryId}");
                return response.StatusCode == HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
