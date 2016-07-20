using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using Core;
using Core.Actors;
using Core.Messages.Jobs;
using NLog;
using Topshelf;

namespace HeadService
{
    public class ServiceBuilder : ServiceControl
    {
        private static int counter = 0;
        private ActorSystem ActorSystem { get; set; }
        //        private TcpServer tcpServer;
        public static int GetCounter()
        {
            counter++;
            return counter;
        }

        public bool Start(HostControl hostControl)
        {
            //            tcpServer = new TcpServer(IPAddress.Any, 10001);
            //            tcpServer.Start();
            ActorSystem = ActorSystem.Create(Constants.ActorSystemName);

            Props parentActorProps = Props.Create<ParentActor>().WithRouter(FromConfig.Instance).WithDeploy(new Deploy(FromConfig.Instance));
            IActorRef parentActor = ActorSystem.ActorOf(parentActorProps, "parentActor");

            Props receiverActorProps = Props.Create<ReceiverActor>();
            IActorRef receiverActor = ActorSystem.ActorOf(receiverActorProps, "receiverActor");


            Parallel.For(0, 1000, x =>
            {
                parentActor.Ask(GetJobRequest(x))
                          .PipeTo(receiverActor);
            });

//            ActorSystem.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(500), parentActor, GetJobRequest(), receiverActor);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            ActorSystem.Terminate();
            return true;
        }

        private JobRequest GetJobRequest(int i)
        {
            return new JobRequest { Number = i };
        }
    }
}
