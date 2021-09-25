using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using antiplatformer.ui;
using antiplatformer.entity;
using antiplatformer.audio;

namespace antiplatformer
{
    public class AntiPlatformer
    {
        public int GAME_STATE = 0;
        public bool GAME_DEBUG = true;
        public string GAME_VERSION = "A-0.2.5";

        public RenderWindow window;
        public VideoMode mode;
        private Text debugText;
        public Sprite skybox;

        public EntityManager entityManager;
        public TileMap loadedLevel = new TileMap();
        public SceneLoader sceneLoader = new SceneLoader();
        public SceneEditor sceneEditor = new SceneEditor();

        public Dictionary<string, APGui> guis = new Dictionary<string, APGui>();

        public View camera;
        public View uiCamera;

        public AntiPlatformer(string[] args)
        {

        }

        public bool isRunning() { return window.IsOpen; }

        public void init()
        {
            Logger.Init();
            Logger.Log("Starting game...");
            GAME_STATE = 1;
            mode = new VideoMode(1280, 720);
            window = new RenderWindow(mode, "The anti-Platformer");
            window.SetFramerateLimit(0);
            window.SetVerticalSyncEnabled(false);
            Logger.Log("Created the window");

            camera = new View(new Vector2f(0, 0), new Vector2f(256, 144));
            uiCamera = new View(new Vector2f(127, 72), new Vector2f(256, 144));
            Logger.Log("Created the cameras");

            debugText = new Text("ERROR", Manager.font);
            debugText.Scale = new Vector2f(0.15f, 0.15f);
            debugText.Position = new Vector2f(0, 1);

            guis.Add("pause", new PauseMenuUI(window, false));

            entityManager = new EntityManager(this);
            Logger.Log("Created the entity manager");
            initSplash();
        }

        private void initSplash()
        {
            GAME_STATE = 1;
            skybox = Loader.LoadSprite("res/sprites/levels/default/skybox.png");

            entityManager.AddEntity("SplashScreen", "");
            //initTitle();
        }

        public void initTitle()
        {
            GAME_STATE = 2;

            entityManager.Clear();

            guis.Add("title", new TitlescreenUI(window, false));

            //guess i added the titlescreen lol
            //initMain();
        }

        public void initMain(int gamestate)
        {
            GAME_STATE = gamestate;

            guis["title"].Unload();
            guis.Remove("title");

            sceneLoader.LoadScene("res/levels/world1/tutorial.apscene");
        }

        public void update()
        {
            Manager.Update(window);
            GameInput.Update();

            SoundEffectManager.Update();
            MusicManager.Update();

            try
            {
                switch(GAME_STATE)
                {
                    case 0:
                        window.Close();
                        break;
                    case 1:
                        updateSplash();
                        break;
                    case 2:
                        updateTitle();
                        break;
                    case 3:
                        updateMain();
                        break;
                    
                    case 99:
                        sceneEditor.Update(window);
                        break;

                    default:
                        window.Close();
                        break;
                }
            }
            catch(Exception e)
            {
                Logger.LogError("Failed to update the main game! Beware of DRAGONS! Exception: " + e);
            }
        }

        private void updateSplash()
        {
            if(entityManager.GetEntity("SplashScreen0").Alive && !GameInput.GetKeyDown(Keyboard.Key.Space))
            {
                try
                {
                    entityManager.GetEntity("SplashScreen0").Update();
                }
                catch
                {
                    initTitle();
                }
            }
            else
            {
                initTitle();
            }
        }

        private void updateTitle()
        {
            
        }

        private void updateMain()
        {
            if(GameInput.GetKeyDown(Keyboard.Key.Escape) && !Manager.escapePressed)
            {
                Manager.escapePressed = true;
                if(Manager.paused)
                {
                    Manager.paused = false;
                    guis["pause"].Clickable = false;
                }
                else
                {
                    Manager.paused = true;
                    guis["pause"].Clickable = true;
                }
            }

            if(!GameInput.GetKeyDown(Keyboard.Key.Escape) && Manager.escapePressed)
            {
                Manager.escapePressed = false;
            }

            if(!Manager.paused)
            {
                foreach(Entity e in entityManager.GetEntities())
                {
                    e.Update();
                    if(!e.Alive)
                    {
                        e.OnKill();
                        entityManager.RemoveEntity(e.Name);
                    }
                }
            }

            camera.Center = entityManager.GetEntity("Player0").Position;

            entity.entities.Player p = (entity.entities.Player)entityManager.GetEntity("Player0");

            debugText.DisplayedString = "The anti-Platformer V" + GAME_VERSION + " Debug menu\nFPS:" + (int)Manager.GetFPS() + "\nX:" + p.Position.X.ToString() + "\nY:" + p.Position.Y.ToString() + "\nAir time: " + p.coyoteJump.ElapsedTime.AsMilliseconds();
        }

        public void render()
        {
            switch(GAME_STATE)
            {
                case 0:
                    window.Close();
                    break;
                case 1:
                    renderSplash();
                    break;
                case 2:
                    renderTitle();
                    break;
                case 3:
                    renderMain();
                    break;

                case 99:
                    sceneEditor.Render(window);
                    break;

                default:
                    window.Close();
                    break;
            }
        }

        private void renderSplash()
        {
            window.Clear();
            window.SetView(uiCamera);
            window.Draw(skybox);

            entityManager.GetEntity("SplashScreen0").Render(window);

            window.Display();
        }

        private void renderTitle()
        {
            window.Clear();
            window.SetView(uiCamera);
            window.Draw(skybox);

            guis["title"].Update();

            window.Display();
        }

        private void renderMain()
        {
            window.Clear();
            window.SetView(uiCamera);
            window.Draw(skybox);

            window.SetView(camera);

            window.Draw(loadedLevel);

            foreach(Entity e in entityManager.GetEntities())
            {
                e.Render(window);
            }

            window.SetView(uiCamera);

            window.Draw(debugText);

            if(Manager.paused)
            {
                guis["pause"].Update();
            }

            window.Display();
        }
    }
}