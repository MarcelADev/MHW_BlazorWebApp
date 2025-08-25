using MHWApp.Library.DTO;
using MHWApp.Library.Exceptions;
using MHWApp.Library.Model;
using Microsoft.Extensions.Logging;
using MHWApp.Client.Identity;
using System.Net;
using System.Net.Http.Json;

namespace MHWApp.Client.Provider
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

        public async Task CreateSessionAsync(SessionCreateDTO createSessionDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("sessions", createSessionDTO);

            // It is allowed to create a session with the same name => HttpStatusCode.Confliict is not expected
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                logger.LogInformation($"Bad Request! Most likely cause: No Participators in the DTO list");
                throw new BadRequestException();
            }
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

        public async Task UpdateSessionAsync(SessionUpdateDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"sessions/{dto.UpdatedSession.Id}", dto);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Entity couldn't be updated, because it wasn't found: {dto.UpdatedSession.Name}");
                throw new EntityWasNotFoundException();
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return;
            }

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
        public async Task RemoveSessionAsync(int sessionId)
        {
            var response = await _httpClient.DeleteAsync($"sessions/{sessionId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation($"Session couldnt be deleted, because it wasn't found: Id={sessionId}");
                throw new EntityWasNotFoundException();
            }
            if (response.StatusCode == HttpStatusCode.NoContent) return;

            logger.LogWarning($"Unexpexted HTTP Response: {response.StatusCode.ToString()}");
            throw new UnexpectedResponseException(response.StatusCode.ToString());
        }
    }
}
