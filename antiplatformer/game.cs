using System;
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
    public class Game
    {
        public static bool debug = false;
        public Game(string[] args)
        {
            #region command line arguments
            if (args.Length > 0)
            {
                bool wasArg = false;
                foreach (string arg in args)
                {
                    if (arg == "-debug" || arg == "-dev" || arg == "-developer")
                    {
                        debug = true;
                        wasArg = true;
                    }
                    if (arg == "-help")
                    {
                        utils.Log("---------------------\nAll current arguments are:\n-help | Shows this message\n-stop | Stops the game as soon as it starts\n-debug/-dev/-developer | Gives access to the advanced debugging features\n---------------------");
                        wasArg = true;
                    }
                    if (arg == "-stop")
                    {
                        renderWindow = new RenderWindow(new VideoMode(0, 0), "Closing...");
                        renderWindow.Close();
                        wasArg = true;
                    }
                    if (arg == "-skipSplash")
                    {
                        skipSplash = true;
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
            #endregion
        }

        //static variables
        public static string GAME_VERSION = "A-0.2.1";
        public static Vector2f GAME_INTERNAL_RESOLUTION = new Vector2f(256, 144);
        public static Clock GAME_GAME_TIME = new Clock();
        public static Clock GAME_SPEEDRUN_TIMER;
        public static int GAME_NUMBER_TICKS = 0;
        public static uint GAME_MAX_FPS = 0;
        public static uint GAME_MINIMIZED_FPS = 30;
        public static Vector2f GAME_PLAYER_POSITION;
        public static Color GAME_BACKGROUND_COLOR = Color.Cyan;
        public static Font GAME_MAIN_FONT;
        public static string GAME_MAIN_CHARACTER_NAME = "roo";
        public static int GAME_MAIN_CHARACTER_HEALTH = 0;
        public static scene GAME_SCENE_MANAGER = new scene();
        public static Sprite GAME_MAIN_SKYBOX;
        public static float GAME_MAIN_AUDIO_VOLUME = 5f;
        public static Music GAME_CURRENT_MUSIC;
        public static int GAME_STATE = 0;
        public static int GAME_PLAYER_INDEX = 0;
        public static bool GAME_CAN_LOAD = true;
        private Image GAME_WINDOW_ICON = new Image("res/icon.png");
        public static Gui gui;

        private static RenderWindow renderWindow;
        private VideoMode videomode;
        public static bool isLoading = true;
        public static View camera;
        public static View uiCamera;
        private static Text debugText;
        public static discordRPC drpc = new discordRPC();
        private bool hasFocus = true;

        private sceneEditor se = new sceneEditor();

        private bool skipSplash = false;
        private Clock splashClock = new Clock();

        public static List<entity> entityList = new List<entity>();
        public static List<entity> killedEntities = new List<entity>();

        //for function keys
        private bool f11Pressed = false;
        private bool isFullscreen = false;
        private bool f2Pressed = false;
        private bool f5Pressed = false;
        private bool f3Pressed = false;
        private bool f7Pressed = false;
        private bool debugOpen = false;

        public static TileMap tilemap = new TileMap();

        #region fps stuff
        private Clock framerate_clock = new Clock();
        public static Clock deltaClock = new Clock();
        public static float deltaTime;
        private float framerate_lastTime = 0;
        public float fps;
        #endregion

        public bool isRunning() { return renderWindow.IsOpen; }
        public static RenderWindow getRenderWindow() { return renderWindow; }

        public void init()
        {
            Directory.CreateDirectory("logs");
            utils.Log("Starting game...");

            videomode = new VideoMode(1280, 720);
            try
            {
                renderWindow = new RenderWindow(videomode, "The anti-Platformer V" + GAME_VERSION);
                renderWindow.SetIcon(64, 64, GAME_WINDOW_ICON.Pixels);
                gui = new Gui(renderWindow);
                utils.Log("Created the main window");
            }
            catch(Exception e)
            {
                utils.LogFatal("The window failed to open! how...? good luck fixing this one buddy: " + e, false);
            }

            camera = new View(new Vector2f(0f, 0f), GAME_INTERNAL_RESOLUTION);
            uiCamera = new View(new Vector2f(127, 72), GAME_INTERNAL_RESOLUTION);
            utils.Log("Created the cameras");

            drpc.Initialize();

            initSplash();
        }

        private void initSplash()
        {
            GAME_STATE = 0;
            GAME_MAIN_SKYBOX = utils.loadSprite("res/misc/randomsprites/skybox.png");
            GAME_MAIN_SKYBOX.Position = new Vector2f(-1, 0);

            entityList.Add(new entity("splashScreen", ""));
        }

        private void initTitle()
        {
            GAME_STATE = 1;
            entityList.Clear();
            splashClock.Dispose();

            #region settings

            if (File.Exists("settings.txt"))
            {
                string[] settings = File.ReadAllLines("settings.txt");
                try
                {
                    foreach (string setting in settings)
                    {
                        if (setting == "regen=true")
                        {
                            File.Delete("settings.txt");
                            var fs = new FileStream("settings.txt", FileMode.Create);
                            fs.Dispose();
                            File.WriteAllText("settings.txt", "version=" + GAME_VERSION + "\nvolume=5.0\nmaxfps=0\nunfocusedfps=30");
                        }
                        else if (setting.Contains("volume="))
                        {
                            GAME_MAIN_AUDIO_VOLUME = float.Parse(setting.Substring(7));
                        }
                        else if (setting.Contains("maxfps="))
                        {
                            GAME_MAX_FPS = UInt32.Parse(setting.Substring(7));
                        }
                        else if (setting.Contains("unfocusedfps="))
                        {
                            GAME_MINIMIZED_FPS = UInt32.Parse(setting.Substring(13));
                        }
                    }
                }
                catch
                {
                    utils.LogError("Settings file corrupt or outdated! Ill load what I can, but if you want to fix this error, back up your settings and add regen=true to the top, and I will regenerate the settings file so its not corrupt. See the wiki for more info!");
                }
            }
            else
            {
                var fs = new FileStream("settings.txt", FileMode.Create);
                fs.Dispose();
                File.WriteAllText("settings.txt", "version=" + GAME_VERSION + "\nvolume=5.0\nmaxfps=165\nunfocusedfps=30");
                utils.Log("Created a new settings file");
            }

            #endregion

            //until i add title screen
            initMain();
        }

        private void initMain()
        {
            GAME_STATE = 2;
            ui.load();

            renderWindow.SetFramerateLimit(GAME_MAX_FPS);

            GAME_MAIN_FONT = new Font("res/misc/font.ttf");
            utils.Log("Loaded the font");

            debugText = new Text("", GAME_MAIN_FONT);
            debugText.Scale = new Vector2f(0.15f, 0.15f);
            debugText.Position = new Vector2f(0, 1);

            gui.TabKeyUsageEnabled = true;

            GAME_SPEEDRUN_TIMER = new Clock();
            GAME_SCENE_MANAGER.loadScene("res/levels/world1/tutorial.apscene");
        }

        public void update()
        {
            try
            {
                events();
                GAME_NUMBER_TICKS++;
                deltaTime = deltaClock.Restart().AsSeconds();

                switch (GAME_STATE)
                {
                    case 0:
                        splashUpdate();
                        splashRender();
                        break;
                    case 1:
                        titleUpdate();
                        titleRender();
                        break;
                    case 2:
                        mainUpdate();
                        mainRender();
                        break;
                    case 99:
                        se.update(deltaTime);
                        se.render();
                        break;
                }
            }
            catch(Exception e)
            {
                utils.LogFatal("Failed to update the main game! I'm going to keep going, but beware of dragons! Exception: " + e, true);
            }
        }

        private void splashUpdate()
        {
            if (splashClock.ElapsedTime.AsMilliseconds() / 7 < 400 && !skipSplash)
            {
                entityList[0].myClass.update(deltaTime);
                if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
                    skipSplash = true;
            }
            else
                initTitle();
            if(skipSplash)
                initTitle();
        }

        private void titleUpdate() { }

        private void tickEntities()
        {
            if(hasFocus)
            {
                try
                {
                    foreach (entity e in entityList)
                    {
                        e.myClass.update(deltaTime);
                        if (e.myClass.destroy == true)
                        {
                            e.myClass.onKill();
                            killedEntities.Add(e);
                        }
                    }

                    foreach (entity e in killedEntities)
                    {
                        entityList.Remove(e);
                        GAME_PLAYER_INDEX = entityList.Count - 1;
                    }

                    killedEntities.Clear();
                }
                catch (Exception e)
                {
                    utils.LogError("Failed to tick entities with exception: " + e);
                }
            }
        }

        private void mainUpdate()
        {
            if (!isLoading)
            {
                GAME_CAN_LOAD = false;

                tickEntities();

                GAME_PLAYER_POSITION = entityList[GAME_PLAYER_INDEX].myClass.position;
                camera.Center = GAME_PLAYER_POSITION;

                debugText.DisplayedString = "The anti-Platformer V" + GAME_VERSION + " Debug menu\nFPS:" + (int)fps + "\nX:" + GAME_PLAYER_POSITION.X.ToString() + "\nY:" + GAME_PLAYER_POSITION.Y.ToString() + "\nAir time: " + entityList[GAME_PLAYER_INDEX].myClass.coyoteJump.ElapsedTime.AsMilliseconds() + "\nSpeedrun Timer: " + GAME_SPEEDRUN_TIMER.ElapsedTime.AsSeconds().ToString();

                GAME_CAN_LOAD = true;
            }
        }

        private void events()
        {
            renderWindow.DispatchEvents();
            renderWindow.Closed += (sender, e) => { ((Window)sender).Close(); };
            renderWindow.LostFocus += (sender, e) => { hasFocus = false; ((RenderWindow)sender).SetFramerateLimit(GAME_MINIMIZED_FPS); };
            renderWindow.GainedFocus += (sender, e) => { hasFocus = true; ((RenderWindow)sender).SetFramerateLimit(GAME_MAX_FPS); };

            #region function keys

            if (Keyboard.IsKeyPressed(Keyboard.Key.F11) && !f11Pressed)
            {
                f11Pressed = true;
                if(isFullscreen)
                {
                    isFullscreen = false;
                    renderWindow.Close();
                    renderWindow = new RenderWindow(videomode, "The anti-Platformer V" + GAME_VERSION);
                    renderWindow.SetFramerateLimit(GAME_MAX_FPS);
                    gui.Target = renderWindow;
                    deltaTime = 0;
                    deltaClock.Restart();
                }
                else
                {
                    //hasFocus = false;
                    isFullscreen = true;
                    renderWindow.Close();
                    renderWindow = new RenderWindow(VideoMode.DesktopMode, "...this is a fullscreen window how are you seeing this", Styles.Fullscreen);
                    renderWindow.SetFramerateLimit(GAME_MAX_FPS);
                    gui.Target = renderWindow;
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
                GAME_SCENE_MANAGER.reloadScene();
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

            if (Keyboard.IsKeyPressed(Keyboard.Key.F7) && !f7Pressed)
            {
                f7Pressed = true;
                if(GAME_STATE == 99)
                {
                    se.exit();
                    GAME_STATE = 2;
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

            #region fps

            float currentTime = framerate_clock.Restart().AsSeconds();
            if (GAME_GAME_TIME.ElapsedTime.AsMilliseconds() % 125 == 0)
            {
                fps = 1 / currentTime;
            }
            framerate_lastTime = currentTime;

            #endregion
        }

        private void splashRender()
        {
            renderWindow.Clear(GAME_BACKGROUND_COLOR);
            renderWindow.Draw(GAME_MAIN_SKYBOX);

            renderWindow.SetView(uiCamera);

            try
            {
                renderWindow.Draw(entityList[0].myClass.getSprite());
            }
            catch
            {
                utils.LogError("splash screen broke lol i dont care");
            }

            renderWindow.Display();
        }

        private void titleRender() { }

        private void mainRender()
        {
            renderWindow.Clear(GAME_BACKGROUND_COLOR);
            renderWindow.SetView(uiCamera);

            renderWindow.Draw(GAME_MAIN_SKYBOX);

            //draw game world
            renderWindow.SetView(camera);

            renderWindow.Draw(tilemap);

            //foreach (Sprite sprite in tileMap.tilemap)
            //{
            //    if (GAME_PLAYER_POSITION.X - sprite.Position.X < GAME_INTERNAL_RESOLUTION.X && GAME_PLAYER_POSITION.Y - sprite.Position.Y < GAME_INTERNAL_RESOLUTION.Y && GAME_PLAYER_POSITION.X + GAME_INTERNAL_RESOLUTION.X - sprite.Position.X > 0 && GAME_PLAYER_POSITION.Y + GAME_INTERNAL_RESOLUTION.Y - sprite.Position.Y > 0)
            //    {
            //        renderWindow.Draw(sprite);
            //    }
            //}

            if(!isLoading)
            {
                foreach (entity e in entityList)
                {
                    renderWindow.Draw(e.myClass.getSprite());
                }
            }

            renderUI();

            renderWindow.Display();
        }

        private void renderUI()
        {
            //draw ui
            renderWindow.SetView(uiCamera);

            if(debugOpen)
            {
                renderWindow.Draw(debugText);
            }
            else
            {
                gui.Draw();
                foreach (Sprite element in ui.sprites)
                {
                    renderWindow.Draw(element);
                }
            }
        }

        public static void cameraShake(int degrees, bool right)
        {
            Thread shake = new Thread(() => {
                if (right)
                {
                    for (int i = 0; i < degrees; i++)
                    {
                        camera.Rotation = i;
                        Thread.Sleep(5);
                    }
                    for (int i = degrees; i > 0; i--)
                    {
                        camera.Rotation = i;
                        Thread.Sleep(5);
                    }
                }
                else
                {
                    for (int i = 0; i < degrees; i++)
                    {
                        camera.Rotation = -i;
                        Thread.Sleep(5);
                    }
                    for (int i = degrees; i > 0; i--)
                    {
                        camera.Rotation = -i;
                        Thread.Sleep(5);
                    }
                }
                camera.Rotation = 0; 
                return; });

            shake.Start();
        }

        public void shutdown()
        {
            camera.Dispose();
            uiCamera.Dispose();
            utils.Log("Unloaded the cameras");
            ui.unload();
            utils.Log("Unloaded the UI");
            entityList.Clear();
            utils.Log("Unloaded the entities");
            debugText.Dispose();
            deltaClock.Dispose();
            framerate_clock.Dispose();
            utils.Log("Unloaded the framerate clocks");
            drpc.Deinitialize();
            utils.Log("Unloaded DiscordRPC");
            renderWindow.Dispose();
            utils.Log("Unloaded the window");
        }
    }
}
