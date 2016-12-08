using DockerFeedNotifier.Mattermost;
using Newtonsoft.Json.Linq;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DockerFeedNotifier
{
    public class DockerRepositoryWatcher
    {
        private readonly string tagListUrlTemplate = "https://hub.docker.com/v2/repositories/{0}/tags/?page_size=5";
        private readonly string tagsUiUrlTemplate = "https://hub.docker.com/r/{0}/tags/";

        private string imageName;
        private string tagListUrl;
        private string tagsUiUrl;
        private string dbFileName;
        private Dictionary<string, string> alreadyKnownTags = new Dictionary<string, string>();
        private INotifier notifier;

        public DockerRepositoryWatcher(string imageName, INotifier notifier)
        {
            this.notifier = notifier;
            if (imageName.Split('/').Length == 1)
            {
                imageName = "library/" + imageName;
            }

            this.imageName = imageName;
            tagListUrl = string.Format(tagListUrlTemplate, imageName);
            tagsUiUrl = string.Format(tagsUiUrlTemplate, imageName);
            string filename = imageName.Replace('/', '-');
            dbFileName = $"./Data/{filename}.db";

            using (var fs = File.Open(dbFileName, FileMode.OpenOrCreate))
            using (var sr = new StreamReader(fs))
            {
                string log = sr.ReadToEnd();
                var lines = log.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var columns = line.Split('\t');
                    alreadyKnownTags.Add(columns[0], columns[1]);
                }
            }
        }

        public async Task CheckRefreshedTags()
        {
            RestClient client = new RestClient(tagListUrl);
            var resp = await client.Execute(new RestRequest(Method.GET));
            var content = JObject.Parse(resp.Content);
            var tags = content["results"];

            foreach (var tag in tags)
            {
                var lastUpdated = tag["last_updated"].ToString();
                var name = tag["name"].ToString();

                if(!alreadyKnownTags.ContainsKey(name) || alreadyKnownTags[name] != lastUpdated)
                {
                    string message;
                    if(alreadyKnownTags.ContainsKey(name))
                    {
                        message = $"The {imageName} tag {name} was updated on {lastUpdated}. <{tagsUiUrl}|Check it please!>";
                    }
                    else
                    {
                        message = $"New {imageName} tag {name} was pushed on {lastUpdated}. <{tagsUiUrl}|Check it please!>";
                    }

                    await notifier.Notify(message);

                    using (var fs = File.Open(dbFileName, FileMode.Append))
                    using (var sw = new StreamWriter(fs))
                    {
                        await sw.WriteLineAsync($"{name}\t{lastUpdated}");
                    }

                    await Task.Delay(1000);
                }
            }
        }
    }
}
