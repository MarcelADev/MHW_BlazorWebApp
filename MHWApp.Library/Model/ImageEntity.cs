using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library.Model
{
    public class ImageEntity
    {
        public required int Id { get; set; }

        [Required]
        public required string Name { get; set; } = string.Empty;

        [Required]
        public required string ImageType { get; set; } = string.Empty;

        [Required]
        public required byte[] Data { get; set; } = Array.Empty<byte>();
    }
}
