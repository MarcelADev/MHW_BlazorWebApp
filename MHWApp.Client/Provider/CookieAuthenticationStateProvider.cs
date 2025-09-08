using MHWApp.Client.Identity.Model;
using MHWApp.Client.Provider.Interfaces;
using MHWApp.Library.DTO.Identity;
using MHWApp.Library.Exceptions;
using MHWApp.Library.Model;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace MHWApp.Client.Provider
{
    /// <summary>
    /// Handles state for cookie-based auth.
    /// </summary>
    /// <remarks>
    /// Create a new instance of the auth provider.
    /// </remarks>
    /// <param name="httpClientFactory">Factory to retrieve auth client.</param>
    public class CookieAuthenticationStateProvider(IHttpClientFactory httpClientFactory, ILogger<CookieAuthenticationStateProvider> logger)
        : AuthenticationStateProvider, IAccountManagement
    {
        /// <summary>
        /// Map the JavaScript-formatted properties to C#-formatted classes.
        /// </summary>
        private readonly JsonSerializerOptions jsonSerializerOptions =
            new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

        /// <summary>
        /// Special auth client.
        /// </summary>
        private readonly HttpClient httpClient = httpClientFactory.CreateClient("Auth");

        /// <summary>
        /// Authentication state.
        /// </summary>
        private bool authenticated = false;

        /// <summary>
        /// Default principal for anonymous (not authenticated) users.
        /// </summary>
        private readonly ClaimsPrincipal unauthenticated = new(new ClaimsIdentity());

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="email">The user's userName address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>The result serialized to a <see cref="FormResult"/>.
        /// </returns>
        public async Task RegisterAsync(IdentityDTO dto)
        {
            // make the request
            var result = await httpClient.PostAsJsonAsync("register", dto);

            if(result.StatusCode == HttpStatusCode.Conflict)
            {
                var problem = result.Content.ReadFromJsonAsync<ProblemDetails>();

                if (problem is null || problem.Result is null || problem.Result.Title == string.Empty) 
                { 
                    throw new ConflictException("Conflict occured. Problem Details were empty"); 
                }

                if (problem.Result.Title.Contains("Prime Vu"))
                {
                    throw new ConflictException("Prime Vu is not assigned. It is achieved");
                }

                if (problem.Result.Title.Contains("Username not allowed"))
                {
                    throw new ConflictException($"\"{dto.UserName}\" is a banned username. Choose another one");
                }

                if (problem.Result.Title.Contains("Username already taken"))
                {
                    throw new ConflictException($"\"{dto.UserName}\" isn't available. Choose another one");
                }

                throw new ConflictException(problem.Result.Title);
            }

            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return;
            }

            if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new BadRequestException();
            }

            throw new UnexpectedResponseException(result.StatusCode.ToString());
        }

        /// <summary>
        /// User login request through custom POST endpoint
        /// </summary>
        /// <param name="userName">The user's userName.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>The result of the login request serialized to a <see cref="FormResult"/>.</returns>
        public async Task LoginAsync(IdentityDTO dto)
        {
            var result = await httpClient.PostAsJsonAsync("login", dto);

            if (result.StatusCode == HttpStatusCode.Conflict)
            {
                throw new ConflictException();
            }

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedRequestException();
            }

            if (result.StatusCode == HttpStatusCode.OK)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());    // need to refresh auth state

                return;
            }

            throw new UnexpectedResponseException(result.StatusCode.ToString());
        }

        /// <summary>
        /// Get authentication state.
        /// </summary>
        /// <remarks>
        /// Called by Blazor anytime and authentication-based decision needs to be made, then cached
        /// until the changed state notification is raised.
        /// </remarks>
        /// <returns>The authentication state asynchronous request.</returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            authenticated = false;

            // default to not authenticated
            var user = unauthenticated;

            try
            {
                // the user info endpoint is secured, so if the user isn't logged in this will fail
                var request = new HttpRequestMessage(HttpMethod.Get, "/manage/info");
                request.SetBrowserRequestOption("credentials", "include");
                var response = await httpClient.SendAsync(request);

                // throw if user info wasn't retrieved
                response.EnsureSuccessStatusCode();

                // user is authenticated,so let's build their authenticated identity
                var userJson = await response.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, jsonSerializerOptions);

                if (userInfo != null)
                {
                    // in this example app, name and userName are the same
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userInfo.Email),
                        new(ClaimTypes.Email, userInfo.Email),
                    };

                    // add any additional claims
                    claims.AddRange(
                        userInfo.Claims.Where(c => c.Key != ClaimTypes.Name && c.Key != ClaimTypes.Email)
                            .Select(c => new Claim(c.Key, c.Value)));

                    // request the roles endpoint for the user's roles
                    var rolesResponse = await httpClient.GetAsync("roles");

                    // throw if request fails
                    rolesResponse.EnsureSuccessStatusCode();

                    // read the response into a string
                    var rolesJson = await rolesResponse.Content.ReadAsStringAsync();

                    // deserialize the roles string into an array
                    var roles = JsonSerializer.Deserialize<RoleClaim[]>(rolesJson, jsonSerializerOptions);

                    // add any roles to the claims collection
                    if (roles?.Length > 0)
                    {
                        foreach (var role in roles)
                        {
                            if (!string.IsNullOrEmpty(role.Type) && !string.IsNullOrEmpty(role.Value))
                            {
                                claims.Add(new Claim(role.Type, role.Value, role.ValueType, role.Issuer, role.OriginalIssuer));
                            }
                        }
                    }

                    // set the principal
                    var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
                    user = new ClaimsPrincipal(id);
                    authenticated = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "App error");
            }

            // return the state
            return new AuthenticationState(user);
        }

        public async Task LogoutAsync()
        {
            const string Empty = "{}";
            var emptyContent = new StringContent(Empty, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("logout", emptyContent);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<bool> CheckAuthenticatedAsync()
        {
            await GetAuthenticationStateAsync();
            return authenticated;
        }
    }
}