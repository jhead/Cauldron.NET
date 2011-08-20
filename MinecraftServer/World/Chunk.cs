using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zlib;

namespace MinecraftServer.World
{
    public class Chunk
    {

        public World World;
        public ChunkLocation Location { get; set; }
        public Dictionary<WorldLocation, Block> Blocks;

        public int SizeX = 15;
        public int SizeY = 127;
        public int SizeZ = 15;

        public Chunk(World world, ChunkLocation loc)
        {
            World = world;
            Location = loc;
            Blocks = new Dictionary<WorldLocation, Block>();
            FillTempChunk(); // Temp
        }

        public Block GetBlock(WorldLocation loc)
        {
            foreach (WorldLocation wl in Blocks.Keys)
            {
                if (wl.Equals(loc))
                    return Blocks[wl];
            }

            return null;
        }

        public void FillTempChunk()
        {
            WorldLocation loc = null;
            Block block;
            BlockType type;
            for (int x = 0; x < SizeX + 1; x++)
            {
                for (int z = 0; z < SizeZ + 1; z++)
                {
                    for (int y = 0; y < SizeY + 1; y++)
                    {
                        loc = new WorldLocation(x + (Location.X * 16), y, z + (Location.Z * 16));

                        if (y == 1)
                            type = BlockType.Bedrock;
                        else if (y > 1 && y <= 4)
                            type = BlockType.Dirt;
                        else if (y == 5)
                            type = BlockType.Grass;
                        else
                            type = BlockType.Air;
                        
                        block = new Block(this, type, loc);
                        Blocks.Add(loc, block);
                    }
                }
            }
        }

        public byte[] GetChunkData()
        {
            MemoryStream byteStream = new MemoryStream();
            byte[] bytes = new byte[Blocks.Values.Count + ((Blocks.Values.Count / 2) * 3)]; 

            int index;
            foreach (Block b in Blocks.Values)
            {
                index = b.GetY() + ((b.GetZ() - (Location.Z * 16)) * (SizeY + 1)) + ((b.GetX() - (Location.X * 16)) * (SizeY + 1) * (SizeZ + 1));
                byteStream.Seek(index, SeekOrigin.Begin);
                byteStream.WriteByte((byte)b.Type);
            }

            byteStream.Seek(Blocks.Values.Count, SeekOrigin.Begin);

            for (int a = 1; a <= 3; a++)
            {
                for (int b = 0; b < 16384; b++)
                {
                    switch (a)
                    {
                        case 1:
                            byteStream.WriteByte(0x00);
                            break;
                        default:
                            byteStream.WriteByte(0xFF);
                            break;
                    }
                }
            }
            
            bytes = byteStream.ToArray();
            byteStream.Dispose();
            byteStream = null;

            // Compress array
            MemoryStream ms = new MemoryStream();
            ZlibStream zs = new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.Default, true);
            zs.Write(bytes, 0, bytes.Length);
            zs.Flush();
            zs.Close();
            byte[] finalBytes = ms.ToArray();
            //

            bytes = null;

            return finalBytes;
        }

    }
}
