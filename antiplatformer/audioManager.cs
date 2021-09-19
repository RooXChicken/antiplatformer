using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace antiplatformer
{
    public class audioManager
    {
        private static IDictionary<String, Sound> sounds = new Dictionary<String, Sound>();
        private static IDictionary<String, Music> music = new Dictionary<String, Music>();

        private static float volume = 5.0f;

        public static void loadSound(string sound, string name)
        {
            try
            {
                sounds.Add(name, new Sound(new SoundBuffer(sound)));
            }
            catch (Exception e)
            {
                utils.LogError("Failed to load a sound with path: " + sound + ". Exception: " + e);
            }
        }

        public static void loadSong(string song, string name, bool looping)
        {
            try
            {
                music.Add(name, new Music(song));
                if(looping)
                {
                    foreach(KeyValuePair<string, Music> songe in music)
                    {
                        if(songe.Key == name)
                        {
                            songe.Value.Loop = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                utils.LogError("Failed to load a sound with path: " + song + ". Exception: " + e);
            }
        }

        public static void playSound(string name)
        {
            foreach(KeyValuePair<string, Sound> sound in sounds)
            {
                if(sound.Key == name)
                {
                    sound.Value.Play();
                }
            }
        }

        public static void playMusic(string name)
        {
            foreach(KeyValuePair<string, Music> song in music)
            {
                if(song.Key == name)
                {
                    song.Value.Play();
                }
            }
        }

        public static void stopMusic(string name)
        {
            foreach(KeyValuePair<string, Music> song in music)
            {
                if(song.Key == name)
                {
                    try
                    {
                        song.Value.Stop();
                    }
                    catch(Exception e)
                    {
                        utils.LogWarn("Failed to stop song, it isn't playing. This is safe to ignore! Exception: " + e);
                    }
                }
            }
        }

        public static void update()
        {
            volume = settings.volume;
            foreach(KeyValuePair<string, Sound> sound in sounds)
            {
                sound.Value.Volume = volume;
            }

            foreach(KeyValuePair<string, Music> song in music)
            {
                song.Value.Volume = volume;
            }
        }
    }
}