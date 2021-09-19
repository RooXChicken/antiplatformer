using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static bool GAME_DEBUG = false;

        public Game(string[] args)
        {
            gameManager.handleArgs(args);
        }

        //static variables
        public static string GAME_MAIN_CHARACTER_NAME = "roo";
        public static int GAME_MAIN_CHARACTER_HEALTH = 0;
        public static Vector2f GAME_PLAYER_POSITION;
        public static networking GAME_NETWORKING = new networking();

        public static scene GAME_SCENE_MANAGER = new scene();
        public static Font GAME_FONT;
        public static Sprite GAME_SKYBOX;
        public static int GAME_PLAYER_INDEX = 0;

        private static RenderWindow renderWindow;
        public static VideoMode videomode;
        private Image windowIcon = new Image("res/icon.png");
        public static View camera;
        public static View uiCamera;
        private Text debugText;

        private Clock splashClock = new Clock();

        public static List<entity> entityList = new List<entity>();
        public static List<entity> players = new List<entity>();
        public static List<entity> killedEntities = new List<entity>();

        public static TileMap tilemap = new TileMap();

        public bool isRunning() { return renderWindow.IsOpen; }
        public static RenderWindow getRenderWindow() { return renderWindow; }

        public void init()
        {
            Directory.CreateDirectory("logs");
            utils.Log("Starting game...");

            settings.loadSettings();

            videomode = new VideoMode(1280, 720);
            try
            {
                renderWindow = new RenderWindow(videomode, "The anti-Platformer V" + gameManager.GAME_VERSION);
                renderWindow.SetIcon(64, 64, windowIcon.Pixels);
                ui.gui = new Gui(renderWindow);
                utils.Log("Created the main window");
            }
            catch(Exception e)
            {
                utils.LogFatal("Failed to open the window. Exception: " + e, false);
            }

            renderWindow.SetFramerateLimit(settings.max_fps);

            camera = new View(new Vector2f(0f, 0f), gameManager.GAME_INTERNAL_RESOLUTION);
            uiCamera = new View(new Vector2f(127, 72), gameManager.GAME_INTERNAL_RESOLUTION);
            utils.Log("Created the cameras");

            GAME_NETWORKING.init("ap.rooxchicken.com", 7777, "awesomegame");

            gameManager.drpc.Initialize();

            initSplash();
        }

        private void initSplash()
        {
            gameManager.GAME_STATE = 1;
            GAME_SKYBOX = utils.loadSprite("res/misc/randomsprites/skybox.png");
            GAME_SKYBOX.Position = new Vector2f(-1, 0);

            entityList.Add(new entity("splashScreen", ""));
        }

        private void initTitle()
        {
            gameManager.GAME_STATE = 2;
            entityList.Clear();
            splashClock.Dispose();
            
            ui.gui.RemoveAllWidgets();

            ui.gui.LoadWidgetsFromFile("res/gui/titlescreen.gui");

            foreach(Button item in ui.gui.Widgets.OfType<Button>())
            {
                switch(item.Name)
                {
                    case "start":
                        item.Pressed += (s, e) => initMain(3);
                        break;
                    case "settings":
                        item.Pressed += (s, e) => utils.Log("Not added (i even told you lol)");
                        break;
                    case "multiplayer":
                        //item.Pressed += (s, e) => initMultiplayer();
                        break;
                    case "quit":
                        item.Pressed += (s, e) => renderWindow.Close();
                        break;
                    default:
                        utils.LogWarn("Button not added yet!");
                        break;
                }
            }

            //guess i added the titlescreen lol
            //initMain();
        }
        
        private void initMultiplayer()
        {
            ui.gui.RemoveAllWidgets();
            ui.gui.LoadWidgetsFromFile("res/gui/multiplayer.gui");

            string ip = "ERROR";
            int port = 0;
            string password = "ERROR";

            try
            {
                foreach(ChildWindow window in ui.gui.Widgets.OfType<ChildWindow>())
                {
                    foreach(Button item in window.Widgets.OfType<Button>())
                    {
                        switch(item.Name)
                        {
                            case "connect":
                                item.Pressed += (s, e) => {
                                    foreach(TextBox item2 in window.Widgets.OfType<TextBox>())
                                    {
                                        switch(item2.Name)
                                        {
                                            case "ip":
                                                ip = item2.Text;
                                                break;
                                            case "port":
                                                try
                                                {
                                                    port = Int32.Parse(item2.Text);
                                                }
                                                catch (Exception eee)
                                                {
                                                    utils.LogError("Failed to start server: The port was not a number ya doofus. Exception: " + eee);
                                                }
                                                break;
                                            default:
                                                utils.Log("Not added yet!");
                                                break;
                                        }

                                        GAME_NETWORKING.init(ip, port, "awesomegame");
                                    }

                                    ui.gui.RemoveAllWidgets();
                                    initMain(4);
                                };
                                break;
                            case "cancel":
                                item.Pressed += (s, e) => initTitle();
                                break;
                            default:
                                utils.LogWarn("Button not added yet!");
                                break;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void initMain(int state)
        {
            gameManager.GAME_STATE = state;
            ui.load();

            GAME_FONT = new Font("res/misc/font.ttf");
            utils.Log("Loaded the font");

            debugText = new Text("", GAME_FONT);
            debugText.Scale = new Vector2f(0.15f, 0.15f);
            debugText.Position = new Vector2f(0, 1);

            ui.gui.TabKeyUsageEnabled = true;

            GAME_SCENE_MANAGER.loadScene("res/levels/world1/tutorial.apscene");

            settings.checkForUpdate();
        }

        public void update()
        {
            try
            {
                GAME_NETWORKING.update();
                gameManager.update();
                gameManager.handleEvents(renderWindow);

                audioManager.update();

                switch (gameManager.GAME_STATE)
                {
                    case 0:
                        renderWindow.Close();
                        break;
                    case 1:
                        splashUpdate();
                        splashRender();
                        break;
                    case 2:
                        titleUpdate();
                        titleRender();
                        break;
                    case 3:
                        mainUpdate();
                        mainRender();
                        break;
                    case 4:
                        multiplayerUpdate();
                        multiplayerRender();
                        break;
                    case 99:
                        gameManager.se.update();
                        gameManager.se.render();
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
            if (splashClock.ElapsedTime.AsMilliseconds() / 7 < 400)
            {
                entityList[0].myClass.update(gameManager.deltaTime);
            }
            else
                initTitle();
        }

        private void titleUpdate() { }

        private void tickEntities()
        {
            if(gameManager.GAME_HAS_FOCUS)
            {
                foreach (entity e in entityList)
                {
                    e.myClass.update(gameManager.deltaTime);
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
            }
        }

        private void mainUpdate()
        {
            if (!gameManager.isLoading)
            {
                tickEntities();

                GAME_PLAYER_POSITION = entityList[GAME_PLAYER_INDEX].myClass.position;
                camera.Center = GAME_PLAYER_POSITION;

                debugText.DisplayedString = "The anti-Platformer V" + gameManager.GAME_VERSION + " Debug menu\nFPS:" + (int)gameManager.getFPS() + "\nX:" + GAME_PLAYER_POSITION.X.ToString() + "\nY:" + GAME_PLAYER_POSITION.Y.ToString() + "\nAir time: " + entityList[GAME_PLAYER_INDEX].myClass.coyoteJump.ElapsedTime.AsMilliseconds();
            }
        }

        private void multiplayerUpdate()
        {
            if(gameManager.GAME_GAME_TIME.ElapsedTime.AsMilliseconds() % 16 == 0)
            {
                
            }

            if (!gameManager.isLoading)
            {
                tickEntities();

                GAME_PLAYER_POSITION = entityList[GAME_PLAYER_INDEX].myClass.position;
                camera.Center = GAME_PLAYER_POSITION;

                debugText.DisplayedString = "The anti-Platformer V" + gameManager.GAME_VERSION + " Debug menu\nFPS:" + (int)gameManager.getFPS() + "\nX:" + GAME_PLAYER_POSITION.X.ToString() + "\nY:" + GAME_PLAYER_POSITION.Y.ToString() + "\nAir time: " + entityList[GAME_PLAYER_INDEX].myClass.coyoteJump.ElapsedTime.AsMilliseconds();
            }
        }

        private void splashRender()
        {
            renderWindow.Clear();
            renderWindow.Draw(GAME_SKYBOX);

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

        private void titleRender() 
        {
            renderWindow.Clear();
            renderWindow.SetView(uiCamera);

            renderWindow.Draw(GAME_SKYBOX);

            renderUI();

            renderWindow.Display();
        }

        private void mainRender()
        {
            renderWindow.Clear();
            renderWindow.SetView(uiCamera);

            renderWindow.Draw(GAME_SKYBOX);

            //draw game world
            renderWindow.SetView(camera);

            renderWindow.Draw(tilemap);

            if(!gameManager.isLoading)
            {
                foreach (entity e in entityList)
                {
                    renderWindow.Draw(e.myClass.getSprite());
                }
            }

            renderUI();

            renderWindow.Display();
        }

        private void multiplayerRender()
        {
            renderWindow.Clear();
            renderWindow.SetView(uiCamera);

            renderWindow.Draw(GAME_SKYBOX);

            //draw game world
            renderWindow.SetView(camera);

            renderWindow.Draw(tilemap);

            if(!gameManager.isLoading)
            {
                foreach (entity e in entityList)
                {
                    renderWindow.Draw(e.myClass.getSprite());
                }

                foreach (entity e in players)
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

            if(gameManager.debugOpen)
            {
                renderWindow.Draw(debugText);
            }
            else
            {
                ui.gui.Draw();
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
            try
            {
                //networking.shutdown();
                GAME_NETWORKING.shutdown();
                camera.Dispose();
                uiCamera.Dispose();
                utils.Log("Unloaded the cameras");
                entityList.Clear();
                utils.Log("Unloaded the entities");
                debugText.Dispose();
                gameManager.deltaClock.Dispose();
                gameManager.framerate_clock.Dispose();
                utils.Log("Unloaded the framerate clocks");
                gameManager.drpc.Deinitialize();
                utils.Log("Unloaded DiscordRPC");
                renderWindow.Dispose();
                utils.Log("Unloaded the window");
                ui.unload();
                utils.Log("Unloaded the UI");
            }
            catch (Exception e)
            {
                utils.LogWarn("Object unloaded was never loaded! Exception: " + e);
            }
        }
    }
}
