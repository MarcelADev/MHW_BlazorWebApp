using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWApp.Library.DTO.Images
{
    public class ImageDTO
    {
        public required string Name { get; set; }
        public required string ImageType { get; set; }
        public required string Base64Data { get; set; }
    }
}
