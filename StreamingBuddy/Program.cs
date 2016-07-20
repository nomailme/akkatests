using System;
using Topshelf;
using Topshelf.Builders;

namespace StreamingBuddy
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.New(x =>
            {
                x.Service<ServiceBuilder>();
                x.SetDescription("Streaming buddy");
            }).Run();
        }
    }
}
