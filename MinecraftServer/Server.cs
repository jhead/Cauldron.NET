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

        private WorldManager WorldManager { get; set; }
        public PacketHandler PacketHandler { get; set; }
        public int TotalEntityCount { get; set; }

        public String ServerName = "Awesome Server";
        public long MapSeed = 0L;
        public Boolean IsNether = false;

        private Socket ServerSocket;
        private Dictionary<Socket, Client> Clients;
        private Dictionary<Client, Thread> ClientThreads;
        private Timer TickTimer;
        private DateTime LastCleanup;

        public Server(IPAddress bindAddress, int bindPort)
        {
            BindAddress = bindAddress;
            BindPort = bindPort;
            LocalEP = new IPEndPoint(BindAddress, BindPort);

            Clients = new Dictionary<Socket, Client>();
            ClientThreads = new Dictionary<Client, Thread>();

            WorldManager = new WorldManager(this);
            PacketHandler = new PacketHandler(this);
            TotalEntityCount = 0;
            TickTimer = null;
        }

        public void Run()
        {
            if(!TryBind())
                Logger.Fatal("Unable to bind to " + BindAddress.ToString() + ":" + BindPort);

            ServerSocket.Listen(0);
            Running = true;

            WorldManager.Init();
            if(WorldManager.Worlds.Count == 0)
                WorldManager.AddWorld(new World.World(WorldManager, "awesome_world", true));

            TickTimer = new Timer(new TimerCallback(Tick), null, 0, 50);

            Socket clientSocket;
            Client client;
            Thread clientThread;
            while (Running)
            {
                clientSocket = ServerSocket.Accept();
                client = new Client(this, clientSocket);
                Clients.Add(clientSocket, client);

                clientThread = new Thread(client.Run);
                ClientThreads.Add(client, clientThread);
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
                foreach (Thread thread in ClientThreads.Values)
                {
                    thread.Abort();
                }
            }

            Clients.Clear();
            Clients = null;

            ClientThreads.Clear();
            ClientThreads = null;

            foreach (World.World world in WorldManager.Worlds.Values)
            {
                world.Save();
            }

            Logger.Info("Server shutdown complete.");

            Thread.CurrentThread.Abort();
        }

        public void Tick(Object stateInfo)
        {
            // TODO: Update time, water, lava, fire, etc.
            if ((DateTime.Now - LastCleanup).Seconds > 10)
            {
                GC.Collect();
                LastCleanup = DateTime.Now;
            }
        }

        public void RemoveClient(Client client)
        {
            Socket cs = null;

            lock (Clients)
            {
                foreach (Socket s in Clients.Keys)
                {
                    if (Clients[s].Equals(client))
                        cs = s;
                }
            }

            if (cs != null)
                Clients.Remove(cs);

            client = null;
            cs = null;
        }

        public WorldManager GetWorldManager()
        {
            return WorldManager;
        }

        public void BroadcastPacket(Packet packet, Client SourceClient = null)
        {
            Client[] clientArray = Clients.Values.ToArray();

            foreach (Client c in clientArray)
            {
                if (!c.Equals(SourceClient) && c.LoggedIn)
                    c.Stream.WritePacket(packet);
            }

           clientArray = null;
        }

        public void OnBlockChange(Block b)
        {
            // TODO: Only send to players within range of block
            BroadcastPacket(new BlockChangePacket(b.Location.X, (byte)b.Location.Y, b.Location.Z, (byte)b.GetBlockType(), 0x00));
        }

        public Client[] GetClients()
        {
            lock (Clients)
            {
                return Clients.Values.ToArray();
            }
        }

    }
}
