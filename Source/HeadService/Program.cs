using System;
using Topshelf;

namespace HeadService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.New(x =>
            {
                x.Service<ServiceBuilder>();
                x.SetDisplayName("HeadService");
            }).Run();
        }
    }
}
