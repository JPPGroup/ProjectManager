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
    return await chrome.webview.hostObjects.projectdocuments.GetProjectFolderPaths(project);    
}

export async function writetofile(project, data) {
    return await chrome.webview.hostObjects.projectdocuments.WriteToFile(project, data);
}

export async function open(path) {
    return await chrome.webview.hostObjects.projectdocuments.Open(path);
}

export async function getsubfiles(path) {
    return await chrome.webview.hostObjects.projectdocuments.GetSubFiles(path);
}