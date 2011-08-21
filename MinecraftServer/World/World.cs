using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MinecraftServer.Net;
using MinecraftServer.Entity;

namespace MinecraftServer.World
{
    public class World
    {

        public WorldManager WorldManager;
        public String Name { get; set; }
        public WorldLocation SpawnLocation;

        private Dictionary<int, Entity.Entity> Entities;
        private List<Chunk> Chunks;

        public World(WorldManager manager, String name, Boolean autoGenerate = false)
        {
            WorldManager = manager;
            Name = name;
            
            Chunks = new List<Chunk>();
            Entities = new Dictionary<int, Entity.Entity>();

            SpawnLocation = new WorldLocation(0, 65, 0);

            if(autoGenerate)
                Generate();
        }

        public void Generate()
        {
            Logger.Info("Generating new world; this may take a minute...");

            // TODO: Real world generation

            ChunkLocation chunkLoc;
            Chunk c;
            for (int x = SpawnLocation.X - 10; x <= SpawnLocation.X + 10; x++)
            {
                for (int z = SpawnLocation.Z - 10; z <= SpawnLocation.Z + 10; z++)
                {
                    chunkLoc = new ChunkLocation(x, 0, z);
                    c = new Chunk(this, chunkLoc);
                    c.FillTempChunk();
                    Chunks.Add(c);
                }
            }
            c = null;

            Logger.Info("World generation complete!");
            Save();
        }

        public void GenerateChunk(ChunkLocation loc)
        {
            // TODO
        }

        public Chunk GetChunk(WorldLocation loc)
        {
            return GetChunk(new ChunkLocation((int)Math.Floor((Double)loc.X / 16), loc.Y, (int)Math.Floor((Double)loc.Z / 16)));
        }

        public Chunk GetChunk(ChunkLocation loc)
        {
            // Check if chunk exists
            foreach(Chunk c in Chunks)
            {
                if (c.Location.Equals(loc))
                    return c;
            }

            Chunk chunk = new Chunk(this, loc);
            Chunks.Add(chunk);
            return chunk;
        }

        public void SetChunk(ChunkLocation loc, Chunk chunk)
        {
            // Check if chunk already exists
            for (int i = 0; i < Chunks.Count; i++)
            {
                if (Chunks[i].Location.Equals(loc))
                    Chunks[i] = chunk;
            }

            // If not, add it
            Chunks.Add(chunk);
        }

        public Nullable<Block> GetBlock(WorldLocation loc)
        {
            ChunkLocation cl = new ChunkLocation((int)Math.Floor((Double)loc.X / 16), loc.Y, (int)Math.Floor((Double)loc.Z / 16));
            Chunk c = GetChunk(cl);
            return c.GetBlock(loc);
        }

        public void AddEntity(Entity.Entity e)
        {
            Entities.Add(e.EntityID, e);
        }

        public void Save()
        {
            String worldFolder = WorldManager.WorldPath + "/" + Name;
            String worldFile = worldFolder + "/world.dat";
            String playerFile = worldFolder + "/player.dat";

            if(!Directory.Exists(worldFolder))
                Directory.CreateDirectory(worldFolder);

            if(File.Exists(worldFile))
            {
                Logger.Info("Backing up world file ['" + Name + "']");
                File.Delete(worldFile + ".bak");
                File.Copy(worldFile, worldFile + ".bak");
                File.Delete(worldFile);
            }

            if (File.Exists(playerFile))
            {
                Logger.Info("Backing up player file ['" + Name + "']");
                File.Delete(playerFile + ".bak");
                File.Copy(playerFile, playerFile + ".bak");
                File.Delete(playerFile);
            }

            Logger.Info("Saving world file ['" + Name + "']");
            FileStream fs;

            // World File
            fs = new FileStream(worldFile, FileMode.CreateNew, FileAccess.Write);
            fs.WriteInt(Chunks.Count); // Chunk Count
            byte[] cData;
            foreach (Chunk c in Chunks)
            {
                fs.WriteInt(c.Location.X); // X
                fs.WriteInt(c.Location.Z); // Y
                cData = c.GetChunkData();
                fs.WriteInt(cData.Length); // Chunk Data Length
                fs.Write(cData, 0, cData.Length); // Chunk Data (Length = 81920)
            }
            fs.Flush();
            fs.Close();

            // Player File
            fs = new FileStream(playerFile, FileMode.CreateNew, FileAccess.Write);
            // TODO
            fs.Flush();
            fs.Close();

            Logger.Info("World save complete ['" + Name + "']");
        }

        public void Load()
        {
            String worldFolder = WorldManager.WorldPath + "/" + Name;
            String worldFile = worldFolder + "/world.dat";
            String playerFile = worldFolder + "/player.dat";

            Logger.Info("Loading world file ['" + Name + "']");
            FileStream fs;
            
            // World File
            fs = new FileStream(worldFile, FileMode.Open, FileAccess.Read);
            
            int chunkCount = fs.ReadInt();
            int cDataLength;
            Chunk c;
            ChunkLocation cLoc;
            byte[] cData;
            for (int i = 0; i < chunkCount; i++)
            {
                cLoc = new ChunkLocation(fs.ReadInt(), 0, fs.ReadInt());
                cDataLength = fs.ReadInt();
                cData = new byte[cDataLength];
                fs.Read(cData, 0, cData.Length);
                c = new Chunk(this, cLoc);
                c.SetChunkData(cData);
                Chunks.Add(c);
            }

            fs.Close();
            // Player File
            fs = new FileStream(playerFile, FileMode.Open, FileAccess.Read);
            fs.Close();

            Logger.Info("World load complete ['" + Name + "']");
        }

    }
}
