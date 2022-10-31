export function verify() {
    if (!chrome.webview)
        return false;

    if (!chrome.webview.hostObjects)
        return false;

    if (!chrome.webview.hostObjects.projectdocuments)
        return false;

    return true;
}