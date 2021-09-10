using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer
{
    public class animation
    {
        List<Sprite> sprites = new List<Sprite>();

        public int frame;
        int maxFrames;
        public Sprite curFrame;

        bool repeats = true;

        Clock animClock = new Clock();

        public animation(string path, bool doesRepeat)
        {
            repeats = doesRepeat;
            curFrame = new Sprite();
            maxFrames = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length - 1;
            int i = 0;
            foreach (string tex in Directory.GetFiles(path).OrderBy(f => f))
            {
                sprites.Add(utils.loadSprite(tex));
                i++;
            }
        }

        public void restart()
        {
            frame = 0;
            curFrame = sprites[0];
        }

        public void animate(float animTime)
        {
            if (animClock.ElapsedTime.AsSeconds() > animTime)
            {
                if (frame > maxFrames)
                {
                    if (repeats)
                        frame = 0;
                    else
                        frame = maxFrames;

                }

                curFrame = sprites[frame];
                frame++;
                animClock.Restart();
            }
        }
    }
}
