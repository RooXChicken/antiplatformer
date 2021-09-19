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
        public string levelName = "ERROR";
        public string skyboxPath = "ERROR";
        public string musicPath = "none";

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
                    levelName = item.Substring(10);
                    gameManager.drpc.Update("Singleplayer V" + gameManager.GAME_VERSION, "In level " + item.Substring(10));
                }
                else if(item.Contains("music="))
                {
                    if(item == "music=none")
                    {
                        audioManager.stopMusic("generic");
                    }
                    else
                    {
                        string[] input = item.Split('>');
                        musicPath = input[0].Substring(6);
                        audioManager.loadSong(input[0].Substring(6), "generic", Boolean.Parse(input[1]));
                    }
                }
                else if(item.Contains("skybox="))
                {
                    Game.GAME_SKYBOX = utils.loadSprite(item.Substring(7));
                    skyboxPath = item.Substring(7);
                    Game.GAME_SKYBOX.Position = new Vector2f(-1, 0);
                }
                else if(item.Contains("entity="))
                {
                    Game.entityList.Add(new entity(item.Substring(7), items[index + 1].Substring(7)));
                    utils.Log("Loaded entity with data: " + item.Substring(7) + " | " + items[index + 1].Substring(7));
                }
                index++;
            }

            audioManager.playMusic("generic");

            Game.GAME_PLAYER_INDEX = 0;
            Game.GAME_PLAYER_INDEX = Game.entityList.Count - 1;

            Game.entityList[Game.GAME_PLAYER_INDEX].myClass.getTilemap(Game.tilemap);

            gameManager.deltaClock.Restart();
            gameManager.deltaTime = 0;

            gameManager.isLoading = false;
        }
    }
}
