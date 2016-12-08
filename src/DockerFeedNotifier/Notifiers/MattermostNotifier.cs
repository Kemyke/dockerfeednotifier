using DockerFeedNotifier.Mattermost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerFeedNotifier.Notifiers
{
    public class MattermostNotifier : INotifier
    {
        private string user;
        private string channel;
        private string iconUrl;
        MattermostClient mmClient;

        public MattermostNotifier(Uri webhookUrl, string channel, string user, Uri icon)
        {
            this.user = user;
            this.channel = channel;
            this.iconUrl = icon.ToString();

            mmClient = new MattermostClient(webhookUrl);
        }

        public async Task Notify(string message)
        {
            await mmClient.SendSlackMessage(new Message { Channel = channel, Text = message, UserName = user, IconUrl = iconUrl });
        }
    }
}
