using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer
{
    public class Location
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Location(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            // TODO
            return base.Equals(obj);
        }

    }

    public class ChunkLocation
    {
        public int X { get; set; }
        public int Z { get; set; }
        public int Y { get; set; }

        public ChunkLocation(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChunkLocation))
                return false;

            ChunkLocation loc = (ChunkLocation)obj;

            if (loc.X == X && loc.Z == Z)
                return true;

            return false;
        }
    }

    public class WorldLocation : ChunkLocation
    {

        public WorldLocation(int x, int y, int z)
            : base (x, y, z)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChunkLocation))
                return false;

            ChunkLocation loc = (ChunkLocation)obj;

            if (loc.X == X && loc.Y == Y && loc.Z == Z)
                return true;

            return false;
        }

    }
}
