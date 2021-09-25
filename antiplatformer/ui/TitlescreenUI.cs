using System;
using System.Collections.Generic;
using TGUI;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace antiplatformer.ui
{
    public class TitlescreenUI : APGui
    {
        public override bool Clickable {get; set;}

        public TitlescreenUI(RenderWindow window, bool tab) : base(window, tab)
        {
            this.GetGUI().LoadWidgetsFromFile("res/gui/titlescreen.gui");

            //add buttons and such
            //ChildWindow window = (ChildWindow) GetGUI().Widgets[GetWidgets()[""]]
            Clickable = true;
            Button start = (Button) GetGUI().Widgets[GetUI.GetWidgets(GetGUI())["start"]];

            start.Clicked += (sender, e) =>
            {
                Program.antiPlatformer.initMain(3);
            };

            Button multiplayer = (Button) GetGUI().Widgets[GetUI.GetWidgets(GetGUI())["multiplayer"]];

            multiplayer.Clicked += (sender, e) =>
            {
                //Program.antiPlatformer.initMain(3);
            };

            Button options = (Button) GetGUI().Widgets[GetUI.GetWidgets(GetGUI())["settings"]];

            options.Clicked += (sender, e) =>
            {
                Logger.LogWarn("It's not added yet I even told you ya doofus");
            };

            Button quit = (Button) GetGUI().Widgets[GetUI.GetWidgets(GetGUI())["quit"]];

            quit.Clicked += (sender, e) =>
            {
                Program.antiPlatformer.window.Close();
            };
        }
    }
}