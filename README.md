dv90's C# Minecraft Server
==========================

I currently don't have a fancy name for it; deal with it.

If you would like to help with the project: Add/Send me a message on skype (Username is 'woahitsjustin') or join irc.pwnage.me #chat and mention 'dv90'.

This project was started on: August 18th, 2011.

Current Features
----------------

* Basic World Manager / World / Chunk / Block Implementations
* Generates basic flat chunks (Grass -> Dirt -> Bedrock)
* Players can place and break blocks (No item drops yet)
* Basic chat implemented (No commands yet)
* Saves/Loads worlds to and from file (Separate format from mcregion)

Planned Features (Soon)
-----------------------

* ~Entity support (Both player and item entities)
* Commands (Chat + Console + WebUI [Later])
* More item/block types (Currently only supports 0-7 [Air-Bedrock])
* ~Complete protocol implementation
* Spawn protection
* ~Load chunks as players move
* ~Spam/overflow protection
* ~Fly/Speedhack protection (allow with config file)
* Configuration file
* Water/Lava/Fire spread and other tick-based operations (Time)

'~' Symbol denotes features currently in the process of being implemented.

Planned Features (Later)
------------------------

* Fast and complete world generation based on selectable (and customizable) world generators
* Load worlds from official Minecraft server world files (mcregion)
* Simple yet powerful plugin system with support for multiple types of plugins (languages); eg. .NET (dll), Java (jar) + Loadable scripts (+ easy reloading/editing for developers)
* Multi-World support and commands for creating/managing worlds on-the-fly
* Multi-Server support; link servers together, across worlds
* Built-In permissions/group system for players
* Built-In grief prevention, block (Chest/Door/Etc.) protection, regions
* Automatic updater to keep the server running the latest version; update without shutting down server, and disconnecting players
* Full web interface for managing the server online
* Integrated multi-server IRC supporty
* Link Nether world to normal worlds through bedrock lower layer: Players can fall through the "void" and appear in the Nether.
* Toggable global/region PVP
* Complete biome/world managment API (Commands and Web UI); Change biomes instantly and with ease, quickly regenerate chunks, and even entire worlds, without disconnecting players
* Built-In inventory editing for server administrators through commands and the Web UI