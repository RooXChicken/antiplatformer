using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class decoration
    {
        public int id = 4;
        public string name = "decoration";
        public Sprite sprite;
        public string spritePath;
        public Vector2f position;
        public bool destroy = false;

        public string[] input = { };

        public decoration(string path)
        {
            input = path.Split('>');
            spritePath = input[2];
            sprite = utils.loadSprite(spritePath);

            sprite.Position = new Vector2f(float.Parse(input[0]), float.Parse(input[1]));
        }

        public Sprite getSprite() { return sprite; }

        public void parseInput()
        {
            spritePath = input[2];
            sprite = utils.loadSprite(spritePath);

            sprite.Position = new Vector2f(float.Parse(input[0]), float.Parse(input[1]));
        }

        public void onSpawn()
        {
            //nothing lol
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
