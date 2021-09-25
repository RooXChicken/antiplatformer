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
    public class PauseMenuUI : APGui
    {
        public override bool Clickable {get; set;}

        public PauseMenuUI(RenderWindow window, bool tab) : base(window, tab)
        {
            this.GetGUI().LoadWidgetsFromFile("res/gui/pause.gui");

            //add buttons and such
            foreach(ChildWindow w in GetGUI().Widgets.OfType<ChildWindow>())
            {
                Button resume = (Button) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["resume"]);

                resume.Clicked += (sender, e) =>
                {
                    if(Clickable)
                        Manager.paused = false;
                };

                Button options = (Button) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["options"]);

                options.Clicked += (sender, e) =>
                {
                    if(Clickable)
                        Logger.Log("Not added yet.");
                };

                Button quit = (Button) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["quit"]);

                quit.Clicked += (sender, e) =>
                {
                    if(Clickable)
                    {
                        antiplatformer.audio.MusicManager.StopMusic("generic");
                        Manager.paused = false;
                        Program.antiPlatformer.initTitle();
                    }
                };
            }
        }
    }
}