using MHWApp.Library.DTO;
using MHWApp.Library.Exceptions;
using MHWApp.Library.Model;
using Microsoft.Extensions.Logging;
using MyMHWApp4.Identity;
using System.Net;
using System.Net.Http.Json;

namespace MyMHWApp4.Provider
{
    public class SessionProvider(IHttpClientFactory httpClientFactory, ILogger<SessionProvider> logger)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AuthAndResp");

        public async Task<List<Session>> GetSessionsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "sessions");
            var response = await _httpClient.SendAsync(request);
            var sessions = await response.Content.ReadFromJsonAsync<List<Session>>();

            if (response.StatusCode == HttpStatusCode.OK) return sessions ?? throw new EntityWasNullException();

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }

        public async Task CreateSessionAsync(CreateSessionDTO createSessionDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("sessions", createSessionDTO);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var newSession = await response.Content.ReadFromJsonAsync<Session>();

                if (newSession is null)
                {
                    logger.LogInformation($"Received 201 HTTP Statuscode, but the entity was null");
                    throw new EntityWasNullException();
                }

                logger.LogInformation($"Entity was successfully created: {newSession.Name}");
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
    }
}
