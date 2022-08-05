using Blazorise.Snackbar;
using Microsoft.AspNetCore.Components;

namespace ProjectManager.Data
{
    public class UINotifier
    {
        SnackbarStack snackbarStack;

        public void SetTarget(SnackbarStack ss)
        {
            snackbarStack = ss;
        }
        
        public async Task Notify(MarkupString message, SnackbarColor color)
        {
            await snackbarStack.PushAsync(message, color);
        }
    }
}
