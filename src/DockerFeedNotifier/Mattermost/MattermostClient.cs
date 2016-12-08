using Newtonsoft.Json;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerFeedNotifier.Mattermost
{
    public sealed class MattermostClient
    {
        private readonly Uri webHookUri;

        public MattermostClient(Uri webHookUri)
        {
            this.webHookUri = webHookUri;
        }

        public async Task SendSlackMessage(Message message)
        {
            using (RestClient webClient = new RestClient())
            {
                RestClient c = new RestClient(this.webHookUri);
                RestRequest req = new RestRequest(Method.POST);
                req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                req.AddBody("payload=" + JsonConvert.SerializeObject(message), Encoding.UTF8);
                await c.Execute(req);
            }
        }
    }
}
