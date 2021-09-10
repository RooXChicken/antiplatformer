using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class decoration
    {
        public int id = 4;
        public string name = "decoration";
        public Sprite decor;
        public Vector2f position;
        public bool destroy = false;

        public decoration(string path)
        {
            string[] input = path.Split('>');
            decor = utils.loadSprite(input[0]);

            decor.Position = new Vector2f(float.Parse(input[1]), float.Parse(input[2]));
        }

        public Sprite getSprite() { return decor; }

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
