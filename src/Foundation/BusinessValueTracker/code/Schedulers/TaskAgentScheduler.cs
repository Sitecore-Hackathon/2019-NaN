using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Processors;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Schedulers
{
    public class TaskAgentScheduler
    {
        public virtual void Run()
        {
            new TaskAgent().RegisterAll().Wait();
        }
    }
}