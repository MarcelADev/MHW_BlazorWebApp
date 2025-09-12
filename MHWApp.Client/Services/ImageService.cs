using MHWApp.Client.Provider.Interfaces;
using MHWApp.Client.Services.Interfaces;
using MHWApp.Library.DTO.Images;
using MHWApp.Library.Exceptions;
using MHWApp.Library.Model;

namespace MHWApp.Client.Services
{
    public class ImageService(IImageProvider imageProvider) : IImageService
    {
        private List<ImageDTO>? _images;
        public async Task SupplyImageServiceWithImagesAsync()
        {
            if (_images == null)
            {
                _images = await imageProvider.GetImagesAsync();
            }
        }

        public bool TryGetImage(string imageName, out ImageDTO? imageDto)
        {
            imageDto = null;

            if (_images == null) return false;

            imageDto = _images.FirstOrDefault(i =>
                string.Equals(i.Name, imageName, StringComparison.OrdinalIgnoreCase));

            return imageDto != null;
        }
    }
}
