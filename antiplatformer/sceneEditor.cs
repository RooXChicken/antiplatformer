using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
namespace antiplatformer
{
    public class sceneEditor
    {
        public Sprite skybox;
        public List<string> sceneData = new List<string>();
        private static Text debugText;
        public string levelPath = "ERROR";
        View cameraPosition = new View(new Vector2f(0f, 0f), Game.GAME_INTERNAL_RESOLUTION);

        float movementSpeed = 200;

        string entityGrabbedName;
        bool isGrabbed = false;

        Vector2f position = new Vector2f(0, 0);
        Vector2f mousePosition;

        string[] grabbedEntityInput;

        public sceneEditor()
        {

        }

        public void init()
        {
            try
            {
                Game.GAME_CURRENT_MUSIC.Stop();
                Game.GAME_CURRENT_MUSIC.Dispose();
                Game.GAME_CURRENT_MUSIC = null;
            }
            catch
            {
                utils.LogWarn("Tried to stop music when it's not playing. This is normal, you can ignore this");
            }

            sceneData = new List<string>();

            sceneData.Add("level=res/levels/world1/level1/level1.map");
            sceneData.Add("skybox=res/misc/randomsprites/skybox.png");
            sceneData.Add("levelname=Level 1");
            sceneData.Add("music=none");

            debugText = new Text("", Game.GAME_MAIN_FONT);
            debugText.Scale = new Vector2f(0.15f, 0.15f);
            debugText.Position = new Vector2f(0, 1);

            Game.drpc.Update("Scene editor", "Working on a scene...");
        }

        public void update(float deltaTime)
        {
            mousePosition = Game.getRenderWindow().MapPixelToCoords(new Vector2i(Mouse.GetPosition().X, Mouse.GetPosition().Y));
            mousePosition.X = mousePosition.X + position.X - (Game.GAME_INTERNAL_RESOLUTION.X / 2);
            mousePosition.Y = mousePosition.Y + position.Y - Game.GAME_INTERNAL_RESOLUTION.Y;

            if (Keyboard.IsKeyPressed(Keyboard.Key.Left) || Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                position.X -= movementSpeed * deltaTime;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right) || Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                position.X += movementSpeed * deltaTime;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up) || Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                position.Y -= movementSpeed * deltaTime;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Down) || Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                position.Y += movementSpeed * deltaTime;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.LControl))
            {
                movementSpeed = 400;
            }
            else
                movementSpeed = 200;

            cameraPosition.Center = position;

            FloatRect mousePos;
            mousePos.Left = mousePosition.X;
            mousePos.Top = mousePosition.Y;
            mousePos.Width = 4;
            mousePos.Height = 4;

            debugText.DisplayedString = "Mouse pos: " + mousePosition.X + ", " + mousePosition.Y + "\nIsGrabbed: " + isGrabbed;

            foreach (entity e in Game.entityList)
            {
                if (isGrabbed)
                {
                    if (e.myClass.name == entityGrabbedName && e.myClass.input == grabbedEntityInput)
                    {
                        if (Mouse.IsButtonPressed(Mouse.Button.Left))
                        {
                            e.myClass.position = mousePosition;
                            e.myClass.sprite.Position = mousePosition;
                        }
                        else
                            isGrabbed = false;
                    }
                }
                if ((Mouse.IsButtonPressed(Mouse.Button.Left) && mousePos.Intersects(e.myClass.sprite.GetGlobalBounds())) && !isGrabbed)
                {
                    isGrabbed = true;
                    grabbedEntityInput = e.myClass.input;
                    entityGrabbedName = e.myClass.name;
                    e.myClass.position = mousePosition;
                    e.myClass.sprite.Position = mousePosition;
                }
            }
        }

        public void exit()
        {
            Game.entityList[Game.GAME_PLAYER_INDEX].myClass.velocity = new Vector2f(0, 0);

            save("test.apscene");

            Game.deltaTime = 0;
            Game.deltaClock.Restart();
        }

        public void save(string sceneName)
        {
            string finalInput = "";
            try
            {
                foreach (string item in sceneData)
                {
                    finalInput += item + "\n";
                }

                foreach (entity e in Game.entityList)
                {
                    if (e.myClass.name.ToLower() == Game.GAME_MAIN_CHARACTER_NAME.ToLower())
                    {
                        finalInput += "\nentity=player";
                    }
                    else
                    {
                        finalInput += "\nentity=" + e.myClass.name;
                    }

                    switch (e.myClass.name.ToLower())
                    {
                        case "text":
                            finalInput += "\nparams=" + string.Join(">", new string[] { e.myClass.textString, e.myClass.position.X.ToString(), e.myClass.position.Y.ToString(), e.myClass.text.Scale.X.ToString() });
                            break;
                        case "endportal":
                            finalInput += "\nparams=" + string.Join(">", new string[] { e.myClass.position.X.ToString(), e.myClass.position.Y.ToString(), e.myClass.facingRight.ToString(), e.myClass.newScenePath });
                            break;
                        case "checkpoint":
                            finalInput += "\nparams=" + string.Join(">", new string[] { e.myClass.position.X.ToString(), e.myClass.position.Y.ToString() });
                            break;
                        case "decoration":
                            finalInput += "\nparams=" + string.Join(">", new string[] { e.myClass.spritePath, e.myClass.position.X.ToString(), e.myClass.position.Y.ToString() });
                            break;
                        case "dustsprite":
                            finalInput += "\nparams=" + string.Join(">", new string[] { e.myClass.position.X.ToString(), e.myClass.position.Y.ToString(), e.myClass.position2.ToString(), e.myClass.position3.ToString(), e.myClass.movementSpeed.ToString() });
                            break;
                        case "king":
                            finalInput += "\nparams=" + string.Join(">", new string[] { e.myClass.state });
                            break;
                        default:
                            if (e.myClass.name.ToLower() == Game.GAME_MAIN_CHARACTER_NAME.ToLower())
                            {
                                finalInput += "\nparams=" + string.Join(">", new string[] { e.myClass.position.X.ToString(), e.myClass.position.Y.ToString(), e.myClass.deathPlane.ToString() });
                            }
                            else
                                finalInput += "This entity has not been added to the scene editor! Please modify the scene manually or modify the scene saving code! Thanks!";
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                utils.LogError("Failed to write level to the scene! Exception: " + e);
            }

            File.WriteAllText("test.apscene", finalInput);

            utils.Log("Saved the scene successfully!");
        }

        public void render()
        {
            Game.getRenderWindow().Clear(Color.Black);

            Game.getRenderWindow().SetView(Game.uiCamera);
            Game.getRenderWindow().Draw(Game.GAME_MAIN_SKYBOX);

            //draw game world
            Game.getRenderWindow().SetView(cameraPosition);

            Game.getRenderWindow().Draw(Game.tilemap);

            foreach (entity e in Game.entityList)
            {
                Game.getRenderWindow().Draw(e.myClass.sprite);
            }

            Game.getRenderWindow().SetView(Game.uiCamera);
            Game.getRenderWindow().Draw(debugText);

            Game.getRenderWindow().Display();
        }
    }
}
