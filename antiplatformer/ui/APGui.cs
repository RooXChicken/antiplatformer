using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TGUI;

namespace antiplatformer.ui
{
    public abstract class APGui
    {
        private Gui GUI;
        public RenderWindow Window;
        public abstract bool Clickable {get; set;}

        public APGui(RenderWindow window, bool tab)
        {
            GUI = new Gui(window);
            GUI.TabKeyUsageEnabled = tab;
            this.Window = window;
        }

        public APGui(RenderWindow window, string path)
        {
            GUI = new Gui(window);
            GUI.TabKeyUsageEnabled = false;
            GUI.LoadWidgetsFromFile(path);

            this.Window = window;
        }

        public APGui(Gui gui)
        {
            this.GUI = gui;
        }

        public virtual void Update()
        {
            GUI.Draw();
        }

        public virtual void Unload()
        {
            GUI.RemoveAllWidgets();
        }

        public virtual void SetRenderWindow(RenderWindow renderWindow)
        {
            Window = renderWindow;
            GUI.Target = renderWindow;
        }

        public Gui GetGUI()
        {
            return GUI;
        }

        public Dictionary<String, int> GetWidgets() {
            Dictionary<String, int> widgets = new Dictionary<string, int>();

            for (int i = 0; i < GUI.Widgets.Count; i++) {
                widgets.Add(GUI.Widgets[i].Name, i);
            }

            return widgets;
        }

        public Widget GetWidget(string widgetName) {
            return GetGUI().Widgets[GetWidgets()[widgetName]];
        }
    }
}