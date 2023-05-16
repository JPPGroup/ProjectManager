using Blazorise.Snackbar;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Components;

namespace ProjectManager.Data
{
    public class UINotifier
    {
        SnackbarStack? snackbarStack;

        public void SetTarget(SnackbarStack ss)
        {
            snackbarStack = ss;
        }

        public async Task Notify(MarkupString message, SnackbarColor color)
        {
            Guard.IsNotNull(snackbarStack);
            await snackbarStack.PushAsync(message, color);
        }
    }
}
