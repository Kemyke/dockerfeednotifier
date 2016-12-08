using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerFeedNotifier.Mattermost
{
    public class Message
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
    }
}
