using MHWApp.Library.Model;
using System.Net.Http.Json;

namespace MyMHWApp4.Provider
{
    public class ParticipatorProvider(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

        public async Task<Participator> GetParticipatorAsync(int sessionId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Participator>($"participators/{sessionId}");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> CreateParticipatorAsync(Participator newParticipator, bool isSelf)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"participators?isSelf={isSelf}", newParticipator);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
