using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer.World
{
    public struct Block
    {
        public BlockType Type;
        public WorldLocation Location;

        public Block(BlockType type, WorldLocation loc)
        {
            Type = type;
            Location = loc;
        }

        public void SetBlockType(BlockType type)
        {
            Type = type;
        }

        public BlockType GetBlockType()
        {
            return Type;
        }
    }
}

