using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.Event;
using Akka.Routing;
using Core.Messages.Jobs;

namespace Core.Actors
{
    public class ParentActor : ReceiveActor, IWithUnboundedStash
    {
        private readonly ILoggingAdapter logger;
        private List<JobResponse> responses;
        private IActorRef sender;

        public ParentActor()
        {
            Props childActorProps = Props.Create<ChildActor>().WithRouter(new ClusterRouterPool(new RandomPool(1), new ClusterRouterPoolSettings(10,2,true,"worker") ));
            Props anotherChildActorProps = Props.Create<AnotherChildActor>().WithRouter(new ClusterRouterPool(new RandomPool(1), new ClusterRouterPoolSettings(10, 2, true, "worker")));

            Children.Add(Context.ActorOf(childActorProps, "childActor"));
            Children.Add(Context.ActorOf(anotherChildActorProps, "anotherChildActor"));

            logger = Context.GetLogger();

            Become(Waiting);
        }

        private List<IActorRef> Children { get; set; } = new List<IActorRef>();

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new AllForOneStrategy(10, TimeSpan.FromSeconds(15), x => Directive.Restart);
        }

        private void Handle(JobRequest message)
        {
            Children.ForEach(x => x.Tell(message));
        }

        private void Handle(JobResponse message)
        {
            responses.Add(message);
            if (responses.Count == Children.Count)
            {
                sender.Tell(responses, Self);
                Stash.UnstashAll();
                UnbecomeStacked();
            }
        }

        private void Waiting()
        {
            Receive<JobRequest>(x =>
            {
                sender = Sender;
                responses = new List<JobResponse>();
                Handle(x);
                BecomeStacked(WaitingForResponse);
            });
        }

        private void WaitingForResponse()
        {
            Receive<JobResponse>(x => { Handle(x); });
            Receive<JobRequest>(x => Stash.Stash());
        }

        /// <summary>
        /// Gets or sets the stash. This will be automatically populated by the framework AFTER the constructor has been run.
        ///             Implement this as an auto property.
        /// </summary>
        /// <value>
        /// The stash.
        /// </value>
        public IStash Stash { get; set; }
    }
}
