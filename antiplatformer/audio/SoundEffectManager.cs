using System;
using System.Collections.Generic;
using SFML.Audio;

namespace antiplatformer.audio
{
    public class SoundEffectManager
    {
        private static IDictionary<String, Sound> Sounds = new Dictionary<String, Sound>();

        public static void LoadSound(string sound, string name)
        {
            try
            {
                Sounds.Add(name, new Sound(new SoundBuffer(sound)));
            }
            catch (Exception e)
            {
                //to see if i need to log an error message or it's simply already there
                bool log = true;
                foreach(string s in Sounds.Keys)
                {
                    if(s == name)
                    {
                        log = true;
                    }
                }

                if(log)
                {
                    Logger.LogError("Failed to load a sound with path: " + sound + ". Exception: " + e);
                }
            }
        }

        public static void UnloadSound(string name)
        {
            try
            {
                Sounds.Remove(name);
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to unload a sound with path: " + name + ". how tf does that happen lmao. Exception: " + e);
            }
        }

        public static void PlaySound(string name)
        {
            foreach(KeyValuePair<string, Sound> sound in Sounds)
            {
                if(sound.Key == name)
                {
                    sound.Value.Play();
                }
            }
        }

        public static void Update()
        {
            foreach(KeyValuePair<string, Sound> sound in Sounds)
            {
                sound.Value.Volume = Settings.Volume;
            }
        }
    }
}