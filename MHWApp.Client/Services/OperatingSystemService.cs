using MHWApp.Client.Services.Interfaces;
using MHWApp.Library.Enums;
using Microsoft.JSInterop;

namespace MHWApp.Client.Services
{
    public sealed class OperatingSystemService : IOperatingSystemService, IAsyncDisposable
    {
        private readonly IJSRuntime _js;
        private IJSObjectReference? _module;
        private OperatingSystemType? _cachedOs;

        public OperatingSystemService(IJSRuntime js)
        {
            _js = js;
        }

        public async ValueTask<OperatingSystemType> GetOperatingSystemAsync()
        {
            if (_cachedOs.HasValue)
                return _cachedOs.Value;

            _module ??= await _js.InvokeAsync<IJSObjectReference>(
                "import", "./js/os-utils-2.js");

            var osString = await _module.InvokeAsync<string>("getOperatingSystem2");

            _cachedOs = osString switch
            {
                "Windows" => OperatingSystemType.Windows,
                "macOS" => OperatingSystemType.MacOS,
                "Linux" => OperatingSystemType.Linux,
                "Android" => OperatingSystemType.Android,
                "iOS" => OperatingSystemType.IOS,
                _ => OperatingSystemType.Unknown
            };

            return _cachedOs.Value;
        }

        public async ValueTask DisposeAsync()
        {
            if (_module is not null)
                await _module.DisposeAsync();
        }
    }
}
