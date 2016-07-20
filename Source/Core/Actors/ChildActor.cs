using System;
using System.Threading;
using Akka.Actor;
using Core.Messages.Jobs;
using NLog;

namespace Core.Actors
{
    public class ChildActor : TypedActor, IHandle<JobRequest>
    {
        public void Handle(JobRequest message)
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));

            LogManager.GetCurrentClassLogger().Info($"Child message handler {message.Number}");
            Sender.Tell(new JobResponse { Number = message.Number }, Self);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new AllForOneStrategy(10, TimeSpan.FromSeconds(15), x => Directive.Restart);
        }
    }
}
