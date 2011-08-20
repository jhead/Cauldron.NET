using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer.World
{
    public class Block
    {

        public Chunk Chunk { get; set; }

        // Type is readonly outside of this class; use SetBlockType() instead.
        private BlockType _Type;
        public BlockType Type
        {
            get
            {
                return _Type;
            }
            set { }
        }

        // Location cannot be set outside of this class; aka, blocks cannot move.
        private WorldLocation _Location;
        public WorldLocation Location
        {
            get
            {
                return _Location;
            }
            set { }
        }

        public Block(Chunk chunk, BlockType type, WorldLocation loc)
        {
            Chunk = chunk;
            _Type = type;
            _Location = loc;
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

        public BlockType GetBlockType()
        {
            return _Type;
        }

        public void SetBlockType(BlockType type)
        {
            _Type = type;
            Chunk.World.WorldManager.Server.OnBlockChange(this);
        }

        public void BreakBlock()
        {
            _Type = BlockType.Air;
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
