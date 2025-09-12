using MHWApp.Library.Exceptions;
using MHWApp.Library.Model;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using MHWApp.Client.Provider.Interfaces;
using MHWApp.Library.DTO.Images;

namespace MHWApp.Client.Provider
{
    public class ImageProvider(IHttpClientFactory httpClientFactory, ILogger<ImageProvider> logger) : IImageProvider
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AuthAndResp");

        public async Task<List<ImageDTO>> GetImagesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "images");
            var response = await _httpClient.SendAsync(request);

            var images = await response.Content.ReadFromJsonAsync<List<ImageDTO>>();

            if (response.StatusCode == HttpStatusCode.OK) return images ?? throw new EntityWasNullException();

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
    }
}
