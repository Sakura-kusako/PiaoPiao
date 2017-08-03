using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace AsyncClient
{
    class Program
    {
        private static IPEndPoint epServer;
        private static UdpClient local;

        static void Main(string[] args)
        {
            //设置服务器端IP和端口  
            epServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10800);
            local = new UdpClient(9001);    //绑定本机IP和端口，9001  
            while (true)
            {
                string strSend = Console.ReadLine();
                if (strSend == "exit") break;
                byte[] sendData = Encoding.ASCII.GetBytes(strSend);
                //开始异步发送，启动一个线程，该线程启动函数是：SendCallback，该函数中结束挂起的异步发送  
                local.BeginSend(sendData, sendData.Length, epServer, new AsyncCallback(SendCallback), null);
                //开始异步接收启动一个线程，该线程启动函数是：ReceiveCallback，该函数中结束挂起的异步接收  
                local.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            }
        }

        private static void SendCallback(IAsyncResult iar)
        {
            int sendCount = local.EndSend(iar);
            if (sendCount == 0)
            { Console.WriteLine("Send a message failure..."); }
        }

        private static void ReceiveCallback(IAsyncResult iar)
        {
            byte[] receiveData = local.EndReceive(iar, ref epServer);
            Console.WriteLine("Server: {0}", Encoding.ASCII.GetString(receiveData));
        }
    }
}