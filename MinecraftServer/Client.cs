using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MinecraftServer.Entity;

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
        
        public Boolean HandshakeComplete { get; set; }
        public Boolean LoggedIn { get; set; }

        public Client(Server server, Socket clientSocket)
        {
            Server = server;
            ClientSocket = clientSocket;

            Player = new Player();

            BaseStream = new NetworkStream(ClientSocket);
            Stream = new PacketStream(BaseStream);
        }

        public void Run()
        {
            Packet packet = null;

            while (ClientSocket.Connected)
            {
                packet = Stream.ReadPacket();
                Server.PacketHandler.HandlePacket(packet, this);

                if (LoggedIn && (DateTime.Now - SentLastKeepAlive).Seconds > 10)
                    SendKeepAlive();
            }

            Logger.Info("Client disconnected: " + ClientSocket.RemoteEndPoint + " (Socket Closed)");
        }

        public void Dispose()
        {

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
            // Pre-Chunks
            PreChunkPacket prechunkPacket;
            for (int x = -3; x <= 3; x++)
            {
                for (int z = -3; z <= 3; z++)
                {
                    prechunkPacket = new PreChunkPacket(x, z, true);
                    Stream.WritePacket(prechunkPacket);
                }
            }

            // Chunks
            MapChunkPacket mapchunkPacket;
            for (int x = -3; x <= 3; x++)
            {
                for (int z = -3; z <= 3; z++)
                {
                    mapchunkPacket = new MapChunkPacket();
                    mapchunkPacket.X = x * 16;
                    mapchunkPacket.Y = 0 * 128;
                    mapchunkPacket.Z = z * 16;
                    mapchunkPacket.SizeX = 15;
                    mapchunkPacket.SizeY = 127;
                    mapchunkPacket.SizeZ = 15;

                    byte[] chunkData = Server.GetWorldManager().GetWorld(0).GetChunk(x, z).GetChunkData();
                    mapchunkPacket.CompressedSize = chunkData.Length;
                    mapchunkPacket.ChunkData = chunkData;

                    Stream.WritePacket(mapchunkPacket);
                }
            }
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
