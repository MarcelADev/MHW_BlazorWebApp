using MHWApp.Library.Exceptions;
using MHWApp.Library.Model;
using Microsoft.Extensions.Logging;
using MHWApp.Client.Identity;
using System.Net;
using System.Net.Http.Json;

namespace MHWApp.Client.Provider
{
    public class ParticipatorProvider(IHttpClientFactory httpClientFactory, ILogger<ParticipatorProvider> logger)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AuthAndResp");

        public async Task<Participator> GetParticipatorAsync(int sessionId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"participators/{sessionId}?user_only=true");
            var response = await _httpClient.SendAsync(request);
            var participator = await response.Content.ReadFromJsonAsync<Participator>();

            if (response.StatusCode == HttpStatusCode.OK) return participator ?? throw new EntityWasNullException();

            logger.LogWarning($"Unexpected HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task<List<Participator>> GetParticipatorsAsync(int sessionId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"participators/{sessionId}?user_only=false");
            var response = await _httpClient.SendAsync(request);
            var participators = await response.Content.ReadFromJsonAsync<List<Participator>>();

            if (participators is null) throw new EntityWasNullException();

            if (response.StatusCode == HttpStatusCode.OK) return participators;

            logger.LogWarning($"Unexpected HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        //public async Task<bool> CreateParticipatorAsync(Participator newParticipator, bool isSelf)
        //{
        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync($"participators?isSelf={isSelf}", newParticipator);
        //        return response.IsSuccessStatusCode;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public async Task RemoveParticipatorAsync(int participatorId)
        {
            var response = await _httpClient.DeleteAsync($"participators/{participatorId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Participator couldnt be deleted, because it wasn't found: Id={participatorId}");
                throw new EntityWasNotFoundException();
            }
            if (response.StatusCode == HttpStatusCode.NoContent) return;

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
    }
}
