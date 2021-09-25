using System;
using System.Linq;
using System.Collections.Generic;
using TGUI;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace antiplatformer.ui
{
    public class SceneEditorUI : APGui
    {
        public override bool Clickable {get; set;}

        public SceneEditorUI(RenderWindow window, bool tab) : base(window, tab)
        {
            this.GetGUI().LoadWidgetsFromFile("res/gui/sceneeditor.gui");

            //add buttons and such
            foreach(ChildWindow w in GetGUI().Widgets.OfType<ChildWindow>())
            {
                if(w.Name == "entityEditor")
                {
                    Button spawn = (Button) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["spawn"]);

                    spawn.Clicked += (sender, e) =>
                    {
                        GetGUI().Widgets[GetUI.GetWidgets(GetGUI())["sWindow"]].Visible = true;
                        GetGUI().Widgets[GetUI.GetWidgets(GetGUI())["sWindow"]].Enabled = true;
                    };

                    Button kill = (Button) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["kill"]);

                    kill.Clicked += (sender, e) =>
                    {
                        try
                        {
                            foreach(entity.Entity ee in Program.antiPlatformer.entityManager.GetEntities())
                            {
                                if(ee.Name == Program.antiPlatformer.sceneEditor.grabbedEntityName)
                                {
                                    ee.OnKill();
                                    Program.antiPlatformer.entityManager.RemoveEntity(ee.Name);
                                }
                            }
                        }
                        catch(Exception eee)
                        {
                            Logger.LogWarn("Failed to kill entity. Is one selected? Exception: " + eee);
                        }
                    };
                }
                else if(w.Name == "levelData")
                {
                    Button render = (Button) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["render"]);

                    render.Clicked += (sender, e) =>
                    {
                        Manager.sceneRenderType = Manager.FlipBool(Manager.sceneRenderType);
                    };

                    Button reload = (Button) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["update"]);

                    reload.Clicked += (sender, e) =>
                    {
                        Program.antiPlatformer.sceneLoader.UpdateScene();
                    };
                }
                else
                {
                    Button spawn = (Button) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["eSpawn"]);

                    spawn.Clicked += (sender, e) =>
                    {
                        TextBox tb = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["eName"]);
                        TextBox par = (TextBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["params"]);
                        Program.antiPlatformer.entityManager.AddEntity(tb.Text, par.Text.Replace('\n', '>'));

                        GetGUI().Widgets[GetUI.GetWidgets(GetGUI())["sWindow"]].Visible = false;
                        GetGUI().Widgets[GetUI.GetWidgets(GetGUI())["sWindow"]].Enabled = false;
                    };

                }
            }
        }
    }
}