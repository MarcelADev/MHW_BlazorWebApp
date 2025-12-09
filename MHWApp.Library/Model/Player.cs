using MHWApp.Library.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MHWApp.Library.Model
{
    public class Player : IdentityUser<int>
    {
        [Required]
        public required bool IsApproved { get; set; }

        public ColorScheme PreferredColorScheme { get; set; } = ColorScheme.Light;
    }
}