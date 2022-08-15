using CommonDataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace ProjectManager.Data.ProjectIntegration
{
    public class ProjectService
    {
        public event EventHandler? ProjectListChanged;

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

        public async Task<ProjectDetails?> GetProjectDetails(string projectcode)
        {
            using var message = GetProjectDetailsRequestMessage(projectcode);
            var response = await client.SendAsync(message).ConfigureAwait(false);

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var responseCollection = JsonConvert.DeserializeObject<ProjectDetails>(result);
                if (responseCollection != null)
                    return responseCollection;
            }

            return null;
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

            if (response.IsSuccessStatusCode)
            {
                var responseCollection = JsonConvert.DeserializeObject<IList<ProjectResponse>>(result);
                if (responseCollection != null)
                    return responseCollection;
            }

            return new List<ProjectResponse>();            
        }

        public async IAsyncEnumerable<Invoice> GetUnpaidInvoices(string company)
        {
            Stream stream;

            try
            {
                var builder = new UriBuilder
                {
                    Scheme = "http",
                    Host = "services.cedarbarn.local",
                    Port = 80,
                    Path = "projects/api/invoices/bycompany",
                    Query = $"company={company}&unpaidonly=true&includedrafts=false"
                };

                stream = await client.GetStreamAsync(builder.Uri);
                
            }
            catch (Exception e)
            {
                throw;
            }

            /*TextReader reader = new StreamReader(stream);
            string response = await reader.ReadToEndAsync();
            var invoices = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Invoice>>(response);*/

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultBufferSize = 25
            };

            await foreach (Invoice? i in System.Text.Json.JsonSerializer.DeserializeAsyncEnumerable<Invoice?>(stream, options))
            {
                if (i != null)
                    yield return i;
            }
        }

        public async IAsyncEnumerable<Invoice> GetMonthInvoices(string company, DateTime targetMonth)
        {
            Stream stream;
            var firstDayOfMonth = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            try
            {
                var builder = new UriBuilder
                {
                    Scheme = "http",
                    Host = "services.cedarbarn.local",
                    Port = 80,
                    Path = "projects/api/invoices/bycompany",
                    Query = $"company={company}&unpaidonly=false&includedrafts=true&fromDate={HttpUtility.UrlEncode(firstDayOfMonth.ToString("yyyy-MM-dd"))}&toDate={HttpUtility.UrlEncode(lastDayOfMonth.ToString("yyyy-MM-dd"))}"
                };

                stream = await client.GetStreamAsync(builder.Uri);

            }
            catch (Exception e)
            {
                throw;
            }

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultBufferSize = 25
            };

            await foreach (Invoice? i in System.Text.Json.JsonSerializer.DeserializeAsyncEnumerable<Invoice?>(stream, options))
            {
                if (i != null)
                    yield return i;
            }
        }

        public async Task<IList<ProjectResponse>> RequestListFromService(string company)
        {
             var builder = new UriBuilder
             {
                 Scheme = "http",
                 Host = "services.cedarbarn.local",
                 Port = 80,
                 Path = "projects/api/projects",
                 Query = $"company={company}"
             }; 

            using var message = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = builder.Uri,
            };
            var response = await this.client.SendAsync(message).ConfigureAwait(false);

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

        private static HttpRequestMessage GetProjectDetailsRequestMessage(string projectcode)
        {
            var builder = new UriBuilder
            {
                Scheme = "http",
                Host = "services.cedarbarn.local",
                Port = 80,
                Path = "projects/api/projectdetails",
                Query = $"projectcode={projectcode}"
            };

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
