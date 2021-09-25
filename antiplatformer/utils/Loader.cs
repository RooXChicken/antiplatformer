using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace antiplatformer
{
    public class Loader
    {
        public static Sprite LoadSprite(string path)
        {
            Texture tex;
            try
            {
                tex = new Texture(path);
                return new Sprite(tex);
            }
            catch
            {
                Logger.LogError("Texture with path: " + path + " does not exist! Check the directory! Loading the missing texture sprite as a backup");
                try
                {
                    tex = new Texture("res/missing.png");
                    return new Sprite(tex);
                }
                catch
                {
                    Logger.LogFatal("You are missing the missing texture sprite, what on earth did you do!?!? like come on dude at the least leave that! im going to return the window icon, so say hi to roos head :)");
                    return new Sprite(new Texture("res/icon.png"));
                }
            }
        }
    }
}