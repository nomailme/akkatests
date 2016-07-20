using System;
using Topshelf;

namespace TipsyWorker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.New(x =>
            {
                x.Service<ServiceBuilder>();
                x.SetDisplayName("TipsyWorker");
            }).Run();
        }
    }
}
