using System;
using System.IO;

namespace antiplatformer
{
    public class Logger
    {
        private static string LogPath = "ERROR.txt";

        public static void Init()
        {
            Directory.CreateDirectory("logs");
            LogPath = "logs/log" + new Random().Next() + ".txt";
        }

        public static void Log(string message)
        {
            string msg = Manager.uptime.ElapsedTime.AsMilliseconds().ToString() + "[INFO] " + message;
            Console.WriteLine(msg);

            try
            {
                using (StreamWriter sw = File.AppendText(LogPath))
                {
                    sw.WriteLine(msg);
                }
            }
            catch (Exception e)
            {
                LogError("Failed to write to the logs, exception: " + e);
            }
        }

        public static void LogWarn(string message)
        {
            string msg = Manager.uptime.ElapsedTime.AsMilliseconds().ToString() + "[WARN] " + message;
            Console.WriteLine(msg);

            try
            {
                using (StreamWriter sw = File.AppendText(LogPath))
                {
                    sw.WriteLine(msg);
                }
            }
            catch (Exception e)
            {
                LogError("Failed to write to the logs, exception: " + e);
            }
        }

        public static void LogError(string message)
        {
            string msg = Manager.uptime.ElapsedTime.AsMilliseconds().ToString() + "[ERROR] " + message;
            Console.WriteLine(msg);

            try
            {
                using (StreamWriter sw = File.AppendText(LogPath))
                {
                    sw.WriteLine(msg);
                }
            }
            catch (Exception e)
            {
                LogError("Failed to write to the logs, exception: " + e);
            }
        }

        public static void LogFatal(string message)
        {
            string msg = Manager.uptime.ElapsedTime.AsMilliseconds().ToString() + "[FATAL] " + message;
            Console.WriteLine(msg);

            try
            {
                using (StreamWriter sw = File.AppendText(LogPath))
                {
                    sw.WriteLine(msg);
                }
            }
            catch (Exception e)
            {
                LogError("Failed to write to the logs, exception: " + e);
            }
        }
    }
}