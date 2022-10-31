using Microsoft.AspNetCore.Http.Connections;
using Microsoft.JSInterop;

namespace ProjectManager.Data
{
    public class NativeFiles
    {
        IJSObjectReference _module;
        IJSRuntime _js;

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
            if (_module == null)
                await Load();

            return await _module.InvokeAsync<bool>("verify");
        }
    }
}
