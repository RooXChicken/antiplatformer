using System;
using System.Collections.Generic;
using SFML.Audio;

namespace antiplatformer.audio
{
    public class MusicManager
    {
        private static IDictionary<String, Music> Music = new Dictionary<String, Music>();

        public static void LoadSong(string song, string name, bool looping)
        {
            try
            {
                Music.Add(name, new Music(song));
                if(looping)
                {
                    foreach(KeyValuePair<string, Music> songe in Music)
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
                Logger.LogError("Failed to load a song with path: " + song + ". Exception: " + e);
            }
        }

        public static void UnloadSong(string name)
        {
            try
            {
                Music.Remove(name);
                
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to unload a song with path: " + name + ". how tf does that happen lmao. Exception: " + e);
            }
        }

        public static void PlayMusic(string name)
        {
            foreach(KeyValuePair<string, Music> song in Music)
            {
                if(song.Key == name)
                {
                    song.Value.Play();
                }
            }
        }

        public static void StopMusic(string name)
        {
            foreach(KeyValuePair<string, Music> song in Music)
            {
                if(song.Key == name)
                {
                    try
                    {
                        song.Value.Stop();
                    }
                    catch(Exception e)
                    {
                        Logger.LogWarn("Failed to stop song, it isn't playing. This is safe to ignore! Exception: " + e);
                    }
                }
            }
        }

        public static void Update()
        {
            foreach(KeyValuePair<string, Music> song in Music)
            {
                song.Value.Volume = Settings.Volume;
            }
        }
    }
}