using System;
using System.Threading;
using Akka.Actor;
using Core.Messages.Jobs;
using NLog;

namespace Core.Actors
{
    public class AnotherChildActor: TypedActor, IHandle<JobRequest>
    {
        public void Handle(JobRequest message)
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));

            LogManager.GetCurrentClassLogger().Info("Child message handler");
            Sender.Tell(new JobResponse { Number = message.Number }, Self);
        }
    }
}