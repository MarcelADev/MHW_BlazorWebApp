using MHWApp.Client.Identity.Model;
using MHWApp.Library.DTO.Identity;

namespace MHWApp.Client.Identity
{
    /// <summary>
    /// Account management services.
    /// </summary>
    public interface IAccountManagement
    {
        /// <summary>
        /// Login service.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="password">User's password.</param>
        /// <returns>The result of the request serialized to <see cref="FormResult"/>.</returns>
        public Task<FormResult> LoginAsync(IdentityDTO dto);

        /// <summary>
        /// Log out the logged in user.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        public Task LogoutAsync();

        /// <summary>
        /// Registration service.
        /// </summary>
        /// <param name="identityDTO">User's name and password.</param>
        /// <returns>The result of the request serialized to <see cref="FormResult"/>.</returns>
        public Task RegisterAsync(IdentityDTO identityDTO);

        public Task<bool> CheckAuthenticatedAsync();

        /// <summary>
        /// Get info from server if registration is enabled.
        /// </summary>
        /// <returns></returns>
        public Task<bool> CheckRegistrationEnabled();
    }
}
