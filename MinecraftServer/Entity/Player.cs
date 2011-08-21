using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer.Entity
{
    public class Player : Entity
    {

        public String Username { get; set; }
        public Location Location { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }

        public Player()
        {
            Location = new Location(0d, 0d, 0d);
        }

    }
}
