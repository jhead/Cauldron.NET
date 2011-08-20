using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinecraftServer;
using MinecraftServer.World;

namespace MinecraftServer.Net
{
    public class PacketHandler
    {

        private Server Server;

        public PacketHandler(Server server)
        {
            Server = server;
        }

        public void HandlePacket(Packet packet, Client client)
        {
            if (!Server.Running || packet == null)
                return;

            try
            {
                switch (packet.Type)
                {
                    case PacketType.KeepAlive:
                        client.Stream.WritePacket(new KeepAlivePacket());
                        break;

                    case PacketType.Handshake:
                        if (client.LoggedIn)
                            break;
                        client.Stream.WritePacket(new HandshakeResponsePacket("-"));
                        Logger.Debug("New user '" + ((HandshakeRequestPacket)packet).Username + "' is connecting...");
                        break;

                    case PacketType.Login:
                        if (client.LoggedIn)
                            break;
                        client.Stream.WritePacket(new LoginResponsePacket(Server.TotalEntityCount + 1, Server.ServerName, 
                            Server.MapSeed, (byte)(Server.IsNether ? -1 : 0)));
                        client.LoggedIn = true;
                        client.Player.Username = ((LoginRequestPacket)packet).Username;
                        client.SendInitialChunks();
                        client.SendInitialPosition();
                        client.SendInitialInventory();
                        Logger.Debug("'" + ((LoginRequestPacket)packet).Username + "' logged in.");
                        break;

                    case PacketType.PlayerPosition:
                        // TODO: Check for speed/fly hax, going through walls, etc.

                        // Update player position
                        client.Player.Location.X = ((PlayerPositionPacket)packet).X;
                        client.Player.Location.Y = ((PlayerPositionPacket)packet).Y;
                        client.Player.Location.Z = ((PlayerPositionPacket)packet).Z;
                        break;

                    case PacketType.PlayerPositionLook:
                        // TODO: See above
                        // Update player position & look
                        client.Player.Location.X = ((PlayerPositionLookPacket)packet).X;
                        client.Player.Location.Y = ((PlayerPositionLookPacket)packet).Y;
                        client.Player.Location.Z = ((PlayerPositionLookPacket)packet).Z;
                        client.Player.Yaw = ((PlayerPositionLookPacket)packet).Yaw;
                        client.Player.Pitch = ((PlayerPositionLookPacket)packet).Pitch;
                        break;

                    case PacketType.ChatMessage:
                        ((ChatMessagePacket)packet).Username = client.Player.Username;
                        Logger.Info(String.Format("<{0}> {1}", ((ChatMessagePacket)packet).Username, ((ChatMessagePacket)packet).Message));
                        Server.BroadcastPacket(packet);
                        break;

                    case PacketType.PlayerDigging:
                        if (((PlayerDiggingPacket)packet).DigStatus == 2) // Finished Digging
                        {
                            Block b = Server.GetWorldManager().GetWorld(0).GetBlock(new WorldLocation(
                                ((PlayerDiggingPacket)packet).DigX, (int)((PlayerDiggingPacket)packet).DigY, ((PlayerDiggingPacket)packet).DigZ));
                            b.BreakBlock();
                        }
                        break;

                    case PacketType.PlayerBlockPlacement:
                        if (((PlayerBlockPlacementPacket)packet).Amount >= 0)
                        {
                            int placeX = ((PlayerBlockPlacementPacket)packet).X;
                            int placeY = ((PlayerBlockPlacementPacket)packet).Y;
                            int placeZ = ((PlayerBlockPlacementPacket)packet).Z;

                            switch (((PlayerBlockPlacementPacket)packet).Direction)
                            {
                                case 0:
                                    placeY--;
                                    break;
                                case 1:
                                    placeY++;
                                    break;
                                case 2:
                                    placeZ--;
                                    break;
                                case 3:
                                    placeZ++;
                                    break;
                                case 4:
                                    placeX--;
                                    break;
                                case 5:
                                    placeX++;
                                    break;
                            }

                            WorldLocation bLoc = new WorldLocation(placeX, placeY, placeZ);
                            Block b = Server.GetWorldManager().GetWorld(0).GetBlock(bLoc);
                            b.SetBlockType((BlockType)((PlayerBlockPlacementPacket)packet).BlockID);
                            Server.OnBlockChange(b);
                        }
                        break;

                    default:
                        Logger.Debug("Unhandled packet (" + packet.Type + ")");
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e);
            }
        }

    }
}
