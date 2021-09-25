using System;
using System.Collections.Generic;
using System.IO;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

using antiplatformer.audio;

namespace antiplatformer
{
    public class SceneLoader
    {
        public string LevelPath = "PLACEHOLDER";
        public string CurrentScene = "PLACEHOLDER";
        public string LevelName = "ERROR";
        public string SkyboxPath = "ERROR";
        public string MusicPath = "none";

        public void ReloadScene()
        {
            LoadScene(CurrentScene);
        }

        public void UpdateScene()
        {
            int index = 0;
            Program.antiPlatformer.loadedLevel.LoadMapFromString(LevelPath);
            if(MusicPath == "none")
            {
                MusicManager.StopMusic("generic");
            }
            else
            {
                MusicManager.LoadSong(MusicPath, "generic", true);
            }
            Program.antiPlatformer.skybox = Loader.LoadSprite(SkyboxPath);
            Program.antiPlatformer.skybox.Position = new Vector2f(-1, 0);

            MusicManager.PlayMusic("generic");

            Manager.RestartDeltaTime();
        }

        public void LoadScene(string path)
        {
            Program.antiPlatformer.entityManager.Clear();
            CurrentScene = path;
            string[] items = File.ReadAllLines(path);
            int index = 0;
            foreach(string item in items)
            {
                if(item.Contains("level="))
                {
                    LevelPath = item.Substring(6);
                    Program.antiPlatformer.loadedLevel.LoadMapFromString(LevelPath);
                    //Logger.Log("Loaded level with path: " + LevelPath);
                }
                else if(item.Contains("levelname="))
                {
                    LevelName = item.Substring(10);
                    //gameManager.drpc.Update("Singleplayer V" + gameManager.GAME_VERSION, "In level " + item.Substring(10));
                }
                else if(item.Contains("music="))
                {
                    if(item == "music=none")
                    {
                        MusicManager.StopMusic("generic");
                    }
                    else
                    {
                        string[] input = item.Split('>');
                        MusicPath = input[0].Substring(6);
                        MusicManager.LoadSong(input[0].Substring(6), "generic", Boolean.Parse(input[1]));
                    }
                }
                else if(item.Contains("skybox="))
                {
                    Program.antiPlatformer.skybox = Loader.LoadSprite(item.Substring(7));
                    SkyboxPath = item.Substring(7);
                    Program.antiPlatformer.skybox.Position = new Vector2f(-1, 0);
                }
                else if(item.Contains("entity="))
                {
                    Program.antiPlatformer.entityManager.AddEntity(item.Substring(7), items[index + 1].Substring(7));
                    Logger.Log("Loaded entity with data: " + item.Substring(7) + " | " + items[index + 1].Substring(7));
                }
                index++;
            }

            MusicManager.PlayMusic("generic");

            Manager.RestartDeltaTime();

            //Program.antiPlatformer. = false;
        }
    }
}