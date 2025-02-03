using MHWApp.Library.Model;
using System.Net.Http.Json;

namespace MyMHWApp4.Provider
{
    public class SessionProvider(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

        public async Task<List<Session>> GetSessionsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Session>>("sessions");
            }
            catch (Exception ex)
            {
                return new List<Session>();
            }
        }

        public async Task<Session> CreateSessionAsync(Session newSession)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("sessions", newSession);
                var createdSession = await response.Content.ReadFromJsonAsync<Session>();

                if (response.IsSuccessStatusCode)
                {
                    return createdSession;
                }

                throw new Exception();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
