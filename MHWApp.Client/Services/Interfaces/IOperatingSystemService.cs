using MHWApp.Library.Enums;

namespace MHWApp.Client.Services.Interfaces
{
    public interface IOperatingSystemService
    {
        ValueTask<OperatingSystemType> GetOperatingSystemAsync();
    }
}