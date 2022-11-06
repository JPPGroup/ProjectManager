using Microsoft.AspNetCore.Http.Connections;
using Microsoft.JSInterop;

namespace ProjectManager.Data
{
    public class NativeFiles
    {
        IJSObjectReference _module;
        IJSRuntime _js;

        private bool? _available = null;

        public NativeFiles(IJSRuntime JS)
        {
            _js = JS;
        }

        private async Task Load()
        {
            _module = await _js.InvokeAsync<IJSObjectReference>("import", "./nativefiles.js");
        }


        public async Task<bool> Available()
        {
            if (_available != null)
                return _available.Value;

            if (_module == null)
                await Load();

             _available = await _module.InvokeAsync<bool>("verify");
            return _available.Value;
        }
    }
}
