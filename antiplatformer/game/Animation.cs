using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer
{
    public class Animation
    {
        List<Sprite> Sprites = new List<Sprite>();

        public int Frame;
        int MaxFrames;
        public Sprite CurrentFrame;

        bool repeats = true;

        Clock AnimClock = new Clock();

        public Animation(string path, bool doesRepeat)
        {
            repeats = doesRepeat;
            CurrentFrame = new Sprite();
            MaxFrames = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length - 1;
            int i = 0;
            foreach (string tex in Directory.GetFiles(path).OrderBy(f => f))
            {
                Sprites.Add(Loader.LoadSprite(tex));
                i++;
            }
        }

        public void restart()
        {
            Frame = 0;
            CurrentFrame = Sprites[0];
        }

        public void animate(float animTime)
        {
            if (AnimClock.ElapsedTime.AsSeconds() * Manager.GAME_TIME_SCALE > animTime)
            {
                if (Frame > MaxFrames)
                {
                    if (repeats)
                        Frame = 0;
                    else
                        Frame = MaxFrames;

                }

                CurrentFrame = Sprites[Frame];
                Frame++;
                AnimClock.Restart();
            }
        }
    }
}
