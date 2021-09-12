using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class dustSprite
    {
        public string name = "DustSprite";
        public int id = 10;
        public int gameState = 0;
        public bool destroy = false;

        public string[] input = { };

        public Vector2f position = new Vector2f(0, 0);
        public float position2 = 0;
        public float position3 = 0;
        public Vector2f velocity = new Vector2f(0, 0);
        public Sprite sprite = utils.loadSprite("res/sprites/enemies/dustSprite.png");
        public float movementSpeed = 0;
        public int maxHealth = 0;
        public int health = 0;

        public bool direction = true;

        public dustSprite(string paramaters)
        {
            input = paramaters.Split('>');

            position = new Vector2f(float.Parse(input[0]) + 1, float.Parse(input[1]));
            position2 = float.Parse(input[2]);
            position3 = float.Parse(input[3]);
            movementSpeed = float.Parse(input[4]);
            sprite.Position = position;
        }

        public Sprite getSprite() { return sprite; }

        public void onSpawn()
        {
            //nothing lol
        }

        public void update(float deltaTime)
        {
            velocity.X = 0;
            if(sprite.GetGlobalBounds().Intersects(Game.entityList[Game.GAME_PLAYER_INDEX].myClass.sprite.GetGlobalBounds()))
            {
                if(Game.entityList[Game.GAME_PLAYER_INDEX].myClass.position.Y - (sprite.Position.Y - sprite.GetGlobalBounds().Height) < 0 && Game.entityList[Game.GAME_PLAYER_INDEX].myClass.velocity.Y > 0)
                {
                    Game.entityList[Game.GAME_PLAYER_INDEX].myClass.velocity = new Vector2f(0, -120);
                    Game.entityList[Game.GAME_PLAYER_INDEX].myClass.isDiving = false;
                    Game.entityList[Game.GAME_PLAYER_INDEX].myClass.isJumping = true;
                    destroy = true;
                }
                else
                {
                    Game.entityList[Game.GAME_PLAYER_INDEX].myClass.onDamage(1);
                }
            }
            move(deltaTime);
        }

        public void move(float deltaTime)
        {
            if(position.X < position2 && direction)
            {
                velocity.X += movementSpeed;
            }
            else if(position.X > position3)
            {
                direction = false;
                sprite.Scale = new Vector2f(-1, 1);
                velocity.X += -movementSpeed;
                sprite.Origin = new Vector2f(5, 0);

            }
            else
            {
                direction = true;
                sprite.Scale = new Vector2f(1, 1);
                sprite.Origin = new Vector2f(0, 0);
            }

            position.X += velocity.X * deltaTime;
            position.Y += velocity.Y * deltaTime;

            sprite.Position = position;
        }

        public void onKill()
        {
            
        }
    }
}
