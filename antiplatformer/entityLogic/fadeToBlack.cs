using System;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class fadeToBlack
    {
        public int id = 5;
        public string name = "fadeToBlack";
        public Sprite sprite;
        public Vector2f position;
        public bool destroy = false;

        public int alphaValue;

        public int state = 0;

        private Clock fadeClock = new Clock();

        public string[] input = { };

        public fadeToBlack()
        {
            sprite = utils.loadSprite("res/misc/randomsprites/blackScreen.png");
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
            alphaValue = (fadeClock.ElapsedTime.AsMilliseconds() - 700) / 4;

            if (alphaValue < 1)
            {
                destroy = true;
            }

            sprite.Color = new Color(255, 255, 255, (byte)alphaValue);
        }

        public void onKill()
        {

        }
    }
}
