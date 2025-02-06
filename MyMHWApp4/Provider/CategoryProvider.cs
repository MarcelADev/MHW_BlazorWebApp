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

        #region CREATE
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

        public async Task<bool> CreateInventoryEntryCategoryAsync(InventoryEntryCategory inventoryEntryCategory)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"categories/inventoryentries", inventoryEntryCategory);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateWeaponCategoryAsync(WeaponCategory weaponCategory)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"categories/weapons", weaponCategory);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateArmorCategoryAsync(ArmorCategory armorCategory)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"categories/armors", armorCategory);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region READ
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
        #endregion

        #region UPDATE
        public async Task<bool> UpdateCategoryAsync(Category updatedCategory)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"categories/{updatedCategory.Id}", updatedCategory);

                if (response.StatusCode == HttpStatusCode.NotFound) return false;
                else if (response.StatusCode == HttpStatusCode.NoContent) return true;

                throw new Exception();
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        #endregion

        #region DELETE
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
        #endregion
    }
}
