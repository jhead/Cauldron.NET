using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MinecraftServer
{
    public class Logger
    {

        public static StreamWriter LogWriter = new StreamWriter(new FileStream("server.log", FileMode.OpenOrCreate));
        public static Boolean EnableDebug = true;

        public static String GetTimestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        public static void Log(LogLevel level, Object o)
        {
            Console.WriteLine("{0} [{1}] {2}", GetTimestamp(), level, o.ToString());
            LogWriter.WriteLine("{0} [{1}] {2}", GetTimestamp(), level, o.ToString());

            if (o is Exception)
            {
                Console.WriteLine(((Exception)o).StackTrace);
                LogWriter.WriteLine(((Exception)o).StackTrace);
            }

            LogWriter.Flush();
        }

        public static void Debug(Object o)
        {
            if(EnableDebug)
                Log(LogLevel.DEBUG, o);
        }

        public static void Info(Object o)
        {
            Log(LogLevel.INFO, o);
        }

        public static void Warn(Object o)
        {
            Log(LogLevel.WARN, o);
        }

        public static void Error(Object o)
        {
            Log(LogLevel.ERROR, o);
        }

        public static void Fatal(Object o)
        {
            Log(LogLevel.FATAL, o);
        }

        public static void Chat(String s)
        {
            Log(LogLevel.CHAT, s);
        }

    }

    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL,
        CHAT
    }
}
