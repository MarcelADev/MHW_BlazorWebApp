using MHWApp.Library.Model;
using MyMHWApp4.Identity;
using System.Net.Http.Json;

namespace MyMHWApp4.Provider
{
    public class SessionProvider(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

        public async Task<List<Session>> GetSessionAsync()
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
    }
}
