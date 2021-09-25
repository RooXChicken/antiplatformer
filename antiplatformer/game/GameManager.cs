using System;
using System.IO;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace antiplatformer
{
    public class Manager
    {
        public static float GAME_TIME_SCALE = 1f; //WILL MAKE THINGS SLOWER OR FASTER. this is **EVERYTHING** that uses deltatime. be careful!
        public static Font font = new Font("res/fonts/font.ttf");
        public static bool GAME_HAS_FOCUS = true;
        public static Clock uptime = new Clock();
        private static Clock framerate_clock = new Clock();
        private static Clock deltaClock = new Clock();
        private static float deltaTime;
        private static float framerate_lastTime = 0;
        private static float fps;
        public static bool paused = false;
        public static bool sceneRenderType = true;

        public static Random random = new Random((int)DateTime.Now.ToFileTime() + DateTime.Now.Millisecond);

        //for function keys
        public static bool escapePressed = false;
        private static bool f11Pressed = false;
        private static bool isFullscreen = false;
        private static bool f2Pressed = false;
        private static bool f5Pressed = false;
        private static bool f3Pressed = false;
        private static bool f7Pressed = false;
        public static bool debugOpen = false;

        public static bool FlipBool(bool item)
        {
            if(item)
                item = false;
            else
                item = true;

            return item;
        }

        public static void Update(RenderWindow window)
        {
            Events(window);

            deltaTime = deltaClock.Restart().AsSeconds();
            float currentTime = framerate_clock.Restart().AsSeconds();
            fps = 1 / currentTime;
            framerate_lastTime = currentTime;

            deltaTime = Math.Clamp(deltaTime, 0, 0.5f) * GAME_TIME_SCALE;

        }

        private static void Events(RenderWindow renderWindow)
        {
            renderWindow.DispatchEvents();
            renderWindow.Closed += (sender, e) => { ((Window)sender).Close(); };
            renderWindow.LostFocus += (sender, e) => { GAME_HAS_FOCUS = false; ((RenderWindow)sender).SetFramerateLimit(Settings.UnfocusedFPS); };
            renderWindow.GainedFocus += (sender, e) => { GAME_HAS_FOCUS = true; ((RenderWindow)sender).SetFramerateLimit(Settings.MaxFPS); };

            #region function keys

            if (Keyboard.IsKeyPressed(Keyboard.Key.F11) && !f11Pressed)
            {
                f11Pressed = true;
                ToggleFullscreen(renderWindow);
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
                    Logger.Log("Saved a screenshot to: " + path);
                }
                catch
                {
                    Logger.LogError("Failed to save a screenshot!");
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
                //Game.GAME_SCENE_MANAGER.reloadScene();
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

            if (Keyboard.IsKeyPressed(Keyboard.Key.F7) && !f7Pressed && Program.antiPlatformer.GAME_DEBUG)
            {
                f7Pressed = true;
                if(Program.antiPlatformer.GAME_STATE == 99)
                {
                    Program.antiPlatformer.sceneEditor.Exit();
                    Program.antiPlatformer.GAME_STATE = 3;
                }
                else
                {
                    Program.antiPlatformer.sceneEditor.Init(renderWindow);
                    Program.antiPlatformer.GAME_STATE = 99;
                }
            }
            if (!Keyboard.IsKeyPressed(Keyboard.Key.F7) && f7Pressed)
            {
                f7Pressed = false;
            }

            #endregion
        }

        public static void ToggleFullscreen(RenderWindow renderWindow)
        {
            try
            {
                if(isFullscreen)
                {
                    isFullscreen = false;
                    renderWindow.Close();
                    renderWindow = new RenderWindow(Program.antiPlatformer.mode, "The anti-Platformer V" + Program.antiPlatformer.GAME_VERSION);
                    renderWindow.SetFramerateLimit(Settings.MaxFPS);
                    foreach(ui.APGui gui in Program.antiPlatformer.guis.Values)
                    {
                        gui.SetRenderWindow(renderWindow);
                    }
                    RestartDeltaTime();
                }
                else
                {
                    isFullscreen = true;
                    renderWindow.Close();
                    renderWindow = new RenderWindow(VideoMode.DesktopMode, "...this is a fullscreen window how are you seeing this", Styles.Fullscreen);
                    renderWindow.SetFramerateLimit(Settings.MaxFPS);
                    foreach(ui.APGui gui in Program.antiPlatformer.guis.Values)
                    {
                        gui.SetRenderWindow(renderWindow);
                    }
                    RestartDeltaTime();
                }
            }
            catch(Exception e)
            {
                Logger.LogError("Failed to toggle fullscreen. Exception: " + e);
                renderWindow = new RenderWindow(Program.antiPlatformer.mode, "The anti-Platformer V" + Program.antiPlatformer.GAME_VERSION);
                renderWindow.SetFramerateLimit(Settings.MaxFPS);
            }
        }

        public static float GetDeltaTime()
        {
            return deltaTime;
        }

        public static void RestartDeltaTime()
        {
            deltaClock.Restart();
            deltaTime = 0;
        }

        public static int GetFPS()
        {
            return (int)fps;
        }
    }

    public class Settings
    {
        private static string Version;
        public static uint MaxFPS = 165;
        public static uint UnfocusedFPS = 30;
        public static float Volume = 5.0f;

        private static void CreateSettings()
        {
            Directory.CreateDirectory("user");
            File.Delete("user/settings.txt");
            var fs = new FileStream("settings.txt", FileMode.Create);
            fs.Dispose();
            File.WriteAllText("user/settings.txt", "version=" + Program.antiPlatformer.GAME_VERSION + "\nvolume=5.0\nmaxfps=0\nunfocusedfps=30");
            Logger.Log("Created a new settings file");
        }

        // public static void checkForUpdate()
        // {
        //     ui.gui.LoadWidgetsFromFile("res/gui/update.gui");

        //     try
        //     {
        //         foreach(ChildWindow window in ui.gui.Widgets)
        //         {
        //             foreach(Button item in window.Widgets.OfType<Button>())
        //             {
        //                 if(item.Name == "done")
        //                 {
        //                     item.Pressed += (s, e) => ui.gui.RemoveAllWidgets();
        //                 }
        //                 else if(item.Name == "link")
        //                 {
        //                     item.Pressed += (s, e) => { gameManager.openUrl("https://github.com/RooXChicken/antiplatformer/releases"); ui.gui.RemoveAllWidgets(); };
        //                 }
        //             }
        //             foreach(TextBox item in window.Widgets.OfType<TextBox>())
        //             {
        //                 if(item.Name == "details")
        //                 {
        //                     string text = Game.GAME_NETWORKING.sendGetString("checkforupdate" + gameManager.GAME_VERSION, true, 2000);
        //                     if(text == "latest")
        //                     {
        //                         ui.gui.RemoveAllWidgets();
        //                     }
        //                     else
        //                     {
        //                         string[] actualtext = text.Split('<');
        //                         if(actualtext.Length != 0)
        //                             item.Text = "";
        //                         foreach(string tex in actualtext)
        //                         {
        //                             item.Text += tex + "\n";
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     catch
        //     {
        //         ui.gui.RemoveAllWidgets();
        //     }
        // }

        public static void LoadSettings()
        {
            if(File.Exists("user/settings.txt"))
            {
                string[] settings = File.ReadAllLines("user/settings.txt");
                try
                {
                    foreach (string setting in settings)
                    {
                        if (setting == "regen=true")
                        {
                            CreateSettings();
                        }
                        else if (setting.Contains("volume="))
                        {
                            Volume = float.Parse(setting.Substring(7));
                        }
                        else if (setting.Contains("maxfps="))
                        {
                            MaxFPS = UInt32.Parse(setting.Substring(7));
                        }
                        else if(setting.Contains("keys="))
                        {
                            string[] keyArr = setting.Split(',');

                            foreach(string key in keyArr)
                            {
                                //keys.Add(key, SFML.Window.Keyboard.)
                            }
                        }
                        else if (setting.Contains("unfocusedfps="))
                        {
                            UnfocusedFPS = UInt32.Parse(setting.Substring(13));
                        }
                        else if(setting.Contains("version="))
                        {
                            Version = setting.Substring(8);
                        }
                    }
                }
                catch
                {
                    Logger.LogError("Failed to load settings. Creating a new settings file.");
                }
            }
            else
            {
                CreateSettings();
            }
        }
    }

    public class Math
    {
        public static float Clamp(float value, float min, float max)
        {
            float val = value;
            if(value < min)
                val = min;
            if(value > max)
                val = max;

            return val;
        }

        public static int ClampInt(int value, int min, int max)
        {
            int val = value;
            if(value < min)
                val = min;
            if(value > max)
                val = max;

            return val;
        }
    }
}