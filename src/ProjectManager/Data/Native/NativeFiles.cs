using CommunityToolkit.Diagnostics;
using Microsoft.JSInterop;

namespace ProjectManager.Data.Native
{
    public class NativeFiles
    {
        IJSObjectReference? _module;
        readonly IJSRuntime _js;

        private bool? _available = null;

        public NativeFiles(IJSRuntime js)
        {
            _js = js;
        }

        private async Task Load()
        {
            _module = await _js.InvokeAsync<IJSObjectReference>("import", "./nativefiles.js");

            if (_module == null)
                throw new InvalidOperationException("Unable to load module");
        }

        public async Task<bool> Available()
        {
            if (_available != null)
                return _available.Value;

            if (_module == null)
                await Load();

            Guard.IsNotNull(_module);
            _available = await _module.InvokeAsync<bool>("verify");
            return _available.Value;
        }

        public async Task<string[]> GetProjectFolderPaths(string code)
        {
            Guard.IsNotNull(_module);

            try
            {
                var resp = await _module.InvokeAsync<string[]>("getprojectfolderpaths", code);
                return resp;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<ProjectFolder> GetProjectFolder(string code)
        {
            var result = await GetProjectFolderPaths(code);
            return new ProjectFolder(code, this, result);
        }

        public async Task WriteToFile(string path, MemoryStream data)
        {
            Guard.IsNotNull(_module);

            try
            {
                string datastring = Convert.ToBase64String(data.ToArray());
                await _module.InvokeVoidAsync("writetofile", path, datastring);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task Open(string path)
        {
            Guard.IsNotNull(_module);
            await _module.InvokeVoidAsync("open", path);
        }

        public async Task<string[]> GetSubFiles(string path)
        {
            try
            {
                Guard.IsNotNull(_module);
                var resp = await _module.InvokeAsync<string[]>("getsubfiles", path);
                return resp;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
