using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MinecraftServer.World
{
    public class WorldManager
    {

        public Server Server;
        public Dictionary<String, World> Worlds;

        public static String WorldPath = "Worlds";

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

        public void Init()
        {
            if (!Directory.Exists(WorldPath))
            {
                Directory.CreateDirectory(WorldPath);
                return;
            }

            String[] existingWorlds = Directory.GetDirectories(WorldPath);

            if (existingWorlds.Length == 0)
            {
                Logger.Info("No existing worlds found in " + WorldPath + "/");
                return;
            }

            World w;
            foreach (String worldDir in existingWorlds)
            {
                if (!File.Exists(worldDir + "/world.dat"))
                    continue;

                w = new World(this, worldDir.Replace(WorldPath + "\\", ""));
                w.Load();
                AddWorld(w);
            }
        }

    }
}
