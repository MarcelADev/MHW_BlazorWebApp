using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MHWApp.Library.Model
{
    public class Player : IdentityUser
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
