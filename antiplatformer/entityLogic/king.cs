using System;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class king
    {
        public int id = 7;
        public string name = "King";
        public Sprite sprite;
        public Vector2f position;
        public bool destroy = false;

        public string[] input = { };

        public bool isFacingRight = true;

        Clock anim = new Clock();

        public string state = "PLACEHOLDER";

        animation intro = new animation("res/sprites/boss/king/intro/", false);

        public king()
        {

        }

        public Sprite getSprite() { return sprite; }

        public void parseInput()
        {

        }

        public void onSpawn(string paramaters)
        {
            state = paramaters;
            sprite = utils.loadSprite("res/sprites/boss/king/idle/idle.png");
        }

        public void update(float deltaTime)
        {
            switch(state)
            {
                case "intro":
                    sprite.Position = new Vector2f(800.0f, 50);
                    if(isFacingRight)
                    {
                        sprite.Origin = new Vector2f(0, 0);
                        sprite.Scale = new Vector2f(1.0f, 1.0f);
                    }
                    else
                    {
                        sprite.Origin = new Vector2f(20, 0);
                        sprite.Scale = new Vector2f(-1.0f, 1.0f);
                    }
                    break;

                default:
                    utils.LogError("King state invalid! Reverting to the intro state");
                    state = "intro";
                    break;
            }
        }

        public void animate()
        {
            switch (state)
            {
                case "intro":
                    intro.animate(0.075f);
                    sprite = intro.curFrame;
                    break;

                default:
                    utils.LogError("King state invalid! Reverting to the intro state");
                    state = "intro";
                    break;
            }
        }

        public void onKill()
        {

        }
    }
}
