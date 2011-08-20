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

        private Dictionary<ChunkLocation, Chunk> Chunks;

        public World(WorldManager manager, String name)
        {
            WorldManager = manager;
            Name = name;
            Chunks = new Dictionary<ChunkLocation, Chunk>();
        }

        public Chunk GetChunk(int x, int z)
        {
            return GetChunk(new ChunkLocation(x, 0, z));
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
