using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinecraftServer;
using MinecraftServer.Entity;
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
                        HandleKeepAlive(client, (KeepAlivePacket)packet);
                        break;

                    case PacketType.Handshake:
                        HandleHandshake(client, (HandshakeRequestPacket)packet);
                        break;

                    case PacketType.Login:
                        HandleLogin(client, (LoginRequestPacket)packet);
                        break;

                    case PacketType.PlayerPosition:
                        HandlePlayerPosition(client, (PlayerPositionPacket)packet);
                        break;

                    case PacketType.PlayerPositionLook:
                        HandlePlayerPositionLook(client, (PlayerPositionLookPacket)packet);
                        break;

                    case PacketType.ChatMessage:
                        HandleChatMessage(client, (ChatMessagePacket)packet);
                        break;

                    case PacketType.PlayerDigging:
                        HandlePlayerDigging(client, (PlayerDiggingPacket)packet);
                        break;

                    case PacketType.PlayerBlockPlacement:
                        HandlePlayerBlockPlacement(client, (PlayerBlockPlacementPacket)packet);
                        break;

                    case PacketType.Disconnect:
                        HandleDisconnect(client, (DisconnectPacket)packet);
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

        private void HandleKeepAlive(Client client, KeepAlivePacket packet)
        {
            client.Stream.WritePacket(new KeepAlivePacket());
        }

        private void HandleHandshake(Client client, HandshakeRequestPacket packet)
        {
            if (client.LoggedIn)
                return;

            client.Stream.WritePacket(new HandshakeResponsePacket("-"));
            Logger.Debug("New user '" + packet.Username + "' is connecting...");
        }

        private void HandleLogin(Client client, LoginRequestPacket packet)
        {
            if (client.LoggedIn)
                return;

            client.Stream.WritePacket(new LoginResponsePacket(Server.TotalEntityCount + 1, Server.ServerName,
                Server.MapSeed, (byte)(Server.IsNether ? -1 : 0)));
            client.Player.Username = packet.Username;
            client.LoggedIn = true;
            client.SendInitialChunks();
            client.SendInitialPosition();
            client.SendInitialInventory();

            Server.BroadcastPacket(new ChatMessagePacket("", ChatColor.Yellow + packet.Username + " joined the game."));
            Server.BroadcastPacket(new NamedEntitySpawnPacket(client.Player.EntityID, client.Player.Username,
                (int)client.Player.Location.X, (int)client.Player.Location.Y, (int)client.Player.Location.Z, 0, 0, 0), client);
            Logger.Info("'" + packet.Username + "' joined the game.");
        }

        private void HandlePlayerPosition(Client client, PlayerPositionPacket packet)
        {
            // TODO: Check for speed/fly hacks, going through walls, etc.
            // Update player position
            client.Player.Location = new Location(packet.X, packet.Y, packet.Z);
        }

        private void HandlePlayerPositionLook(Client client, PlayerPositionLookPacket packet)
        {
            // TODO: See above
            // Update player position & look
            Location newLoc = new Location(packet.X, packet.Y, packet.Z);

            // if (Location.Distance(client.Player.Location, newLoc) > 15 && client.Spawned)
            //     client.Dispose();

            client.Player.Location = newLoc;
            client.Player.Yaw = packet.Yaw;
            client.Player.Pitch = packet.Pitch;

            if (!client.Spawned)
                client.Spawned = true;
        }

        private void HandleChatMessage(Client client, ChatMessagePacket packet)
        {
            // TODO: Commands, spam filter, etc.
            packet.Username = client.Player.Username;
            Server.BroadcastPacket(packet);
            Logger.Chat(String.Format("<{0}> {1}", packet.Username, packet.Message));
        }

        private void HandlePlayerDigging(Client client, PlayerDiggingPacket packet)
        {
            if (!client.LoggedIn)
                client.Dispose();

            // TODO: Check if player is close enough to break the block; implement Start Digging and Drop statuses
            if (packet.DigStatus == 2) // Finished Digging
            {
                Nullable<Block> nb = Server.GetWorldManager().GetWorld(0).GetBlock(new WorldLocation(packet.DigX, (int)packet.DigY, packet.DigZ));
                if (nb != null)
                {
                    Block b = (Block)nb;

                    if (Location.Distance(new Location(b.Location.X, b.Location.Y, b.Location.Z), client.Player.Location) > 5)
                        return;

                    ItemEntity dropEntity = new ItemEntity(Server.TotalEntityCount++, b.GetBlockType());
                    dropEntity.Location = new Location(b.Location.X, b.Location.Y, b.Location.Z);
                    Server.GetWorldManager().GetWorld(0).AddEntity(dropEntity);

                    Server.BroadcastPacket(new PickupSpawnPacket(dropEntity.EntityID, (short)dropEntity.Type, 1, 0,
                        (int)dropEntity.Location.X, (int)dropEntity.Location.Y, (int)dropEntity.Location.Z, 0, 0, 0));
                    b.SetBlockType(BlockType.Air);
                    Server.BroadcastPacket(new EntityPacket(dropEntity.EntityID));

                    Server.OnBlockChange(b);
                }
            }
        }

        private void HandlePlayerBlockPlacement(Client client, PlayerBlockPlacementPacket packet)
        {
            // TODO: Check if player is close enough to place the block; check if player is placing on top of another block, etc.
            if (packet.Amount >= 0 && packet.BlockID > 0)
            {
                int placeX = packet.X;
                int placeY = packet.Y;
                int placeZ = packet.Z;

                switch (packet.Direction)
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
                Nullable<Block> nb = Server.GetWorldManager().GetWorld(0).GetBlock(bLoc);
                if (nb != null)
                {
                    Block b = (Block)nb;
                    b.SetBlockType((BlockType)packet.BlockID);
                    Server.OnBlockChange(b);
                }
            }
        }

        private void HandleDisconnect(Client client, DisconnectPacket packet)
        {
            if (client.LoggedIn && client.Player.Username.Length > 0)
            {
                Server.BroadcastPacket(new ChatMessagePacket("", ChatColor.Yellow + client.Player.Username + " left the game."));
                Logger.Info(client.Player.Username + " left the game. (" + packet.Reason + ")");
            }

            client.Dispose();
        }

    }
}
