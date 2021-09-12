using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class errorEntity
    {

        public string name = "ERROR";
        public int id = 0;
        public bool destroy = false;

        public string[] input = { };

        public Vector2f position = new Vector2f(0, 0);
        public Vector2f velocity = new Vector2f(0, 0);
        public Sprite sprite;
        public int maxHealth = 0;
        public int health = 0;

        public errorEntity()
        {

        }

        public SFML.Graphics.Sprite getSprite() { return sprite; }

        public void onSpawn()
        {
            sprite = utils.loadSprite("res/missing.png");

            utils.LogError("Hello there player! This is the error entity (hi :D) This message should not appear unless something broke, which in this case, it did. Let me know, will ya!");
        }

        public void update(float deltaTime)
        {

        }

        public void onKill()
        {

        }
    }
}
