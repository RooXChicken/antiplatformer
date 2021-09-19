using System;
using System.Linq;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using TGUI;

namespace antiplatformer
{
    public class ui
    {
        public bool isPaused = false;

        public static Sprite fullHeart;
        public static Sprite halfHeart;
        public static Sprite emptyHeart;

        public static Gui gui;

        public ui()
        {

        }

        public static void load()
        {
            fullHeart = utils.loadSprite("res/sprites/ui/fullheart.png");
            halfHeart = utils.loadSprite("res/sprites/ui/halfheart.png");
            //emptyHeart = utils.loadSprite("res/sprites/ui/emptyheart.png");

            fullHeart.Scale = new Vector2f(2.0f, 2.0f);
            halfHeart.Scale = new Vector2f(2.0f, 2.0f);
        }

        public static void loadUI(string path)
        {
            gui.LoadWidgetsFromFile(path);
        }

        public static List<Sprite> sprites = new List<Sprite>();

        public static void health(int playerHealth, int antiHealth)
        {
            Vector2f pos = new Vector2f(0, 1);
            sprites.Clear();
            if (playerHealth % 2 == 0)
            {
                for(int b = 0; b < playerHealth / 2; b++)
                {
                    fullHeart.Position = pos;
                    sprites.Add(new Sprite(fullHeart));
                    pos.X += 16;
                }
            }
            else
            {
                for (int b = 0; b < (playerHealth + 1) / 2; b++)
                {
                    if(b != (playerHealth - 1) / 2)
                    {
                        fullHeart.Position = pos;
                        sprites.Add(new Sprite(fullHeart));
                        pos.X += 16;
                    }
                    else
                    {
                        halfHeart.Position = pos;
                        sprites.Add(new Sprite(halfHeart));
                        pos.X += 16;
                    }
                }
            }
        }

        public static void unload()
        {
            fullHeart.Dispose();
            halfHeart.Dispose();
            //emptyHeart.Dispose();
        }

        public void pause()
        {
            isPaused = !isPaused;
            if (isPaused)
            {

            }
        }
    }
}
