using MHWApp.Client.Services.Interfaces;
using MHWApp.Library.DTO.Create.Category_Armor_InventoryEntry;
using MHWApp.Library.Enums;
using MHWApp.Library.Exceptions;
using MHWApp.Library.Model;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace MHWApp.Client.Services
{
    public class PreferredColorSchemeService(IHttpClientFactory httpClientFactory, ILogger<PreferredColorSchemeService> logger, IGlobalAlertService globalAlertService) : IPreferredColorSchemeService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AuthAndResp");

        public event Action? OnChange;

        private ColorScheme _scheme;

        public ColorScheme Scheme
        {
            get => _scheme;
            set
            {
                if (_scheme != value)
                {
                    _scheme = value;
                    OnChange?.Invoke();
                    _ = PostPreferredColorScheme(value);
                }
            }
        }

        public bool IsInitialized { get; private set; } = false;

        public bool IsDarkMode => Scheme == ColorScheme.Dark;

        public async Task InitializeColorScheme()
        {
            if (IsInitialized) return;

            ColorScheme colorScheme = await GetColorSchemeAsync();
            _scheme = colorScheme;
        }

        public async Task<ColorScheme> GetColorSchemeAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "preferred-color-scheme");
            var response = await _httpClient.SendAsync(request);
            var preferredColorScheme = await response.Content.ReadFromJsonAsync<ColorScheme>();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            }

            return preferredColorScheme;
        }

        public async Task PostPreferredColorScheme(ColorScheme colorScheme)
        {
            var response = await _httpClient.PostAsJsonAsync("preferred-color-scheme", colorScheme);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                globalAlertService.ShowDanger("Invalid color scheme.");
                return;
            }
            
            if (!response.IsSuccessStatusCode)
            {
                globalAlertService.ShowDanger($"Unexpected error: {response.StatusCode}");
                return;
            }
            else
            {
                globalAlertService.ShowSuccess($"Saved new color scheme preference successfully");
                return;
            }
        }
    }
}
