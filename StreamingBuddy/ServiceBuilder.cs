using Akka.Actor;
using Topshelf;

namespace StreamingBuddy
{
    public class ServiceBuilder: ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            ActorSystem = ActorSystem.Create("StreamingBuddy");
            var rabbitReader = new RabbitReader(ActorSystem);
            
            return true;
        }

        public ActorSystem ActorSystem { get; set; }

        public bool Stop(HostControl hostControl)
        {
            ActorSystem.Terminate();
            return true;
        }
    }
}