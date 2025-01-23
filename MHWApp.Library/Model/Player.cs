using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace MHWApp.Library.Model
{
    public class Player : IdentityUser<int>, IModel
    {
        
    }
}
