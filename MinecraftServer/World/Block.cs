using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer.World
{
    public class Block
    {

        public Chunk Chunk { get; set; }
        public WorldLocation Location { get; set; }
        public BlockType Type;

        public Block(Chunk chunk, BlockType type, WorldLocation loc)
        {
            Chunk = chunk;
            Type = type;
            Location = loc;
        }

        public int GetX()
        {
            return Location.X;
        }

        public int GetY()
        {
            return Location.Y;
        }

        public int GetZ()
        {
            return Location.Z;
        }

        public void BreakBlock()
        {
            Type = BlockType.Air;
            // TODO: Drop item
            Chunk.World.WorldManager.Server.OnBlockChange(this);
        }

    }

    public enum BlockType : byte
    {
        Air = 0x00,
        Stone = 0x01,
        Grass = 0x02,
        Dirt = 0x03,
        Cobblestone = 0x04,
        WoodPlank = 0x05,
        Sapling = 0x06,
        Bedrock = 0x07
    }
}
