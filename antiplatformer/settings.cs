using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using TGUI;

namespace antiplatformer
{
    public class settings
    {
        private static string version;
        public static uint max_fps = 165;
        public static uint unfocused_fps = 30;
        public static float volume = 5.0f;

        public static IDictionary<string, SFML.Window.Keyboard.Key> keys = new Dictionary<string, SFML.Window.Keyboard.Key>();

        private static void createSettings()
        {
            Directory.CreateDirectory("user");
            File.Delete("user/settings.txt");
            var fs = new FileStream("settings.txt", FileMode.Create);
            fs.Dispose();
            File.WriteAllText("user/settings.txt", "version=" + gameManager.GAME_VERSION + "\nvolume=5.0\nmaxfps=0\nunfocusedfps=30");
            utils.Log("Created a new settings file");
        }

        public static void checkForUpdate()
        {
            ui.gui.LoadWidgetsFromFile("res/gui/update.gui");

            try
            {
                foreach(ChildWindow window in ui.gui.Widgets)
                {
                    foreach(Button item in window.Widgets.OfType<Button>())
                    {
                        if(item.Name == "done")
                        {
                            item.Pressed += (s, e) => ui.gui.RemoveAllWidgets();
                        }
                        else if(item.Name == "link")
                        {
                            item.Pressed += (s, e) => { gameManager.openUrl("https://github.com/RooXChicken/antiplatformer/releases"); ui.gui.RemoveAllWidgets(); };
                        }
                    }
                    foreach(TextBox item in window.Widgets.OfType<TextBox>())
                    {
                        if(item.Name == "details")
                        {
                            string text = Game.GAME_NETWORKING.sendGetString("update" + gameManager.GAME_VERSION, true, 2000);
                            if(text == "latest")
                            {
                                ui.gui.RemoveAllWidgets();
                            }
                            else
                            {
                                string[] actualtext = text.Split('<');
                                if(actualtext.Length != 0)
                                    item.Text = "";
                                foreach(string tex in actualtext)
                                {
                                    item.Text += tex + "\n";
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                ui.gui.RemoveAllWidgets();
            }
        }

        public static void loadSettings()
        {
            if(File.Exists("user/settings.txt"))
            {
                string[] settings = File.ReadAllLines("user/settings.txt");
                try
                {
                    foreach (string setting in settings)
                    {
                        if (setting == "regen=true")
                        {
                            createSettings();
                        }
                        else if (setting.Contains("volume="))
                        {
                            volume = float.Parse(setting.Substring(7));
                        }
                        else if (setting.Contains("maxfps="))
                        {
                            max_fps = UInt32.Parse(setting.Substring(7));
                        }
                        else if(setting.Contains("keys="))
                        {
                            string[] keyArr = setting.Split(',');

                            foreach(string key in keyArr)
                            {
                                //keys.Add(key, SFML.Window.Keyboard.)
                            }
                        }
                        else if (setting.Contains("unfocusedfps="))
                        {
                            unfocused_fps = UInt32.Parse(setting.Substring(13));
                        }
                        else if(setting.Contains("version="))
                        {
                            version = setting.Substring(8);
                        }
                    }
                }
                catch
                {
                    utils.LogError("Failed to load settings. Creating a new settings file.");
                }
            }
            else
            {
                createSettings();
            }
        }
    }
}