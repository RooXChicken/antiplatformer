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
    public class OptionsUI : APGui
    {
        public override bool Clickable {get; set;}

        public OptionsUI(RenderWindow window, bool tab) : base(window, tab)
        {
            this.GetGUI().LoadWidgetsFromFile("res/gui/options.gui");

            //add buttons and such
            foreach(ChildWindow w in GetGUI().Widgets.OfType<ChildWindow>())
            {
                CheckBox fullscreen = (CheckBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["fullscreen"]);

                fullscreen.Clicked += (sender, e) =>
                {
                    if(Clickable)
                        if(fullscreen.Checked)
                        {
                            Manager.ToggleFullscreen(window);
                        }
                        else
                        {
                            Manager.ToggleFullscreen(window);
                        }
                };

                CheckBox focus = (CheckBox) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["focus"]);

                focus.Clicked += (sender, e) =>
                {
                    if(Clickable)
                        Logger.Log("Not added yet.");
                };

                Slider vol = (Slider) w.Widgets.ElementAt(GetUI.GetWidgetsFromWindow(w)["volume"]);

                vol.ValueChanged += (sender, e) =>
                {
                    if(Clickable)
                    {
                        Settings.Volume = vol.Value;
                    }
                };
            }
        }
    }
}