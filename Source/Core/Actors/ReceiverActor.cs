using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Core.Messages.Jobs;
using NLog;

namespace Core.Actors
{
    public class ReceiverActor : UntypedActor
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void Handle(List<JobResponse> message)
        {
            logger.Info($"Reply {message.First().Number}");
        }

        protected override void OnReceive(object message)
        {
            if (message is Task<object>)
            {
                var result = (message as Task<object>).Result;

                Handle(result as List<JobResponse>);
            }
        }
    }
}
