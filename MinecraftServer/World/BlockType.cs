using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer.World
{
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
