using System.Collections.Generic;
using TGUI;

namespace antiplatformer.ui
{
    public class GetUI
    {
        public static Dictionary<string, int> GetWidgetsFromWindow(ChildWindow window) {
            Dictionary<string, int> widgets = new Dictionary<string, int>();

            for (int i = 0; i < window.Widgets.Count; i++) {
                widgets.Add(window.Widgets[i].Name, i);
            }

            return widgets;
        }

        public static Dictionary<string, int> GetWidgets(Gui gui) {
            Dictionary<string, int> widgets = new Dictionary<string, int>();

            for (int i = 0; i < gui.Widgets.Count; i++) {
                widgets.Add(gui.Widgets[i].Name, i);
            }

            return widgets;
        }
    }
}