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

        public const int SizeX = 15;
        public const int SizeY = 127;
        public const int SizeZ = 15;

        public World World;
        public ChunkLocation Location { get; set; }
        public List<Block> Blocks;

        public Chunk(World world, ChunkLocation loc)
        {
            World = world;
            Location = loc;
            Blocks = new List<Block>();
            // FillTempChunk(); // Temp
        }

        public Nullable<Block> GetBlock(WorldLocation loc)
        {
            foreach (Block b in Blocks)
            {
                if(b.Location.Equals(loc))
                    return b;
            }

            return null;
        }

        public void FillTempChunk()
        {
            BlockType type;
            for (int x = 0; x < SizeX + 1; x++)
            {
                for (int z = 0; z < SizeZ + 1; z++)
                {
                    for (int y = 0; y < SizeY + 1; y++)
                    {
                        if (y == 1)
                            type = BlockType.Bedrock;
                        else if (y > 1 && y <= 4)
                            type = BlockType.Dirt;
                        else if (y == 5)
                            type = BlockType.Grass;
                        else
                            type = BlockType.Air;

                        Blocks.Add(new Block(type, new WorldLocation(x + (Location.X * 16), y, z + (Location.Z * 16))));
                    }
                }
            }
        }

        public void SetChunkData(byte[] cData, Boolean isCompressed = true)
        {
            MemoryStream byteStream = new MemoryStream();
            ZlibStream zStream = new ZlibStream(byteStream, CompressionMode.Decompress, CompressionLevel.BestCompression, true);
            zStream.Write(cData, 0, cData.Length);
            zStream.Flush();
            zStream.Dispose();
            zStream = null;

            byte[] bytes = byteStream.ToArray();
            byteStream.Dispose();
            byteStream = null;

            Block b;
            WorldLocation blockLoc;
            int x, y, z;
            // Block Types
            for (int i = 0; i < 32768; i++)
            {
                x = (Location.X * 16) + (i >> 11);
                y = i & 0x7F;
                z = (Location.Z * 16) + ((i & 0x780) >> 7);
                blockLoc = new WorldLocation(x, y, z);
                b = new Block((BlockType)bytes[i], blockLoc);
                Blocks.Add(b);
            }

            // TODO: Read and handle block/sky light and metadata

            bytes = null;
        }

        public byte[] GetChunkData()
        {
            MemoryStream byteStream = new MemoryStream();
            byte[] bytes = new byte[Blocks.Count + ((Blocks.Count / 2) * 3)]; 

            int index;
            foreach (Block b in Blocks)
            {
                index = b.Location.Y + ((b.Location.Z - (Location.Z * 16)) * (SizeY + 1)) + ((b.Location.X - (Location.X * 16)) * (SizeY + 1) * (SizeZ + 1));
                byteStream.Seek(index, SeekOrigin.Begin);
                byteStream.WriteByte((byte)b.Type);
            }

            byteStream.Seek(Blocks.Count, SeekOrigin.Begin);

            for (int a = 1; a <= 3; a++)
            {
                for (int b = 0; b < 16384; b++)
                {
                    switch (a)
                    {
                        case 1:
                            byteStream.WriteByte(0xFF);
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
            zs.Dispose();
            zs = null;
            byte[] finalBytes = ms.ToArray();
            ms.Dispose();
            ms = null;
            bytes = null;
            //

            return finalBytes;
        }

    }
}
