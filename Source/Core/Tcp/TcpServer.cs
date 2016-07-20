using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NLog;

namespace Core.Tcp
{
    public class TcpServer : IDisposable
    {
        private const string LocalHost = "127.0.0.1";
        private readonly TcpListener _myTcpListener;
        private readonly ManualResetEvent _stopHandle = new ManualResetEvent(false);

        private Thread _mainThread;

        private bool _stopping;

        public TcpServer() : this(IPAddress.Parse(LocalHost), 58889)
        {
        }

        public TcpServer(IPAddress hostAddress, int portNumber)
        {
            _myTcpListener = new TcpListener(hostAddress, portNumber);
            collection.Add("12","Hello workd");
        }

        public Dictionary<string, string> collection { get; set; } = new Dictionary<string, string>();

        public void Close()
        {
            _stopping = true;
            _stopHandle.Set();
            _mainThread.Join();
        }

        public void Dispose()
        {
            Close();
        }

        public void Start()
        {
            if (_stopping)
            {
                return;
            }

            _mainThread = new Thread(Listen);
            _mainThread.Start();
        }

        private void HandleIncomingTcpClient(IAsyncResult result)
        {
            byte[] buffer = new byte[4096];
            using (TcpClient tcpClient = _myTcpListener.EndAcceptTcpClient(result))
            {
                using (NetworkStream networkStream = tcpClient.GetStream())
                {
                    using (var stream = new MemoryStream())
                    {
                        try
                        {
                            var readBytes = 0;
                            do
                            {
                                readBytes = networkStream.Read(buffer, 0, buffer.Length);
                                stream.Write(buffer, 0, readBytes);
                            }
                            while (networkStream.DataAvailable);
                            string id = Encoding.UTF8.GetString(stream.ToArray());

                            byte[] response = Encoding.UTF8.GetBytes(collection[id]);

                            foreach (byte item in response)
                            {
                                networkStream.WriteByte(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.GetCurrentClassLogger().Error(ex);
                        }
                    }
                }
            }
        }

        private void Listen()
        {
            _myTcpListener.Start();

            while (!_stopping)
            {
                IAsyncResult asyncResult = _myTcpListener.BeginAcceptTcpClient(HandleIncomingTcpClient, null);
                //blocks until a client has connected to the server or stopping has been signalled
                WaitHandle.WaitAny(new[]
                {
                    _stopHandle,
                    asyncResult.AsyncWaitHandle
                });
            }

            _myTcpListener.Stop();
        }
    }
}
