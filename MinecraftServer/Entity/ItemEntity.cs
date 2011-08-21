using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinecraftServer.World;

namespace MinecraftServer.Entity
{
    public class ItemEntity : Entity
    {

        public BlockType Type;

        public ItemEntity(int entityID, BlockType type)
        {
            EntityID = entityID;
            Type = type;
        }

    }
}
