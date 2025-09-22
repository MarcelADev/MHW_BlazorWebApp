using MHWApp.Library.DTO.Images;
using MHWApp.Library.Model;

namespace MHWApp.Client.Provider.Interfaces
{
    public interface IImageProvider
    {
        Task<string> GetImageAsync(string fileName);
    }
}