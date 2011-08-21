using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer
{
    public struct Location
    {
        public double X;
        public double Y;
        public double Z;

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

        public static double Distance(Location loc1, Location loc2)
        {
            return Math.Sqrt(Math.Pow(loc1.X - loc2.X, 2) +
                        Math.Pow(loc1.Y - loc2.Y, 2) +
                        Math.Pow(loc1.Z - loc2.Z, 2));
        }

    }

    public struct ChunkLocation
    {
        public int X;
        public int Z;
        public int Y;

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

    public struct WorldLocation
    {
        public int X;
        public int Z;
        public int Y;

        public static WorldLocation Zero = new WorldLocation(0, 0, 0);

        public WorldLocation(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WorldLocation))
                return false;

            WorldLocation loc = (WorldLocation)obj;

            if (loc.X == X && loc.Y == Y && loc.Z == Z)
                return true;

            return false;
        }

    }
}
