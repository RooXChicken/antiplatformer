using System.IO;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class placeholder
    {
        public string name = "ERROR";
        public int id = 999;
        public int gameState = 0;
        public JObject entityData = JObject.Parse(File.ReadAllText("res/entities/CHANGEME.json"));
        public bool destroy = false;

        public string[] input = { };

        public Vector2f position = new Vector2f(0, 0);
        public Vector2f velocity = new Vector2f(0, 0);
        public Sprite sprite;
        public int maxHealth = 0;
        public int health = 0;

        public placeholder(string paramaters)
        {
            input = paramaters.Split('>');
        }

        public Sprite getSprite() { return sprite; }

        public void parseInput()
        {

        }

        public void onSpawn()
        {
            utils.LogError("This message should not appear, if it does, well, something went horribly wrong");
        }

        public void update(float deltaTime)
        {
            //nothing else lol
        }

        public void onKill()
        {
            
        }
    }
}
