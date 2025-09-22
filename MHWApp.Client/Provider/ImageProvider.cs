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

        public async Task<string> GetImageAsync(string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "images/" + fileName);
            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound) throw new EntityWasNotFoundException();

            var base64String = await response.Content.ReadFromJsonAsync<string>();

            if (response.StatusCode == HttpStatusCode.OK) return base64String ?? throw new EntityWasNullException();

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
    }
}
