using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerFeedNotifier
{
    public interface INotifier
    {
        Task Notify(string message);
    }
}
