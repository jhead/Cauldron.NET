using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer.Net
{
    public class Packet
    {

        public PacketType Type { get; set; }
        public int Length { get; set; }
        
        private byte[] Data_ { get; set; }
        private byte[] Data
        {
            get
            {
                return Data_;
            }
            set
            {
                Data_ = value;
                Length = value.Length;
            }
        }

        public Packet(PacketType type)
        {
            Type = type;
            Data = new byte[0];
        }

        public Packet(int id)
        {
            Type = (PacketType)((byte)id);
        }

        public byte[] GetData()
        {
            return Data;
        }

    }

    public class KeepAlivePacket : Packet
    {
        public KeepAlivePacket()
            : base(PacketType.KeepAlive)
        { }
    }

    public class LoginRequestPacket : Packet
    {
        public int ProtocolVersion;
        public String Username;
        public long MapSeed;
        public byte Dimension;

        public LoginRequestPacket()
            : base(PacketType.Login)
        { }
    }

    public class LoginResponsePacket : Packet
    {
        public int EntityID;
        public String ServerName;
        public long MapSeed;
        public byte Dimension;

        public LoginResponsePacket(int entityID, String serverName, long mapSeed, byte dimension)
            : base(PacketType.Login)
        {
            EntityID = entityID;
            ServerName = serverName;
            MapSeed = mapSeed;
            Dimension = dimension;
        }
    }

    public class HandshakeRequestPacket : Packet
    {
        public String Username;

        public HandshakeRequestPacket()
            : base(PacketType.Handshake)
        { }
    }

    public class HandshakeResponsePacket : Packet
    {
        public String Hash;

        public HandshakeResponsePacket(String hash)
            : base(PacketType.Handshake)
        {
            Hash = hash;
        }
    }

    public class PlayerPositionPacket : Packet
    {
        public Double X;
        public Double Y;
        public Double Z;
        public Double Stance;
        public Boolean OnGround;

        // Optional param allows PlayerPositionLookPacket to extend this
        public PlayerPositionPacket(PacketType type = PacketType.PlayerPosition)
            : base(type)
        {
        }
    }

    public class PlayerPositionLookPacket : PlayerPositionPacket
    {
        public float Yaw;
        public float Pitch;

        public PlayerPositionLookPacket()
            : base(PacketType.PlayerPositionLook)
        {
        }
    }

    public class PreChunkPacket : Packet
    {
        public int X;
        public int Z;
        public Boolean Mode; // false = unload | true = load

        public PreChunkPacket(int x, int z, Boolean mode)
            : base(PacketType.PreChunk)
        {
            X = x;
            Z = z;
            Mode = mode;
        }
    }

    public class MapChunkPacket : Packet
    {
        public int X;
        public short Y;
        public int Z;
        public byte SizeX;
        public byte SizeY;
        public byte SizeZ;
        public int CompressedSize;
        public byte[] ChunkData;

        public MapChunkPacket()
            : base(PacketType.MapChunk)
        {
        }
    }

    public class SpawnPositionPacket : Packet
    {
        public int X;
        public int Y;
        public int Z;

        public SpawnPositionPacket(int x, int y, int z)
            : base(PacketType.SpawnPosition)
        {
            X = x;
            Y = y;
            Z = y;
        }
    }

    public class ChatMessagePacket : Packet
    {
        public String Message;
        public String Username; 

        public ChatMessagePacket(String username, String message)
            : base(PacketType.ChatMessage)
        {
            Username = username;
            Message = message;
        }

    }

    public class PlayerDiggingPacket : Packet
    {
        public byte DigStatus;
        public int DigX;
        public byte DigY;
        public int DigZ;
        public int DigFace;

        public PlayerDiggingPacket()
            : base(PacketType.PlayerDigging)
        {
        }
    }

    public class BlockChangePacket : Packet
    {
        public int X;
        public byte Y;
        public int Z;
        public byte Type;
        public byte Metadata;

        public BlockChangePacket(int x, byte y, int z, byte type, byte metadata)
            : base(PacketType.BlockChange)
        {
            X = x;
            Y = y;
            Z = z;
            Type = type;
            Metadata = metadata;
        }
    }

    public class SetSlotPacket : Packet
    {
        public byte WindowID;
        public short Slot;
        public short ItemID;
        public byte ItemCount;
        public short ItemUses;

        public SetSlotPacket(byte windowid, short slot, short itemid, byte itemcount, short itemuses)
            : base(PacketType.SetSlot)
        {
            WindowID = windowid;
            Slot = slot;
            ItemID = itemid;
            ItemCount = itemcount;
            ItemUses = itemuses;
        }
    }

    public class PlayerBlockPlacementPacket : Packet
    {
        public int X;
        public byte Y;
        public int Z;
        public byte Direction;
        public short BlockID;
        public byte Amount;
        public short Damage;

        public PlayerBlockPlacementPacket()
            : base(PacketType.PlayerBlockPlacement)
        {
        }

    }

    public class PickupSpawnPacket : Packet
    {
        public int EntityID;
        public short ItemID;
        public byte Count;
        public short Damage;
        public int X, Y, Z;
        public byte Rotation, Pitch, Roll;

        public PickupSpawnPacket(int eid, short iid, byte count, short damage, int x, int y, int z, byte rot, byte pitch, byte roll)
            : base(PacketType.PickupSpawn)
        {
            EntityID = eid;
            ItemID = iid;
            Count = count;
            Damage = damage;
            X = x;
            Y = y;
            Z = z;
            Rotation = rot;
            Pitch = pitch;
            Roll = roll;
        }
    }

    public class EntityPacket : Packet
    {
        public int EntityID;

        public EntityPacket(int eid)
            : base(PacketType.Entity)
        {
            EntityID = eid;
        }
    }

    public class NamedEntitySpawnPacket : Packet
    {
        public int EntityID;
        public String Name;
        public int X, Y, Z;
        public byte Rotation, Pitch;
        public short Item;

        public NamedEntitySpawnPacket(int eid, String name, int x, int y, int z, byte rot, byte pitch, short item)
            : base(PacketType.NamedEntitySpawn)
        {
            EntityID = eid;
            Name = name;
            X = x;
            Y = y;
            Z = z;
            Rotation = rot;
            Pitch = pitch;
            Item = item;
        }
    }

    public class DestroyEntityPacket : Packet
    {
        public int EntityID;

        public DestroyEntityPacket(int eid)
            : base(PacketType.DestroyEntity)
        {
            EntityID = eid;
        }
    }

    public class EntityLookRelativeMovePacket : Packet
    {
        public int EntityID;
        public byte X, Y, Z;
        public byte Yaw, Pitch;

        public EntityLookRelativeMovePacket(int eid, byte x, byte y, byte z, byte yaw, byte pitch)
            : base(PacketType.EntityLookRelativeMove)
        {
            EntityID = eid;
            X = x;
            Y = y;
            Z = z;
            Yaw = yaw;
            Pitch = pitch;
        }
    }

    public class DisconnectPacket : Packet
    {
        public String Reason;

        public DisconnectPacket()
            : base(PacketType.Disconnect)
        {
        }
    }

    public enum PacketType : byte
    {
        KeepAlive = 0x00,
        Login = 0x01,
        Handshake = 0x02,
        ChatMessage = 0x03,
        TimeUpdate = 0x04,
        EntityEquipment = 0x05,
        SpawnPosition = 0x06,
        UseEntity = 0x07,
        UpdateHealth = 0x08,
        Respawn = 0x09,
        Player = 0x0A,
        PlayerPosition = 0x0B,
        PlayerLook = 0x0C,
        PlayerPositionLook = 0x0D,
        PlayerDigging = 0x0E,
        PlayerBlockPlacement = 0x0F,
        HoldingChange = 0x10,
        UseBed = 0x11,
        Animation = 0x12,
        EntityAction = 0x13,
        NamedEntitySpawn = 0x14,
        PickupSpawn = 0x15,
        CollectItem = 0x16,
        AddObject = 0x17,
        MobSpawn = 0x18,
        EntityPainting = 0x19,
        StanceUpdate = 0x1B,
        EntityVelocity = 0x1C,
        DestroyEntity = 0x1D,
        Entity = 0x1E,
        EntityRelativeMove = 0x1F,
        EntityLook = 0x20,
        EntityLookRelativeMove = 0x21,
        EntityTeleport = 0x22,
        EntityStatus = 0x26,
        AttachEntity = 0x27,
        EntityMetadata = 0x28,
        PreChunk = 0x32,
        MapChunk = 0x33,
        MultiBlockChange = 0x34,
        BlockChange = 0x35,
        BlockAction = 0x36,
        Explosion = 0x3C,
        SoundEffect = 0x3D,
        InvalidState = 0x46,
        Thunderbolt = 0x47,
        OpenWindow = 0x64,
        CloseWindow = 0x65,
        WindowClick = 0x66,
        SetSlot = 0x67,
        WindowItems = 0x68,
        UpdateProgressBar = 0x69,
        Transaction = 0x6A,
        UpdateSign = 0x82,
        ItemData = 0x83,
        IncrementStatistic = 0xC8,
        Disconnect = 0xFF
    }
}
