using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftServer
{
    public class ChatColor
    {
        public static ChatColor Black = new ChatColor('0');
        public static ChatColor DarkBlue = new ChatColor('1');
        public static ChatColor DarkGreen = new ChatColor('2');
        public static ChatColor DarkCyan = new ChatColor('3');
        public static ChatColor DarkRed = new ChatColor('4');
        public static ChatColor Purple = new ChatColor('5');
        public static ChatColor Gold = new ChatColor('6');
        public static ChatColor Gray = new ChatColor('7');
        public static ChatColor DarkGray = new ChatColor('8');
        public static ChatColor Blue = new ChatColor('9');
        public static ChatColor BrightGreen = new ChatColor('a');
        public static ChatColor Cyan = new ChatColor('b');
        public static ChatColor Red = new ChatColor('c');
        public static ChatColor Pink = new ChatColor('d');
        public static ChatColor Yellow = new ChatColor('e');
        public static ChatColor White = new ChatColor('f');

        private char ColorCode;

        private ChatColor(char code)
        {
            ColorCode = code;
        }

        public override String ToString()
        {
            return "\u00A7" + ColorCode;
        }
    }
}
