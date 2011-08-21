using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace MinecraftServer.Net
{
    public class PacketStream
    {

        private NetworkStream Stream;

        public Boolean Closed { get; set; }

        public PacketStream(NetworkStream stream)
        {
            Stream = stream;
        }

        public void Dispose()
        {
            Stream.Dispose();
            Closed = true;
        }

        public void WritePacket(Packet packet)
        {
            if (!Stream.CanWrite || Closed)
                return;

            try
            {
                Stream.WriteByte((byte)packet.Type);

                switch (packet.Type)
                {
                    case PacketType.KeepAlive:
                        break;

                    case PacketType.Login:
                        Stream.WriteInt(((LoginResponsePacket)packet).EntityID);
                        Stream.WriteString16(((LoginResponsePacket)packet).ServerName);
                        Stream.WriteLong(((LoginResponsePacket)packet).MapSeed);
                        Stream.WriteByte(((LoginResponsePacket)packet).Dimension);
                        break;

                    case PacketType.Handshake:
                        Stream.WriteString16(((HandshakeResponsePacket)packet).Hash);
                        break;

                    case PacketType.PlayerPositionLook:
                        Stream.WriteDouble(((PlayerPositionLookPacket)packet).X);
                        Stream.WriteDouble(((PlayerPositionLookPacket)packet).Y);
                        Stream.WriteDouble(((PlayerPositionLookPacket)packet).Stance);
                        Stream.WriteDouble(((PlayerPositionLookPacket)packet).Z);
                        Stream.WriteFloat(((PlayerPositionLookPacket)packet).Yaw);
                        Stream.WriteFloat(((PlayerPositionLookPacket)packet).Yaw);
                        Stream.WriteBoolean(((PlayerPositionLookPacket)packet).OnGround);
                        break;

                    case PacketType.PreChunk:
                        Stream.WriteInt(((PreChunkPacket)packet).X);
                        Stream.WriteInt(((PreChunkPacket)packet).Z);
                        Stream.WriteBoolean(((PreChunkPacket)packet).Mode);
                        break;

                    case PacketType.MapChunk:
                        Stream.WriteInt(((MapChunkPacket)packet).X);
                        Stream.WriteShort(((MapChunkPacket)packet).Y);
                        Stream.WriteInt(((MapChunkPacket)packet).Z);
                        Stream.WriteByte(((MapChunkPacket)packet).SizeX);
                        Stream.WriteByte(((MapChunkPacket)packet).SizeY);
                        Stream.WriteByte(((MapChunkPacket)packet).SizeZ);
                        Stream.WriteInt(((MapChunkPacket)packet).CompressedSize);
                        Stream.Write(((MapChunkPacket)packet).ChunkData, 0, ((MapChunkPacket)packet).CompressedSize);
                        break;

                    case PacketType.SpawnPosition:
                        Stream.WriteInt(((SpawnPositionPacket)packet).X);
                        Stream.WriteInt(((SpawnPositionPacket)packet).Y);
                        Stream.WriteInt(((SpawnPositionPacket)packet).Z);
                        break;

                    case PacketType.ChatMessage:
                        if (((ChatMessagePacket)packet).Username.Length > 0)
                            Stream.WriteString16(String.Format("<{0}> {1}", ((ChatMessagePacket)packet).Username, ((ChatMessagePacket)packet).Message));
                        else
                            Stream.WriteString16(((ChatMessagePacket)packet).Message);
                        break;

                    case PacketType.BlockChange:
                        Stream.WriteInt(((BlockChangePacket)packet).X);
                        Stream.WriteByte(((BlockChangePacket)packet).Y);
                        Stream.WriteInt(((BlockChangePacket)packet).Z);
                        Stream.WriteByte(((BlockChangePacket)packet).Type);
                        Stream.WriteByte(((BlockChangePacket)packet).Metadata);
                        break;

                    case PacketType.SetSlot:
                        Stream.WriteByte(((SetSlotPacket)packet).WindowID);
                        Stream.WriteShort(((SetSlotPacket)packet).Slot);
                        Stream.WriteShort(((SetSlotPacket)packet).ItemID);
                        Stream.WriteByte(((SetSlotPacket)packet).ItemCount);
                        Stream.WriteShort(((SetSlotPacket)packet).ItemUses);
                        break;

                    case PacketType.PickupSpawn:
                        Stream.WriteInt(((PickupSpawnPacket)packet).EntityID);
                        Stream.WriteShort(((PickupSpawnPacket)packet).ItemID);
                        Stream.WriteByte(((PickupSpawnPacket)packet).Count);
                        Stream.WriteShort(((PickupSpawnPacket)packet).Damage);
                        Stream.WriteInt(((PickupSpawnPacket)packet).X);
                        Stream.WriteInt(((PickupSpawnPacket)packet).Y);
                        Stream.WriteInt(((PickupSpawnPacket)packet).Z);
                        Stream.WriteByte(((PickupSpawnPacket)packet).Rotation);
                        Stream.WriteByte(((PickupSpawnPacket)packet).Pitch);
                        Stream.WriteByte(((PickupSpawnPacket)packet).Roll);
                        break;

                    case PacketType.Entity:
                        Stream.WriteInt(((EntityPacket)packet).EntityID);
                        break;

                    case PacketType.NamedEntitySpawn:
                        Stream.WriteInt(((NamedEntitySpawnPacket)packet).EntityID);
                        Stream.WriteString16(((NamedEntitySpawnPacket)packet).Name);
                        Stream.WriteInt(((NamedEntitySpawnPacket)packet).X);
                        Stream.WriteInt(((NamedEntitySpawnPacket)packet).Y);
                        Stream.WriteInt(((NamedEntitySpawnPacket)packet).Z);
                        Stream.WriteByte(((NamedEntitySpawnPacket)packet).Rotation);
                        Stream.WriteByte(((NamedEntitySpawnPacket)packet).Pitch);
                        Stream.WriteShort(((NamedEntitySpawnPacket)packet).Item);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e);
                Dispose();
            }

            if(packet != null)
                Logger.Debug("Sent " + packet.Type + " packet");
        }

        public Packet ReadPacket()
        {
            if (!Stream.CanRead)
                throw new Exception("Cannot read packet from stream.");

            Packet packet = null;

            try
            {
                PacketType type = (PacketType)Stream.ReadByte();
                switch (type)
                {
                    case PacketType.KeepAlive:
                        break;

                    case PacketType.Login:
                        packet = new LoginRequestPacket();
                        ((LoginRequestPacket)packet).ProtocolVersion = Stream.ReadInt();
                        ((LoginRequestPacket)packet).Username = Stream.ReadString16();
                        ((LoginRequestPacket)packet).MapSeed = Stream.ReadLong();
                        ((LoginRequestPacket)packet).Dimension = (byte)Stream.ReadByte();
                        break;

                    case PacketType.Handshake:
                        packet = new HandshakeRequestPacket();
                        ((HandshakeRequestPacket)packet).Username = Stream.ReadString16();
                        break;

                    case PacketType.PlayerPosition:
                        packet = new PlayerPositionPacket();
                        ((PlayerPositionPacket)packet).X = Stream.ReadDouble(); // X
                        ((PlayerPositionPacket)packet).Stance = Stream.ReadDouble(); // Stance
                        ((PlayerPositionPacket)packet).Y = Stream.ReadDouble(); // Y
                        ((PlayerPositionPacket)packet).Z = Stream.ReadDouble(); // Z
                        ((PlayerPositionPacket)packet).OnGround = Stream.ReadBoolean(); // On-Ground
                        break;

                    case PacketType.PlayerLook:
                        // TODO
                        Stream.Read(new byte[9], 0, 9);
                        break;

                    case PacketType.Player:
                        // TODO
                        Stream.ReadBoolean();
                        break;

                    case PacketType.PlayerPositionLook:
                        packet = new PlayerPositionLookPacket();
                        ((PlayerPositionLookPacket)packet).X = Stream.ReadDouble(); // X
                        ((PlayerPositionLookPacket)packet).Stance = Stream.ReadDouble(); // Stance
                        ((PlayerPositionLookPacket)packet).Y = Stream.ReadDouble(); // Y
                        ((PlayerPositionLookPacket)packet).Z = Stream.ReadDouble(); // Z
                        ((PlayerPositionLookPacket)packet).Yaw = Stream.ReadFloat(); // Yaw
                        ((PlayerPositionLookPacket)packet).Pitch = Stream.ReadFloat(); // Pitch
                        ((PlayerPositionLookPacket)packet).OnGround = Stream.ReadBoolean(); // On-Ground
                        break;

                    case PacketType.Animation:
                        // TODO
                        Stream.ReadInt();
                        Stream.ReadByte();
                        break;

                    case PacketType.EntityAction:
                        // TODO
                        Stream.ReadInt();
                        Stream.ReadByte();
                        break;

                    case PacketType.PlayerDigging:
                        // TODO
                        packet = new PlayerDiggingPacket();
                        ((PlayerDiggingPacket)packet).DigStatus = (byte)Stream.ReadByte(); // Status (0 = Started, 2 = Finished, 4 = Drop Item)
                        ((PlayerDiggingPacket)packet).DigX = Stream.ReadInt();
                        ((PlayerDiggingPacket)packet).DigY = (byte)Stream.ReadByte();
                        ((PlayerDiggingPacket)packet).DigZ = Stream.ReadInt();
                        ((PlayerDiggingPacket)packet).DigFace = Stream.ReadByte();
                        break;

                    case PacketType.ChatMessage:
                        packet = new ChatMessagePacket("", Stream.ReadString16());
                        break;

                    case PacketType.HoldingChange:
                        // TODO
                        Stream.ReadShort(); // Slot ID
                        break;

                    case PacketType.PlayerBlockPlacement:
                        // TODO
                        packet = new PlayerBlockPlacementPacket();
                        
                        ((PlayerBlockPlacementPacket)packet).X = Stream.ReadInt(); // X
                        ((PlayerBlockPlacementPacket)packet).Y = (byte)Stream.ReadByte(); // Y
                        ((PlayerBlockPlacementPacket)packet).Z = Stream.ReadInt(); // Z
                        ((PlayerBlockPlacementPacket)packet).Direction = (byte)Stream.ReadByte(); // Direction
                        if ((((PlayerBlockPlacementPacket)packet).BlockID = Stream.ReadShort()) >= 0) // Block or Item ID
                        {
                            ((PlayerBlockPlacementPacket)packet).Amount = (byte)Stream.ReadByte(); // Amount
                            ((PlayerBlockPlacementPacket)packet).Damage = Stream.ReadShort(); // Damage
                        }
                        break;

                    case PacketType.WindowClick:
                        // TODO
                        Stream.ReadByte(); // Window ID
                        Stream.ReadShort(); // Slot
                        Stream.ReadByte(); // Right Click
                        Stream.ReadShort(); // Action Number    
                        Stream.ReadBoolean(); // Shift 
                        if (Stream.ReadShort() != -1) // Item ID
                        {
                            Stream.ReadByte(); // Item Count
                            Stream.ReadShort(); // Item Uses
                        }
                        break;

                    case PacketType.Disconnect:
                        packet = new DisconnectPacket();
                        ((DisconnectPacket)packet).Reason = Stream.ReadString16();
                        break;

                    default:
                        Logger.Debug("Unhandled data: " + type);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e);
            }

            if(packet != null)
                Logger.Debug("Received " + packet.Type + " packet");

            return packet;
        }

    }

    public static class StreamExtensions
    {

        public static short ReadShort(this Stream stream)
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, 0));
        }

        public static int ReadInt(this Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, 0));
        }

        public static long ReadLong(this Stream stream)
        {
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buffer, 0));
        }

        public static bool ReadBoolean(this Stream stream)
        {
            return (stream.ReadByte() == 1 ? true : false);
        }

        public static float ReadFloat(this Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            Array.Reverse(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }

        public static double ReadDouble(this Stream stream)
        {
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            Array.Reverse(buffer);
            return BitConverter.ToDouble(buffer, 0);
        }

        public static String ReadString8(this Stream stream)
        {
            int length = (int)StreamExtensions.ReadShort(stream);
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);

            return Encoding.UTF8.GetString(buffer);
        }

        public static String ReadString16(this Stream stream)
        {
            int length = ((int)StreamExtensions.ReadShort(stream)) * 2;
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);

            return Encoding.BigEndianUnicode.GetString(buffer);
        }

        public static void WriteInt(this Stream stream, int value)
        {
            byte[] buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteShort(this Stream stream, short value)
        {
            byte[] buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteLong(this Stream stream, long value)
        {
            byte[] buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteBoolean(this Stream stream, bool value)
        {
            stream.WriteByte((value ? (byte)0x01 : (byte)0x00));
        }

        public static void WriteFloat(this Stream stream, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteDouble(this Stream stream, double value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            stream.Write(buffer,0, buffer.Length);
        }

        public static void WriteString8(this Stream stream, String value)
        {
            byte[] buffer = new byte[value.Length + 2];
            short length = (short)value.Length;

            Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length)), 0, buffer, 0, 2);
            Array.Copy(Encoding.UTF8.GetBytes(value), 0, buffer, 2, 0);

            stream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteString16(this Stream stream, String value)
        {
            byte[] buffer = new byte[(value.Length * 2) + 2];
            short length = (short)value.Length;

            if (length > 100)
                length = 100;

            Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length)), 0, buffer, 0, 2);
            Array.Copy(Encoding.BigEndianUnicode.GetBytes(value), 0, buffer, 2, length * 2);

            stream.Write(buffer, 0, buffer.Length);
        }

    }
}
