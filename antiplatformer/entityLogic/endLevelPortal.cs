using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class endLevelPortal
    {
        public string name = "EndPortal";
        public int id = 9;
        public int gameState = 0;
        public bool destroy = false;
        public bool facingRight = false;

        public string newScenePath = "NULL";

        public string[] input = { };

        public Vector2f position = new Vector2f(0, 0);
        public Vector2f velocity = new Vector2f(0, 0);
        public Sprite sprite = utils.loadSprite("res/misc/randomsprites/decorations/portal.png");
        public int maxHealth = 0;
        public int health = 0;

        public endLevelPortal(string paramaters)
        {
            input = paramaters.Split('>');

            position = new Vector2f(float.Parse(input[0]), float.Parse(input[1]));
            sprite.Position = position;
            if(!Boolean.Parse(input[2]))
            {
                facingRight = false;
                sprite.Scale = new Vector2f(-1, 1);
            }
            else
            {
                facingRight = true;
            }
            newScenePath = input[3];
        }

        public Sprite getSprite() { return sprite; }

        public void parseInput()
        {

        }

        public void onSpawn()
        {
            //nothing lol
        }

        public void update(float deltaTime)
        {
            if(sprite.GetGlobalBounds().Intersects(Game.entityList[Game.GAME_PLAYER_INDEX].myClass.sprite.GetGlobalBounds()))
            {
                Game.GAME_PLAYER_INDEX = 0;
                Game.GAME_SCENE_MANAGER.loadScene(newScenePath);
                destroy = true;
            }
        }

        public void onKill()
        {
            
        }
    }
}
