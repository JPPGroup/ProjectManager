using Blazorise;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.JSInterop;

namespace ProjectManager.Data.Native
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

        public async Task<string[]> GetProjectFolderPaths(string code)
        {
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
            ProjectFolder folder = new ProjectFolder(code, this);
            folder.Paths = await GetProjectFolderPaths(code);
            //TODO: Replace with a better system to determine
            folder.PrimaryPath = folder.Paths[0];
            return folder;
        }

        public async Task WriteToFile(string path, MemoryStream data)
        {
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
    }
}
