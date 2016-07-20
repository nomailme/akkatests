using System;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace StreamingBuddy
{
    public sealed class RabbitReader : IDisposable
    {
        private readonly ActorMaterializer materializer;
        private ConnectionFactory connectionFactory;

        public RabbitReader(ActorSystem actorSystem)
        {
            materializer = actorSystem.Materializer();
            Listen();
        }

        /// <summary>
        /// Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            materializer.Dispose();
        }

        private static byte[] GetMessage(IModel channel)
        {
            BasicGetResult message = channel.BasicGet("TestQueue", true);
//            BasicGetResult message = channel.BasicGet("TestQueue", true);
            if (message == null)
            {
                return new byte[0];
            }
            return message.Body;
        }

        private QueueItem ConvertToQueueItem(byte[] arg)
        {
            throw new NotImplementedException();
        }

        private void Listen()
        {
            connectionFactory = new ConnectionFactory();
            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            
            channel.ExchangeDeclare("TestExchange", ExchangeType.Direct);
            channel.QueueBind("TestQueue", "TestExchange", "", null);
            channel.BasicPublish("TestExchange", "", new BasicProperties(), Encoding.UTF8.GetBytes("hello"));


            Source<byte[], NotUsed> source = Source.Unfold(0, x => new Tuple<int, byte[]>(x, GetMessage(channel)));
            Flow<byte[], string, NotUsed> flow = Flow.FromFunction<byte[], string>(x => Encoding.UTF8.GetString(x));
            Sink<string, Task> sink = Sink.ForEach<string>(x => Console.Write(x));

            source.Via(flow).To(sink).Run(materializer);
        }
    }
}
