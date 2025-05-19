using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MHWApp.Api.Provider
{
    public class NameIdentifierUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // Use the ASP.NET Core Identity User ID (ClaimTypes.NameIdentifier)
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
