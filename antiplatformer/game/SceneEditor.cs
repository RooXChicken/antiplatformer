using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using TGUI;
using System.Linq;
using System.Threading;

using antiplatformer.audio;
using antiplatformer.ui;

namespace antiplatformer
{
    public class SceneEditor
    {
        public bool unsavedChanges = false;
        public bool hasSaved = false;
        public Sprite skybox;
        private static Text debugText;
        public string levelPath = "ERROR";
        View cameraPosition = new View(new Vector2f(0f, 0f), new Vector2f(256, 144));

        float movementSpeed = 200;

        public string entityGrabbedName = "ERROR";
        int selectedEntityID = 0;
        string updatedParams = "ERROR";
        bool isGrabbed = false;

        int paramIndex = 0;

        public Vector2f position = new Vector2f(0, 0);
        Vector2f mousePosition;

        SceneEditorUI ui;

        public string grabbedEntityName = "ERROR";
        public string[] grabbedEntityInput = {""};

        public void Init(RenderWindow renderWindow)
        {
            hasSaved = false;
            ui = new SceneEditorUI(renderWindow, true);
            MusicManager.StopMusic("generic");

            debugText = new Text("", Manager.font);
            debugText.Scale = new Vector2f(0.15f, 0.15f);
            debugText.Position = new Vector2f(0, 1);

            ChildWindow w = (ChildWindow) ui.GetWidget("levelData");

            TextBox path = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["path"]);
            path.Text = Program.antiPlatformer.sceneLoader.LevelPath;

            TextBox name = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["levelName"]);
            name.Text = Program.antiPlatformer.sceneLoader.LevelName;

            TextBox skybox = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["skybox"]);
            skybox.Text = Program.antiPlatformer.sceneLoader.SkyboxPath;

            TextBox music = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["music"]);
            music.Text = Program.antiPlatformer.sceneLoader.MusicPath;

            position = Program.antiPlatformer.entityManager.GetEntity("Player0").Position;

            //Manager.drpc.Update("Scene editor", "Working on a scene...");
        }

        public void Update(RenderWindow window)
        {
            bool hasFocus = true;

            ChildWindow ww = (ChildWindow) ui.GetWidget("levelData");

            TextBox path = (TextBox) ww.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(ww)["path"]);
            TextBox name = (TextBox) ww.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(ww)["levelName"]);
            TextBox skybox = (TextBox) ww.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(ww)["skybox"]);
            TextBox music = (TextBox) ww.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(ww)["music"]);

            Program.antiPlatformer.sceneLoader.LevelName = name.Text;
            Program.antiPlatformer.sceneLoader.LevelPath = path.Text;
            Program.antiPlatformer.sceneLoader.SkyboxPath = skybox.Text;
            Program.antiPlatformer.sceneLoader.MusicPath = music.Text;

            ChildWindow w = (ChildWindow) ui.GetWidget("entityEditor");
            TextBox entity = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["data"]);
            TextBox eName = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["name"]);
            TextBox eType = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["type"]);

            #region bruh.

            if(path.Focus)
            {
                hasFocus = true;
            }
            else if(name.Focus)
            {
                hasFocus = true;
            }
            else if(skybox.Focus)
            {
                hasFocus = true;
            }
            else if(music.Focus)
            {
                hasFocus = true;
            }
            else if(entity.Focus)
            {
                hasFocus = true;
            }
            else if(eName.Focus)
            {
                hasFocus = true;
            }
            else if(eType.Focus)
            {
                hasFocus = true;
            }
            else
            {
                hasFocus = false;
            }

            #endregion

            if(!hasFocus)
            {
                if (GameInput.GetKeyDown(Keyboard.Key.LControl))
                {
                    movementSpeed = 400;
                }
                else
                    movementSpeed = 200;

                position.X += (movementSpeed * GameInput.GetHorizontal()) * Manager.GetDeltaTime();
                position.Y += (movementSpeed * GameInput.GetVertical()) * Manager.GetDeltaTime();
            }
            else
            {
                unsavedChanges = true;
            }

            mousePosition = window.MapPixelToCoords(new Vector2i(Mouse.GetPosition(window).X, Mouse.GetPosition(window).Y));
            mousePosition.X = mousePosition.X + position.X - (256 / 2) + 1;
            mousePosition.Y = mousePosition.Y + position.Y - (144 / 2);

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
                foreach (entity.Entity e in Program.antiPlatformer.entityManager.GetEntities())
                {
                    if (e.Name == entityGrabbedName)
                    {
                        if (grabbedEntityInput != entity.Text.Split('\n'))
                        {
                            grabbedEntityInput = entity.Text.Split('\n');
                            e.Input = entity.Text.Split('\n');
                            e.ParseInput(entity.Text.Replace('\n', '>'));
                        }

                        if (isGrabbed)
                        {
                            if (Mouse.IsButtonPressed(Mouse.Button.Left))
                            {
                                e.Position = mousePosition;
                                e.Sprite.Position = mousePosition;
                                entity.Text = "";
                                int indexx = 0;
                                foreach (string i in e.Input)
                                {
                                    if (indexx == 0)
                                        entity.Text += e.Position.X.ToString() + "\n";
                                    else if (indexx == 1)
                                        entity.Text += e.Position.Y.ToString() + "\n";
                                    else
                                        if (i != "")
                                        entity.Text += i + "\n";
                                    indexx++;
                                }
                            }
                            else
                            {
                                isGrabbed = false;
                            }
                        }
                    }

                    if ((Mouse.IsButtonPressed(Mouse.Button.Left) && mousePos.Intersects(e.Sprite.GetGlobalBounds())) && !isGrabbed)
                    {
                        unsavedChanges = true;
                        isGrabbed = true;
                        entityGrabbedName = e.Name;
                        e.Position = mousePosition;
                        e.Sprite.Position = mousePosition;
                        eName.Text = e.Name;
                        eType.Text = e.GetType().ToString().Substring(31);
                        selectedEntityID = index;
                        entity.Text = "";
                        foreach (string i in e.Input)
                        {
                            if (i != "")
                                entity.Text += i + "\n";
                        }
                    }

                    if (!Mouse.IsButtonPressed(Mouse.Button.Left))
                        isGrabbed = false;

                    index++;
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to update the scene editor. Exception: " + e);
            }
        }

        public void Exit()
        {
            if(unsavedChanges)
            {
                ui.GetWidget("unsaved").Enabled = true;
                ui.GetWidget("unsaved").Visible = true;
            }
            else
            {
                hasSaved = true;
            }

            while(!hasSaved)
            {
                Program.antiPlatformer.update();
                Program.antiPlatformer.render();
            }
        }

        public void Unload()
        {
            selectedEntityID = 0;
            isGrabbed = false;
            grabbedEntityInput = new string[1];
            entityGrabbedName = "ERROR";

            //Save("");

            ui.Unload();

            Program.antiPlatformer.GAME_STATE = 3;
            Manager.RestartDeltaTime();
        }

        public void Save(string sceneName)
        {
            hasSaved = true;
            string finalInput = "";
            ChildWindow w = (ChildWindow) ui.GetWidget("levelData");
            try
            {
                TextBox path = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["path"]);
                finalInput += "level=" + path.Text + "\n";

                TextBox name = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["levelName"]);
                finalInput += "levelname=" + name.Text + "\n";

                TextBox skybox = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["skybox"]);
                finalInput += "skybox=" + skybox.Text + "\n";

                TextBox music = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["music"]);
                finalInput += "music=" + music.Text + "\n";

                foreach (entity.Entity e in Program.antiPlatformer.entityManager.GetEntities())
                {
                    if (e.Name == "Player0")
                    {
                        finalInput += "\nentity=Player";
                    }
                    else
                    {
                        finalInput += "\nentity=" + Regex.Replace(e.Name, @"[\d-]", string.Empty).ToString();
                    }

                    finalInput += "\nparams=";

                    int amount = e.Input.Length - 1;
                    int index = 0;

                    foreach (string i in e.Input)
                    {
                        if(amount != index)
                        {
                            if (i != "")
                                finalInput += i + ">";
                        }
                        else
                        {
                            if (i != "")
                                finalInput += i;
                        }
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to write level to the scene! Exception: " + e);
            }

            if (sceneName != "")
            {
                File.WriteAllText(sceneName, finalInput);
            }
            else
            {
                Logger.Log("Please specify a scene path! Not saving any changes!");
            }

            Logger.Log("Saved the scene successfully!");
        }

        public void Render(RenderWindow renderWindow)
        {
            renderWindow.Clear();

            renderWindow.SetView(Program.antiPlatformer.uiCamera);
            renderWindow.Draw(Program.antiPlatformer.skybox);

            //draw game world
            renderWindow.SetView(cameraPosition);

            renderWindow.Draw(Program.antiPlatformer.loadedLevel);

            if(Manager.sceneRenderType)
            {
                foreach (entity.Entity e in Program.antiPlatformer.entityManager.GetEntities())
                {
                    renderWindow.Draw(e.Sprite);
                }
            }
            else
            {
                foreach (entity.Entity e in Program.antiPlatformer.entityManager.GetEntities())
                {
                    e.Render(renderWindow);
                }
            }

            renderWindow.SetView(Program.antiPlatformer.uiCamera);
            renderWindow.Draw(debugText);
            ui.GetGUI().Draw();

            renderWindow.Display();
        }
    }
}