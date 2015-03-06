using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

using Jobbr.Common;

using Newtonsoft.Json;

namespace Jobbr.Runtime
{
    /// <summary>
    /// The jobbr run time client.
    /// </summary>
    public class JobbrRuntimeClient
    {
        private readonly string jobServer;

        private readonly long jobRunId;

        private HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrRuntimeClient"/> class.
        /// </summary>
        /// <param name="jobServer">
        /// The server url.
        /// </param>
        /// <param name="jobRunId"></param>
        public JobbrRuntimeClient(string jobServer, long jobRunId)
        {
            this.jobServer = jobServer;
            this.jobRunId = jobRunId;

            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri(jobServer + (jobServer.EndsWith("/") ? string.Empty : "/") + "client/");
        }

        public bool PublishState(JobRunState state)
        {
            var url = string.Format("jobRun/{0}", this.jobRunId);
            var content = new JobRunUpdateDto { State = state };

            var request = this.httpClient.PutAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
            var result = request.Result;

            return result.StatusCode == HttpStatusCode.Accepted;
        }

        public bool SendFiles(string[] files)
        {
            var multipartContent = new MultipartFormDataContent();
            var fileNumber = 0;

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                multipartContent.Add(new StreamContent(File.OpenRead(file)), "result", fileName);
            }

            var url = string.Format("jobRun/{0}/artefacts", this.jobRunId);
            var response = this.httpClient.PostAsync(url, multipartContent).Result;
        
            return response.StatusCode == HttpStatusCode.Accepted;
        }

        public JobRunInfoDto GetJobRunInfo()
        {
            var url = string.Format("jobRun/{0}", this.jobRunId);

            var request = this.httpClient.GetAsync(url);
            var result = request.Result;

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var content = result.Content.ReadAsStringAsync().Result;

                var dto = JsonConvert.DeserializeObject<JobRunInfoDto>(content);

                return dto;
            }

            return null;
        }
    }
}
