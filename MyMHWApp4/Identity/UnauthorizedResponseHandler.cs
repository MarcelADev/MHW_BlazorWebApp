using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace MyMHWApp4.Identity
{
    /// <summary>
    /// Handler to log out the user and redirect to the start page if a 401 Unauthorized response is received.
    /// </summary>
    public class UnauthorizedResponseHandler(NavigationManager navigationManager, AuthenticationStateProvider cookieAuthenticationStateProvider) : DelegatingHandler
    {
        //private readonly NavigationManager _navigationManager;

        //public UnauthorizedResponseHandler()
        //{
        //    _navigationManager = navigationManager;
        //}

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            // Check if the response is 401 Unauthorized
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Log out user (for example, by clearing cookies, local storage, etc.)
                // You can also invoke your authentication service to clear the authentication token or session.

                // Redirect to the start page or login page
                navigationManager.NavigateTo("/"); // Or use the appropriate route

                // Optionally, you could also handle any other cleanup (e.g., clearing session or tokens).
            }

            return response;
        }
    }
}

