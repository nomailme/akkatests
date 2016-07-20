using System;
using Akka.Actor;
using Core;
using Core.Tcp;
using Topshelf;

namespace TipsyWorker
{
    public class ServiceBuilder : ServiceControl
    {
        private ActorSystem ActorSystem { get; set; }

        public bool Start(HostControl hostControl)
        {
            ActorSystem = ActorSystem.Create(Constants.ActorSystemName);
//            MyTcpClient.Do();
            
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            ActorSystem.Terminate();
            return true;
        }
    }
}
