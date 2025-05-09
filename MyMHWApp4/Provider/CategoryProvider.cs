using MHWApp.Library.Model;
using System.Net.Http.Json;
using System.Net.Http;
using MHWApp.Library.DTO;
using System.Net;
using MHWApp.Library.Exceptions;
using Microsoft.Extensions.Logging;

namespace MyMHWApp4.Provider
{
    public class CategoryProvider(IHttpClientFactory httpClientFactory, ILogger<CategoryProvider> logger)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AuthAndResp");

        #region READ
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "categories");
            var response = await _httpClient.SendAsync(request);
            var categories = await response.Content.ReadFromJsonAsync<List<Category>>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return categories ?? throw new EntityWasNullException();
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task<List<InventoryEntryCategory>> GetInventoryCategoriesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "categories/inventoryentries");
            var response = await _httpClient.SendAsync(request);
            var categories = await response.Content.ReadFromJsonAsync<List<InventoryEntryCategory>>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return categories ?? throw new EntityWasNullException();
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
        #endregion

        #region CREATE
        public async Task CreateCategoryAsync(CategoryDTO categoryDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("categories", categoryDTO);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                logger.LogInformation($"Entity already exists: {categoryDTO.NewCategory.Name}");
                throw new EntityAlreadyExistsException();
            }
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var newCategory = await response.Content.ReadFromJsonAsync<Category>();

                if (newCategory is null)
                {
                    logger.LogInformation($"Received 201 HTTP Statuscode, but the entity was null");
                    throw new EntityWasNullException();
                }

                logger.LogInformation($"Entity was successfully created: {newCategory.Name}");
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task CreateInventoryEntryCategoryAsync(InventoryEntryCategory inventoryCategory)
        {
            var response = await _httpClient.PostAsJsonAsync($"categories/inventoryentries", inventoryCategory);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                logger.LogInformation($"InventoryCategory already exists: Id:{inventoryCategory.Id} CategoryId: {inventoryCategory.CategoryId}");
                throw new EntityAlreadyExistsException();
            }
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var newInventoryCategory = await response.Content.ReadFromJsonAsync<InventoryEntryCategory>();

                if (newInventoryCategory is null)
                {
                    logger.LogInformation($"Received 201 HTTP Statuscode, but the entity was null");
                    throw new EntityWasNullException();
                }

                logger.LogInformation($"InventoryCategory was successfully created: Id:{newInventoryCategory.Id} CategoryId: {newInventoryCategory.CategoryId}");
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task CreateWeaponCategoryAsync(WeaponCategory weaponCategory)
        {
            var response = await _httpClient.PostAsJsonAsync($"categories/weapons", weaponCategory);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                logger.LogInformation($"WeaponCategory already exists: Id:{weaponCategory.Id} CategoryId: {weaponCategory.CategoryId}");
                throw new EntityAlreadyExistsException();
            }
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var newWeaponCategory = await response.Content.ReadFromJsonAsync<WeaponCategory>();

                if (newWeaponCategory is null)
                {
                    logger.LogInformation($"Received 201 HTTP Statuscode, but the entity was null");
                    throw new EntityWasNullException();
                }

                logger.LogInformation($"WeaponCategory was successfully created: Id:{newWeaponCategory.Id} CategoryId: {newWeaponCategory.CategoryId}");
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task CreateArmorCategoryAsync(ArmorCategory armorCategory)
        {
            var response = await _httpClient.PostAsJsonAsync($"categories/armors", armorCategory);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                logger.LogInformation($"ArmorCategory already exists: Id:{armorCategory.Id} CategoryId: {armorCategory.CategoryId}");
                throw new EntityAlreadyExistsException();
            }
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var newArmorCategory = await response.Content.ReadFromJsonAsync<ArmorCategory>();

                if (newArmorCategory is null)
                {
                    logger.LogInformation($"Received 201 HTTP Statuscode, but the entity was null");
                    throw new EntityWasNullException();
                }

                logger.LogInformation($"ArmorCategory was successfully created: Id:{newArmorCategory.Id} CategoryId: {newArmorCategory.CategoryId}");
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
        #endregion

        #region UPDATE
        public async Task<bool> UpdateCategoryAsync(Category updatedCategory)
        {
            var response = await _httpClient.PutAsJsonAsync($"categories/{updatedCategory.Id}", updatedCategory);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Entity couldn't be updated, because it wasn't found: {updatedCategory.Name}");
                return false;
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
        #endregion

        #region DELETE
        public async Task RemoveCategoryAsync(int categoryId)
        {
            var response = await _httpClient.DeleteAsync($"categories/{categoryId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Category couldnt be deleted, because it wasn't found: Id={categoryId}");
                throw new EntityWasNotFoundException();
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task RemoveInventoryCategoryAsync(int inventoryCategoryId)
        {
            var response = await _httpClient.DeleteAsync($"categories/inventoryentries/{inventoryCategoryId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Inventory Category couldnt be deleted, because it wasn't found: Id={inventoryCategoryId}");
                throw new EntityWasNotFoundException();
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task RemoveWeaponCategoryAsync(int weaponCategoryId)
        {
            var response = await _httpClient.DeleteAsync($"categories/weapons/{weaponCategoryId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Weapon Category couldnt be deleted, because it wasn't found: Id={weaponCategoryId}");
                throw new EntityWasNotFoundException();
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task RemoveArmorCategoryAsync(int armorCategoryId)
        {
            var response = await _httpClient.DeleteAsync($"categories/armors/{armorCategoryId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Weapon Category couldnt be deleted, because it wasn't found: Id={armorCategoryId}");
                throw new EntityWasNotFoundException();
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
        #endregion
    }
}