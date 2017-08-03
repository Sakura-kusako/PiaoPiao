using ClientPublic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ppNetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientS clientS = new ClientS();
            clientS.OpenClient();
            while (true)
            {
                Thread.Sleep(5);
                clientS.SendAll();
                for (int i = 0; i < 6; i++)
                {
                    int delay = clientS.GetDelay(i);
                    if(delay > 0)
                    {
                        //Console.WriteLine("clients " + i + " : " + delay + " ms");
                    }
                }
            }
        }
    }
}
