using ProjectManager.Data.Native;

namespace ProjectManager.Data
{
    public class FeatureFlags
    {
        public bool NativeFileAvailable { get; set; }

        public bool NativeOfficeAvailable { get; set; }

        public bool TasksEnabled
        {
            get { return _userProfile.TasksEnabled; }
            set
            {
                _userProfile.TasksEnabled = value;
                _state.SaveChangesAsync();
            }
        }

        public bool CreationEnabled
        {
            get { return _userProfile.CreationEnabled; }
            set
            {
                _userProfile.CreationEnabled = value;
                _state.SaveChangesAsync();
            }
        }

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
        TaskStateMachine _state;

        UserProfile _userProfile;

        public FeatureFlags(NativeFiles nativeFiles, TaskStateMachine state)
        {
            _nativeFiles = nativeFiles;
            _state = state;

            _userProfile = _state.GetUser();

        }

        public async Task ScanNative()
        {
            NativeFileAvailable = await _nativeFiles.Available();
            NativeOfficeAvailable = false;
        }
    }
}
