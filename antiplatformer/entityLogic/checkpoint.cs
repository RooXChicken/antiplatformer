using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class checkpoint
    {
        public string name = "Checkpoint";
        public int id = 8;
        public int gameState = 0;
        public bool destroy = false;

        public string[] input = { };

        public Vector2f position = new Vector2f(0, 0);
        public Vector2f velocity = new Vector2f(0, 0);
        public Sprite sprite = utils.loadSprite("res/sprites/common/empty.png");
        public int maxHealth = 0;
        public int health = 0;

        public checkpoint(string paramaters)
        {
            input = paramaters.Split('>');

            position = new Vector2f(float.Parse(input[0]), float.Parse(input[1]));
        }

        public Sprite getSprite() { return sprite; }

        public void onSpawn()
        {

        }

        public void update(float deltaTime)
        {
            if (Game.GAME_PLAYER_POSITION.X > Int32.Parse(input[0]))
            {
                destroy = true;
                if (Game.entityList[Game.GAME_PLAYER_INDEX].myClass.currentCheckpoint.X < position.X)
                {
                    Game.entityList[Game.GAME_PLAYER_INDEX].myClass.currentCheckpoint = position;
                }
            }
        }

        public void onKill()
        {
            
        }
    }
}
