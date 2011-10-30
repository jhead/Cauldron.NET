using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MinecraftServer.Net;
using MinecraftServer.Entity;
using MinecraftServer.World;

namespace MinecraftServer
{
    public class Client
    {

        private Server Server;
        private Socket ClientSocket;
        private NetworkStream BaseStream;

        private DateTime SentLastKeepAlive;

        public Player Player { get; set; }
        public PacketStream Stream { get; set; }
        public IPEndPoint RemoteEP { get; set; }
        
        public Boolean HandshakeComplete { get; set; }
        public Boolean LoggedIn { get; set; }
        public Boolean Spawned { get; set; }
        public Boolean IsDisposed { get; set; }

        public Client(Server server, Socket clientSocket)
        {
            Server = server;
            ClientSocket = clientSocket;
            RemoteEP = (IPEndPoint)ClientSocket.RemoteEndPoint;

            Player = new Player();

            BaseStream = new NetworkStream(ClientSocket);
            Stream = new PacketStream(BaseStream);
        }

        public void Run()
        {
            Packet packet = null;

            while (ClientSocket.Connected && !Stream.Closed)
            {
                packet = Stream.ReadPacket();
                Server.PacketHandler.HandlePacket(packet, this);

                if (LoggedIn && (DateTime.Now - SentLastKeepAlive).Seconds > 10)
                    SendKeepAlive();
            }

            Logger.Info("Client disconnected: " + RemoteEP + " (Socket Closed)");

            if (!IsDisposed)
                Dispose();
        }

        public void Dispose()
        {
            IsDisposed = true;
            if (ClientSocket.Connected)
                ClientSocket.Dispose();

            Stream.Dispose();

            Server.RemoveClient(this);
        }

        public void SendInitialPosition()
        {
            // TODO: Save player location, etc.
            SpawnPositionPacket spawnPacket = new SpawnPositionPacket(0, 75, 0);

            Stream.WritePacket(spawnPacket);
            spawnPacket = null;

            PlayerPositionLookPacket positionPacket = new PlayerPositionLookPacket();
            positionPacket.X = 0d;
            positionPacket.Y = 75d;
            positionPacket.Z = 0d;
            positionPacket.Stance = positionPacket.Y + 1.60;
            positionPacket.Yaw = 0f;
            positionPacket.Pitch = 0f;
            positionPacket.OnGround = false;

            Stream.WritePacket(positionPacket);
            positionPacket = null;
        }

        public void SendKeepAlive()
        {
            Stream.WritePacket(new KeepAlivePacket());
            SentLastKeepAlive = DateTime.Now;
        }

        public void SendInitialChunks()
        {
            Chunk c = null;

            for (int xOff = -7; xOff <= 7; xOff++)
            {
                for (int zOff = -7; zOff <= 7; zOff++)
                {
                    c = Server.GetWorldManager().GetWorld(0).GetChunk(
                        new WorldLocation((int)Player.Location.X + (xOff * 16), (int)Player.Location.Y, (int)Player.Location.Z + (zOff * 16)));
                        
                    if (c != null)
                        SendChunk(c);
                }
            }
        }

        public void SendChunk(Chunk c, Boolean includePrechunk = true)
        {
            if (includePrechunk)
            {
                PreChunkPacket prechunkPacket = new PreChunkPacket(c.Location.X, c.Location.Z, true);
                Stream.WritePacket(prechunkPacket);
            }

            MapChunkPacket mapchunkPacket = new MapChunkPacket();
            mapchunkPacket.ChunkData = c.GetChunkData();
            mapchunkPacket.CompressedSize = mapchunkPacket.ChunkData.Length;
            mapchunkPacket.X = c.Location.X * 16;
            mapchunkPacket.Y = (short)0;
            mapchunkPacket.Z = c.Location.Z * 16;
            mapchunkPacket.SizeX = (byte)Chunk.SizeX;
            mapchunkPacket.SizeY = (byte)Chunk.SizeY;
            mapchunkPacket.SizeZ = (byte)Chunk.SizeZ;
            Stream.WritePacket(mapchunkPacket);
        }

        public void SendInitialInventory()
        {
            // TODO: Save player inventory, etc.
            Stream.WritePacket(new SetSlotPacket(0, 36, (short)278, 1, 0));
            Stream.WritePacket(new SetSlotPacket(0, 37, (short)BlockType.Dirt, 64, 0));
            Stream.WritePacket(new SetSlotPacket(0, 38, (short)BlockType.WoodPlank, 64, 0));
            Stream.WritePacket(new SetSlotPacket(0, 39, (short)BlockType.Stone, 64, 0));
        }

    }
}
