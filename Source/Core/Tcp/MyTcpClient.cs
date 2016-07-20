using System.IO;
using System.Net.Sockets;
using System.Text;
using NLog;

namespace Core.Tcp
{
    public class MyTcpClient
    {
        public static void Do()
        {
            
            var client = new TcpClient("localhost", 10001);
            var data = Encoding.GetEncoding(1252).GetBytes("12");
            var stm = client.GetStream();
//            stm.Write(data, 0, data.Length);
            byte[] resp = new byte[2048];
            var memStream = new MemoryStream();
            stm.Write(data, 0, data.Length);
            while (stm.DataAvailable)
            {
                int bytes = memStream.Read(resp, 0, resp.Length);
                memStream.Write(resp,0,bytes);
            }
            string message = Encoding.GetEncoding(1252).GetString(memStream.ToArray()); 
            LogManager.GetCurrentClassLogger().Debug(message);
        }
    }
}