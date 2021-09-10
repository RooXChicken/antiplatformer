using System;
using System.IO;
using SFML.Graphics;

namespace antiplatformer
{
    public static class utils
    {
        public static string logPath = "logs/log" + new Random().Next() + ".txt";

        public static void Log(string message)
        {
            Console.WriteLine(Game.GAME_NUMBER_TICKS.ToString() + "[INFO] " + message);

            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine(Game.GAME_NUMBER_TICKS.ToString() + "[INFO] " + message);
                }
            }
            catch (Exception e)
            {
                utils.LogError("Failed to write to the logs, exception: " + e);
            }
        }

        public static void LogNoConsole(string message)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine(Game.GAME_NUMBER_TICKS.ToString() + "[INFO] " + message);
                }
            }
            catch (Exception e)
            {
                utils.LogError("Failed to write to the logs, exception: " + e);
            }
        }

        public static void LogWarn(string message)
        {
            Console.WriteLine(Game.GAME_NUMBER_TICKS.ToString() + "[WARN] " + message);

            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine(Game.GAME_NUMBER_TICKS.ToString() + "[WARN] " + message);
                }
            }
            catch (Exception e)
            {
                utils.LogError("Failed to write to the logs, exception: " + e);
            }
        }

        public static void LogError(string message)
        {
            Console.WriteLine(Game.GAME_NUMBER_TICKS.ToString() + "[ERROR] " + message);

            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine(Game.GAME_NUMBER_TICKS.ToString() + "[ERROR] " + message);
                }
            }
            catch (Exception e)
            {
                utils.LogError("Failed to write to the logs, exception: " + e);
            }
        }

        public static void LogFatal(string message, bool canRecover)
        {
            Console.WriteLine(Game.GAME_NUMBER_TICKS.ToString() + "[FATAL] " + message);
            if(canRecover)
            {
                try
                {
                    using (StreamWriter sw = File.AppendText(logPath))
                    {
                        sw.WriteLine(Game.GAME_NUMBER_TICKS.ToString() + "[FATAL] " + message);
                    }
                }
                catch (Exception e)
                {
                    utils.LogError("Failed to write to the logs, exception: " + e);
                }
            } else
            {
                try
                {
                    using (StreamWriter sw = File.AppendText(logPath))
                    {
                        sw.WriteLine(Game.GAME_NUMBER_TICKS.ToString() + "[FATAL] " + message + " | Can't recover!");
                    }
                }
                catch (Exception e)
                {
                    utils.LogError("Failed to write to the logs, exception: " + e);
                }
            }
        }

        public static Sprite loadSprite(string path)
        {
            Texture tex;
            try
            {
                tex = new Texture(path);
                return new Sprite(tex);
            }
            catch
            {
                utils.LogError("Texture with path: " + path + " does not exist! Check the directory! Loading the missing texture sprite as a backup");
                try
                {
                    tex = new Texture("res/missing.png");
                    return new Sprite(tex);
                }
                catch
                {
                    utils.LogFatal("You are missing the missing texture sprite, what on earth did you do!?!? like come on dude at the least leave that! im going to return the window icon, so say hi to roos head :)", true);
                    return new Sprite(new Texture("res/icon.png"));
                }
            }
        }
    }
}
