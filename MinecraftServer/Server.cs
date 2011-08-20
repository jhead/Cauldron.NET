using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MinecraftServer.Net;
using MinecraftServer.World;

namespace MinecraftServer
{
    public class Server
    {

        public IPAddress BindAddress { get; set; }
        public int BindPort { get; set; }
        public IPEndPoint LocalEP { get; set; }
        public Boolean Running { get; set; }

        public WorldManager WorldManager { get; set; }
        public PacketHandler PacketHandler { get; set; }
        public int TotalEntityCount { get; set; }

        public String ServerName = "Awesome Server";
        public long MapSeed = 0L;
        public Boolean IsNether = false;

        private Socket ServerSocket;
        private Dictionary<Socket, Client> Clients;
        private List<Thread> ClientThreads;

        public Server(IPAddress bindAddress, int bindPort)
        {
            BindAddress = bindAddress;
            BindPort = bindPort;
            LocalEP = new IPEndPoint(BindAddress, BindPort);

            Clients = new Dictionary<Socket, Client>();
            ClientThreads = new List<Thread>();

            WorldManager = new WorldManager(this);
            PacketHandler = new PacketHandler(this);
            TotalEntityCount = 0;
        }

        public void Run()
        {
            if(!TryBind())
                Logger.Fatal("Unable to bind to " + BindAddress.ToString() + ":" + BindPort);

            ServerSocket.Listen(0);

            Running = true;
            WorldManager.AddWorld(new World.World(WorldManager, "world"));

            Socket clientSocket;
            Client client;
            Thread clientThread;
            while (Running)
            {
                clientSocket = ServerSocket.Accept();
                client = new Client(this, clientSocket);
                Clients.Add(clientSocket, client);

                clientThread = new Thread(client.Run);
                ClientThreads.Add(clientThread);
                clientThread.Start();

                Logger.Info("New Connection: " + clientSocket.RemoteEndPoint);
                TotalEntityCount++;
            }

            Shutdown();
        }

        private Boolean TryBind()
        {
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                ServerSocket.Bind(LocalEP);
                Logger.Info("Server bound to " + BindAddress.ToString() + ":" + BindPort);
                return true;
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
            }

            return false;
        }

        public void Shutdown()
        {
            Logger.Info("Server is shutting down...");

            Running = false;

            lock (Clients)
            {
                foreach (Client client in Clients.Values)
                {
                    client.Dispose();
                }
            }

            lock (ClientThreads)
            {
                foreach (Thread thread in ClientThreads)
                {
                    thread.Abort();
                }
            }

            Clients.Clear();
            Clients = null;

            ClientThreads.Clear();
            ClientThreads = null;

            Logger.Info("Server shutdown complete.");

            Thread.CurrentThread.Abort();
        }

        public WorldManager GetWorldManager()
        {
            return WorldManager;
        }

        public void BroadcastPacket(Packet packet, Client SourceClient = null)
        {
            foreach (Client c in Clients.Values)
            {
                if(!c.Equals(SourceClient))
                    c.Stream.WritePacket(packet);
            }
        }

        public void OnBlockChange(Block b)
        {
            // TODO: Only send to players within range of block
            BroadcastPacket(new BlockChangePacket(b.GetX(), (byte)b.GetY(), b.GetZ(), (byte)b.Type, 0x00));
        }

    }
}
