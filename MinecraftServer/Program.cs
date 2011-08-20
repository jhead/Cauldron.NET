using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new Server(System.Net.IPAddress.Any, 25565).Run();
        }
    }
}
