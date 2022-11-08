export function verify() {
    if (!chrome.webview)
        return false;

    if (!chrome.webview.hostObjects)
        return false;

    if (!chrome.webview.hostObjects.projectdocuments)
        return false;

    return true;
}

export async function getprojectfolderpaths(project) {
    return await chrome.webview.hostObjects.sync.projectdocuments.GetProjectFolderPaths(project);    
}