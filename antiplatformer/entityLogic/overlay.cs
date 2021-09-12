using System;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class overlay
    {
        public int id = 6;
        public string name = "overlay";
        public Sprite sprite;
        public Vector2f position;
        public bool destroy = false;

        public string[] input = { };

        public overlay()
        {

        }

        public Sprite getSprite() { return sprite; }

        public void onSpawn(string paramaters)
        {
            input = paramaters.Split('>');
            sprite = utils.loadSprite("res/misc/randomsprites/whiteScreen.png");
            sprite.Color = new Color(byte.Parse(input[0]), byte.Parse(input[1]), byte.Parse(input[2]), byte.Parse(input[3]));
        }

        public void update(float deltaTime)
        {
            sprite.Position = new Vector2f(Game.GAME_PLAYER_POSITION.X - 129, Game.GAME_PLAYER_POSITION.Y - 73.5f);
            sprite.Scale = new Vector2f(1.2f, 1.5f);
        }

        public void onKill()
        {

        }
    }
}
