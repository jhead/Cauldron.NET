using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer.World
{
    public class WorldManager
    {

        public Server Server;
        public Dictionary<String, World> Worlds;

        public WorldManager(Server server)
        {
            Server = server;
            Worlds = new Dictionary<String, World>();
        }

        public World GetWorld(int index)
        {
            int i = 0;
            foreach (World w in Worlds.Values)
            {
                if (i == index)
                    return w;

                i++;
            }

            return null;
        }

        public World GetWorld(String worldName)
        {
            return Worlds[worldName];
        }

        public World AddWorld(World world)
        {
            Worlds.Add(world.Name, world);
            return world;
        }

    }
}
