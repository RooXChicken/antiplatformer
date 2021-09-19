using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TGUI;

namespace antiplatformer
{   
    public class gameManager
    {
        public static string GAME_VERSION = "A-0.2.3";
        public static Vector2f GAME_INTERNAL_RESOLUTION = new Vector2f(256, 144);
        public static Clock GAME_GAME_TIME = new Clock();
        public static int GAME_TICKS = 0;
        public static bool isLoading = true;
        public static int GAME_STATE = 0;
        public static bool GAME_HAS_FOCUS = true;
        public static discordRPC drpc = new discordRPC();

        public static Clock framerate_clock = new Clock();
        public static Clock deltaClock = new Clock();
        public static float deltaTime;
        private static float framerate_lastTime = 0;
        private static float fps;

        public static sceneEditor se = new sceneEditor();

        //for function keys
        private static bool f11Pressed = false;
        private static bool isFullscreen = false;
        private static bool f2Pressed = false;
        private static bool f5Pressed = false;
        private static bool f3Pressed = false;
        private static bool f7Pressed = false;
        public static bool debugOpen = false;

        public static void handleArgs(string[] args)
        {
            if (args.Length > 0)
            {
                bool wasArg = false;
                foreach (string arg in args)
                {
                    if (arg == "-debug" || arg == "-dev" || arg == "-developer")
                    {
                        Game.GAME_DEBUG = true;
                        wasArg = true;
                    }
                    if (arg == "-help")
                    {
                        utils.Log("---------------------\nAll current arguments are:\n-help | Shows this message\n-stop | Stops the game as soon as it starts\n-debug/-dev/-developer | Gives access to the advanced debugging features\n---------------------");
                        wasArg = true;
                    }
                    if (arg == "-skipSplash")
                    {
                        GAME_STATE = 2;
                        wasArg = true;
                    }
                    if (arg == "")
                    {
                        wasArg = true;
                    }
                }

                if (!wasArg)
                {
                    utils.LogWarn("That was not a command line argument! Please try -help to get a list of arguments!");
                }
            }
        }

        public static void update()
        {
            if(!Game.GAME_DEBUG && GAME_STATE == 99)
            {
                GAME_STATE = 3;
            }

            deltaTime = deltaClock.Restart().AsSeconds();
            GAME_TICKS++;

            float currentTime = framerate_clock.Restart().AsSeconds();
            if (GAME_GAME_TIME.ElapsedTime.AsMilliseconds() % 125 == 0)
            {
                fps = 1 / currentTime;
            }
            framerate_lastTime = currentTime;
        }

        public static float getFPS() { return fps; }

        public static void handleEvents(RenderWindow renderWindow)
        {
            renderWindow.DispatchEvents();
            renderWindow.Closed += (sender, e) => { ((Window)sender).Close(); };
            renderWindow.LostFocus += (sender, e) => { gameManager.GAME_HAS_FOCUS = false; ((RenderWindow)sender).SetFramerateLimit(settings.unfocused_fps); };
            renderWindow.GainedFocus += (sender, e) => { gameManager.GAME_HAS_FOCUS = true; ((RenderWindow)sender).SetFramerateLimit(settings.max_fps); };

            #region function keys

            if (Keyboard.IsKeyPressed(Keyboard.Key.F11) && !f11Pressed)
            {
                f11Pressed = true;
                if(isFullscreen)
                {
                    isFullscreen = false;
                    renderWindow.Close();
                    renderWindow = new RenderWindow(Game.videomode, "The anti-Platformer V" + GAME_VERSION);
                    renderWindow.SetFramerateLimit(settings.max_fps);
                    ui.gui.Target = renderWindow;
                    deltaTime = 0;
                    deltaClock.Restart();
                }
                else
                {
                    //hasFocus = false;
                    isFullscreen = true;
                    renderWindow.Close();
                    renderWindow = new RenderWindow(VideoMode.DesktopMode, "...this is a fullscreen window how are you seeing this", Styles.Fullscreen);
                    renderWindow.SetFramerateLimit(settings.max_fps);
                    ui.gui.Target = renderWindow;
                    deltaTime = 0;
                    deltaClock.Restart();
                }
            }
            if (!Keyboard.IsKeyPressed(Keyboard.Key.F11) && f11Pressed)
            {
                f11Pressed = false;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.F2) && !f2Pressed)
            {
                f2Pressed = true;
                Image screenshot = renderWindow.Capture();
                try
                {
                    Directory.CreateDirectory("screenshots");
                    string path = "screenshots/" + DateTime.Now.ToFileTime() + DateTime.Now.Millisecond + ".png";
                    screenshot.SaveToFile(path);
                    utils.Log("Saved a screenshot to: " + path);
                }
                catch
                {
                    utils.LogError("Failed to save a screenshot!");
                }

                deltaTime = 0;
                deltaClock.Restart();
            }
            if (!Keyboard.IsKeyPressed(Keyboard.Key.F2) && f2Pressed)
            {
                f2Pressed = false;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.F5) && !f5Pressed)
            {
                f5Pressed = true;
                Game.GAME_SCENE_MANAGER.reloadScene();
            }
            if (!Keyboard.IsKeyPressed(Keyboard.Key.F5) && f5Pressed)
            {
                f5Pressed = false;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.F3) && !f3Pressed)
            {
                f3Pressed = true;
                debugOpen = !debugOpen;
            }
            if (!Keyboard.IsKeyPressed(Keyboard.Key.F3) && f3Pressed)
            {
                f3Pressed = false;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.F7) && !f7Pressed && Game.GAME_DEBUG)
            {
                f7Pressed = true;
                if(GAME_STATE == 99)
                {
                    se.exit();
                    GAME_STATE = 3;
                }
                else
                {
                    se.init();
                    GAME_STATE = 99;
                }
            }
            if (!Keyboard.IsKeyPressed(Keyboard.Key.F7) && f7Pressed)
            {
                f7Pressed = false;
            }

            #endregion
        }

        public static void openUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}