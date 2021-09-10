using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class splashText
    {
        public int id = 2;
        public string name = "splashScreen";
        public Sprite sprite;
        public Vector2f position;
        public bool destroy = false;

        private Clock splashClock = new Clock();

        public splashText()
        {

        }

        public Sprite getSprite() { return sprite; }

        public void onSpawn()
        {
            sprite = utils.loadSprite("res/sprites/splashText.png");
            sprite.Position = new Vector2f(Game.GAME_INTERNAL_RESOLUTION.X / 3, Game.GAME_INTERNAL_RESOLUTION.Y / 2.25f);
        }

        public void update(float deltaTime)
        {
            int alphaValue = (splashClock.ElapsedTime.AsMilliseconds() - 700) / 4;

            if (alphaValue > 255)
            {
                alphaValue = 255;
            }
            if (alphaValue < 0)
            {
                alphaValue = 0;
            }

            sprite.Color = new Color(255, 255, 255, (byte)alphaValue);
        }

        public void onKill()
        {
            //nada lol
        }
    }
}
