using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using TGUI;
using System.Linq;
using System.Threading;

namespace antiplatformer
{
    public class sceneEditor
    {
        public Sprite skybox;
        private static Text debugText;
        public string levelPath = "ERROR";
        View cameraPosition = new View(new Vector2f(0f, 0f), Game.GAME_INTERNAL_RESOLUTION);

        float movementSpeed = 200;

        string entityGrabbedName;
        int selectedEntityID = 0;
        string updatedParams = "ERROR";
        bool isGrabbed = false;

        int paramIndex = 0;

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

            debugText = new Text("", Game.GAME_MAIN_FONT);
            debugText.Scale = new Vector2f(0.15f, 0.15f);
            debugText.Position = new Vector2f(0, 1);

            Game.gui.LoadWidgetsFromFile("res/gui/sceneeditor.gui");

            //foreach (ChildWindow window in Game.gui.Widgets.OfType<ChildWindow>())
            //{
            //    foreach (Button item in window.Widgets.OfType<Button>())
            //    {
            //        item.Pressed += (sender, es) => {
            //            foreach (ChildWindow windoww in Game.gui.Widgets.OfType<ChildWindow>())
            //            {
            //                if (windoww.Name == "createEntity")
            //                {
            //                    windoww.Visible = true;
            //                }
            //            }
            //        };
            //    }
            //}

            foreach (ChildWindow window in Game.gui.Widgets.OfType<ChildWindow>())
            {
                foreach (TextBox item in window.Widgets.OfType<TextBox>())
                {
                    switch (item.Name)
                    {
                        case "path":
                            item.Text = Game.GAME_SCENE_MANAGER.levelPath;
                            break;
                        case "name":
                            item.Text = Game.GAME_SCENE_MANAGER.levelName;
                            break;
                        case "skybox":
                            item.Text = Game.GAME_SCENE_MANAGER.skyboxPath;
                            break;
                        case "music":
                            item.Text = Game.GAME_SCENE_MANAGER.musicPath;
                            break;

                    }
                }
            }

            Game.drpc.Update("Scene editor", "Working on a scene...");
        }

        public void update(float deltaTime)
        {
            mousePosition = Game.getRenderWindow().MapPixelToCoords(new Vector2i(Mouse.GetPosition(Game.getRenderWindow()).X, Mouse.GetPosition(Game.getRenderWindow()).Y));
            mousePosition.X = mousePosition.X + position.X - (Game.GAME_INTERNAL_RESOLUTION.X / 2) + 1;
            mousePosition.Y = mousePosition.Y + position.Y - (Game.GAME_INTERNAL_RESOLUTION.Y / 2);

            bool hasFocus = false;

            foreach (ChildWindow window in Game.gui.Widgets.OfType<ChildWindow>())
            {
                foreach (TextBox item in window.Widgets.OfType<TextBox>())
                {
                    if (item.Focus)
                    {
                        hasFocus = true;
                    }
                }
            }

            if(!hasFocus)
            {
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
            }

            cameraPosition.Center = position;

            FloatRect mousePos;
            mousePos.Left = mousePosition.X;
            mousePos.Top = mousePosition.Y;
            mousePos.Width = 4;
            mousePos.Height = 4;

            debugText.DisplayedString = "Mouse pos: " + mousePosition.X + ", " + mousePosition.Y + "\nIsGrabbed: " + isGrabbed;

            int index = 0;
            try
            {
                foreach (entity e in Game.entityList)
                {
                    if (e.myClass.name == entityGrabbedName && selectedEntityID == index)
                    {
                        foreach (ChildWindow window in Game.gui.Widgets.OfType<ChildWindow>())
                        {
                            foreach (TextBox item in window.Widgets.OfType<TextBox>())
                            {
                                if(item.Name == "entity")
                                {
                                    if (grabbedEntityInput != item.Text.Split('\n'))
                                    {
                                        item.Text.Split('\n');
                                        e.myClass.input = item.Text.Split('\n');
                                        e.myClass.parseInput();
                                    }
                                }
                            }
                        }

                        if (isGrabbed)
                        {
                            if (Mouse.IsButtonPressed(Mouse.Button.Left))
                            {
                                e.myClass.position = mousePosition;
                                e.myClass.sprite.Position = mousePosition;
                                foreach (ChildWindow window in Game.gui.Widgets.OfType<ChildWindow>())
                                {
                                    foreach (TextBox item in window.Widgets.OfType<TextBox>())
                                    {
                                        if(item.Name == "entity")
                                        {
                                            item.Text = "";
                                            int indexx = 0;
                                            foreach (string i in e.myClass.input)
                                            {
                                                if (indexx == 0)
                                                    item.Text += e.myClass.position.X.ToString() + "\n";
                                                else if (indexx == 1)
                                                    item.Text += e.myClass.position.Y.ToString() + "\n";
                                                else
                                                    if (i != "")
                                                    item.Text += i + "\n";
                                                indexx++;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                isGrabbed = false;
                            }
                        }
                    }

                    if ((Mouse.IsButtonPressed(Mouse.Button.Left) && mousePos.Intersects(e.myClass.sprite.GetGlobalBounds())) && !isGrabbed)
                    {
                        isGrabbed = true;
                        grabbedEntityInput = e.myClass.input;
                        entityGrabbedName = e.myClass.name;
                        e.myClass.position = mousePosition;
                        e.myClass.sprite.Position = mousePosition;
                        selectedEntityID = index;
                        foreach (ChildWindow window in Game.gui.Widgets.OfType<ChildWindow>())
                        {
                            foreach (TextBox item in window.Widgets.OfType<TextBox>())
                            {
                                if(item.Name == "entity")
                                {
                                    item.Text = "";
                                    foreach (string i in e.myClass.input)
                                    {
                                        if (i != "")
                                            item.Text += i + "\n";
                                    }
                                }
                            }
                        }
                    }

                    if (!Mouse.IsButtonPressed(Mouse.Button.Left))
                        isGrabbed = false;

                    index++;
                }
            }
            catch (Exception e)
            {
                utils.LogError("Failed to update the scene editor. Exception: " + e);
            }
        }

        public void exit()
        {
            Game.entityList[Game.GAME_PLAYER_INDEX].myClass.velocity = new Vector2f(0, 0);
            selectedEntityID = 0;
            isGrabbed = false;
            grabbedEntityInput = new string[1];
            entityGrabbedName = "ERROR";

            save("test.apscene");

            Game.gui.RemoveAllWidgets();

            Game.deltaTime = 0;
            Game.deltaClock.Restart();
        }

        public void save(string sceneName)
        {
            string finalInput = "";
            try
            {
                foreach (ChildWindow window in Game.gui.Widgets.OfType<ChildWindow>())
                {
                    foreach (TextBox item in window.Widgets.OfType<TextBox>())
                    {
                        switch (item.Name)
                        {
                            case "path":
                                finalInput += "level=" + item.Text + "\n";
                                break;
                            case "name":
                                finalInput += "levelname=" + item.Text + "\n";
                                break;
                            case "skybox":
                                finalInput += "skybox=" + item.Text + "\n";
                                break;
                            case "music":
                                finalInput += "music=" + item.Text + "\n";
                                break;
                        }
                    }
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
                            finalInput += "\nparams=" + string.Join(">", new string[] { e.myClass.position.X.ToString(), e.myClass.position.Y.ToString(), e.myClass.textString, e.myClass.text.Scale.X.ToString() });
                            break;
                        case "endportal":
                            finalInput += "\nparams=" + string.Join(">", new string[] { e.myClass.position.X.ToString(), e.myClass.position.Y.ToString(), e.myClass.facingRight.ToString(), e.myClass.newScenePath });
                            break;
                        case "checkpoint":
                            finalInput += "\nparams=" + string.Join(">", new string[] { e.myClass.position.X.ToString(), e.myClass.position.Y.ToString() });
                            break;
                        case "decoration":
                            finalInput += "\nparams=" + string.Join(">", new string[] { e.myClass.position.X.ToString(), e.myClass.position.Y.ToString(), e.myClass.spritePath });
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

            foreach (ChildWindow window in Game.gui.Widgets.OfType<ChildWindow>())
            {
                foreach (TextBox item in window.Widgets.OfType<TextBox>())
                {
                    if (item.Name == "save")
                    {
                        if (item.Text != "")
                        {
                            File.WriteAllText(item.Text, finalInput);
                        }
                        else
                        {
                            utils.Log("Please specify a scene path! Not saving any changes!");
                        }
                    }
                }
            }

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
            Game.gui.Draw();

            Game.getRenderWindow().Display();
        }
    }
}
