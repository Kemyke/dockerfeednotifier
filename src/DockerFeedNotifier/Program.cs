using DockerFeedNotifier.Notifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DockerFeedNotifier
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(AppContext.BaseDirectory)
                                .AddJsonFile("appsettings.json", optional: false);

            var configRoot = builder.Build();

            var mmConfig = configRoot.GetSection("mattermostnotifier");
            var channel = mmConfig["channel"];
            var name = mmConfig["name"];
            var webhookurl = mmConfig["webhookurl"];
            var iconurl = mmConfig["iconurl"];

            MattermostNotifier notifier = new MattermostNotifier(new Uri(webhookurl), channel, name, new Uri(iconurl));

            var repos = configRoot.GetSection("repositories").GetChildren();
            foreach (var repo in repos)
            {
                DockerRepositoryWatcher watcher = new DockerRepositoryWatcher(repo.Value, notifier);
                watcher.CheckRefreshedTags().GetAwaiter().GetResult();
            }
        }
    }
}
