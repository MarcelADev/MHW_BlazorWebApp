using MHWApp.Library.DTO.Images;
using MHWApp.Library.Model;

namespace MHWApp.Client.Services.Interfaces
{
    public interface IImageService
    {
        Task SupplyImageServiceWithImagesAsync();
        bool TryGetImage(string imageName, out ImageDTO? imageDto);
    }
}
