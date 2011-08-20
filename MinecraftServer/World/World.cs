using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer.World
{
    public class World
    {

        public WorldManager WorldManager;
        public String Name { get; set; }
        public WorldLocation SpawnLocation;

        private Dictionary<ChunkLocation, Chunk> Chunks;

        public World(WorldManager manager, String name, Boolean autogenerate = false)
        {
            WorldManager = manager;
            Name = name;
            Chunks = new Dictionary<ChunkLocation, Chunk>();
            SpawnLocation = new WorldLocation(0, 65, 0);

            if(autogenerate)
                Generate();
        }

        public void Generate()
        {
            Logger.Info("Generating new world; this may take a minute...");

            ChunkLocation chunkLoc;
            for (int x = SpawnLocation.X - 3; x <= SpawnLocation.X + 3; x++)
            {
                for (int z = SpawnLocation.Z - 3; z <= SpawnLocation.Z + 3; z++)
                {
                    chunkLoc = new ChunkLocation(x, 0, z);
                    Chunks.Add(chunkLoc, new Chunk(this, chunkLoc));
                }
            }

            Logger.Info("World generation complete!");
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
            foreach(ChunkLocation cl in Chunks.Keys)
            {
                if(cl.Equals(loc))
                    return Chunks[cl];
            }

            Chunk c = new Chunk(this, loc);
            Chunks.Add(loc, c);
            return c;
        }

        public void SetChunk(ChunkLocation loc, Chunk chunk)
        {
            // Check if chunk already exists
            foreach (ChunkLocation cl in Chunks.Keys)
            {
                if (cl.Equals(loc))
                {
                    Chunks[cl] = chunk;
                    return;
                }
            }

            // If not, add it
            Chunks[loc] = chunk;
        }

        public Block GetBlock(WorldLocation loc)
        {
            ChunkLocation cl = new ChunkLocation((int)Math.Floor((Double)loc.X / 16), loc.Y, (int)Math.Floor((Double)loc.Z / 16));
            Chunk c = GetChunk(cl);
            return c.GetBlock(loc);
        }

    }
}
