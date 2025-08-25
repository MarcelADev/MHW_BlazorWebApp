using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace MHWApp.Client.Identity
{
    /// <summary>
    /// Handler to log out the user and redirect to the start page if a 401 Unauthorized response is received.
    /// </summary>
    public class MiddlewareHttpResponseHandler(NavigationManager navigationManager, IAccountManagement accountManagement) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            // Check if the response is 401 Unauthorized
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Log out user
                await accountManagement.LogoutAsync();

                // Redirect to the logout page
                navigationManager.NavigateTo("/logout");
            }
            
            return response;
        }
    }
}

