using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ProjectManager.Data.ProjectIntegration
{
    public class ProjectService
    {
        public event EventHandler ProjectListChanged;

        private readonly HttpClient client;
        private IList<ProjectResponse> projects;

        public ProjectService()
        {
            client = CreateHttpClient();
            projects = new List<ProjectResponse>();
        }

        public async Task<IEnumerable<ProjectResponse>> GetProjects(string firstname, string lastname)
        {
            await ReloadProjects(firstname, lastname);
            return projects;
        }

        private async Task ReloadProjects(string firstname, string lastname)
        {
            var list = await RequestFromService(firstname, lastname);
            if (list.Count == projects.Count) return;

            projects = list;
            OnProjectListChanged(EventArgs.Empty);
        }

        private void OnProjectListChanged(EventArgs e)
        {
            var handler = ProjectListChanged;
            handler?.Invoke(this, e);
        }        

        private async Task<IList<ProjectResponse>> RequestFromService(string firstname, string lastname)
        {
            using var message = GetProjectRequestMessage(firstname, lastname);
            var response = await client.SendAsync(message).ConfigureAwait(false);

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return response.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<IList<ProjectResponse>>(result)
                : new List<ProjectResponse>();
        }

        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler { UseDefaultCredentials = true };
            var clientHttp = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(10) };
            clientHttp.DefaultRequestHeaders.Accept.Clear();
            clientHttp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return clientHttp;
        }

        private static HttpRequestMessage GetProjectRequestMessage(string firstname, string lastname)
        {
            var builder = GetUriBuilder("projects/api/projects/user", firstname, lastname);

            return new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = builder.Uri,
            };
        }

        private static UriBuilder GetUriBuilder(string path, string firstname, string lastname)
        {
            return new UriBuilder
            {
                Scheme = "http",
                Host = "services.cedarbarn.local",
                Port = 80,
                Path = path,
                Query = $"firstname={firstname}&lastname={lastname}"
            };
        }
    }
}
