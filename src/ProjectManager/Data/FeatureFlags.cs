using Microsoft.AspNetCore.Components;
using ProjectManager.Data.Native;

namespace ProjectManager.Data
{
    public class FeatureFlags
    {
        public bool NativeFileAvailable { get; set; }

        public bool NativeOfficeAvailable { get; set; }

        public bool TasksEnabled { get; set; } = false;

        public bool NativeFileNotAvailable
        {
            get { return !NativeFileAvailable; }
            set { return; } //Required for binding
        }

        public bool NativeOfficeNotAvailable
        {
            get { return !NativeOfficeAvailable; }
            set { return; } //Required for binding
        }

        NativeFiles _nativeFiles { get; set; }

        public FeatureFlags(NativeFiles nativeFiles)
        {
            _nativeFiles = nativeFiles;
        }

        public async Task ScanNative()
        {
            NativeFileAvailable = await _nativeFiles.Available();
            NativeOfficeAvailable = false;
        }
    }
}
