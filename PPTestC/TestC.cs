using ClientPublic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PPTestC
{
    class TestC
    {
        static void Main(string[] args)
        {
            ClientC clientC = new ClientC();
            clientC.OpenClient();
            while (true)
            {
                Thread.Sleep(16);
                clientC.SendAll();
                int delay = clientC.GetDelay();
                if (delay > 0)
                {
                    Console.WriteLine("clients " + " : " + delay + " ms");
                }
                clientC.UpdateTime();
                if (clientC.IsConnect() == false)
                    break;
            }
        }
    }
}
