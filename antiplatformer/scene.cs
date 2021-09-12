using System;
using System.Collections.Generic;
using System.IO;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer
{
    public class scene
    {
        public string levelPath = "PLACEHOLDER";
        public string currentScene = "PLACEHOLDER";
        private string levelName = "ERROR";

        public scene()
        {

        }

        public void reloadScene()
        {
            loadScene(currentScene);
        }

        public void loadScene(string path)
        {
            Game.entityList = new List<entity>();
            currentScene = path;
            string[] items = File.ReadAllLines(path);
            int index = 0;
            foreach(string item in items)
            {
                if(item.Contains("level="))
                {
                    levelPath = item.Substring(6);
                    Game.tilemap.loadMap(levelPath);
                    utils.Log("Loaded level with path: " + levelPath);
                }
                else if(item.Contains("levelname="))
                {
                    Game.drpc.Update("Singleplayer V" + Game.GAME_VERSION, "In level " + item.Substring(10));
                }
                else if(item.Contains("music="))
                {
                    if(item == "music=none")
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
                    }
                    else
                    {
                        string[] input = item.Split('>');
                        Game.GAME_CURRENT_MUSIC = new Music(input[0].Substring(6));
                        Game.GAME_CURRENT_MUSIC.Volume = Game.GAME_MAIN_AUDIO_VOLUME;
                        Game.GAME_CURRENT_MUSIC.Loop = Boolean.Parse(input[1]);
                    }
                }
                else if(item.Contains("skybox="))
                {
                    Game.GAME_MAIN_SKYBOX = utils.loadSprite(item.Substring(7));
                    Game.GAME_MAIN_SKYBOX.Position = new Vector2f(-1, 0);
                }
                else if(item.Contains("entity="))
                {
                    Game.entityList.Add(new entity(item.Substring(7), items[index + 1].Substring(7)));
                    utils.Log("Loaded entity with data: " + item.Substring(7) + " | " + items[index + 1].Substring(7));
                }
                index++;
            }

            if (Game.GAME_CURRENT_MUSIC != null)
            {
                Game.GAME_CURRENT_MUSIC.Play();
            }

            int indexA = 0;
            foreach (entity e in Game.entityList)
            {
                try
                {
                    if (e.myClass.id == 1)
                    {
                        Game.GAME_PLAYER_INDEX = indexA;
                    }
                    indexA++;
                }
                catch
                {
                    utils.LogError("Entity does not have tag id, whoops :P");
                }
            }

            Game.entityList[Game.GAME_PLAYER_INDEX].myClass.getTilemap(Game.tilemap);

            Game.deltaClock.Restart();
            Game.deltaTime = 0;

            Game.isLoading = false;
        }
    }
}
