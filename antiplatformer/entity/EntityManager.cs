using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace antiplatformer.entity
{
    public class EntityManager
    {
        public Dictionary<string, Entity> EntityList;
        public AntiPlatformer antiPlatformer;

        public EntityManager(AntiPlatformer antiPlatformer)
        {
            EntityList = new Dictionary<string, Entity>();
            this.antiPlatformer = antiPlatformer;
        }

        public void Update(RenderWindow renderWindow)
        {
            for(int i = 0; i < GetEntities().Count; i++)
            {

            }
        }

        public void AddEntity(string name, string paramaters)
        {
            int index = 0;
            foreach(Entity ee in GetEntities())
            {
                if(ee.Name.ToLower().Contains(name.ToLower()))
                {
                    index++;
                }
            }
            try
            {
                Entity e = new entities.Error();
                switch(name.ToLower())
                {
                    case "error":
                        e = new entities.Error();
                        break;
                    case "splashscreen":
                        e = new entities.SplashScreen();
                        break;
                    case "player":
                        e = new entities.Player();
                        break;
                    case "text":
                        e = new entities.Text();
                        break;
                    case "endlevelportal":
                        e = new entities.EndLevelPortal();
                        break;
                    case "checkpoint":
                        e = new entities.Checkpoint();
                        break;
                    case "dustsprite":
                        e = new entities.DustSprite();
                        break;
                    case "decoration":
                        e = new entities.Decoration();
                        break;

                    default:
                        e = new entities.Error();
                        break;

                }

                e.OnSpawn(index);
                e.ParseInput(paramaters);

                try
                {
                    EntityList.Add(name.ToLower() + index, e);
                }
                catch(Exception ee)
                {
                    Logger.LogError("Failed to load entity of type: " + name + ". Exception: " + ee);
                    //EntityList.Add(name.ToLower(), e);
                }
            }
            catch(Exception e)
            {
                Logger.LogError("Failed to load entity. Exception: " + e);
            }
        }

        public void Clear()
        {
            EntityList.Clear();
        }

        public void RemoveEntity(string name)
        {
            EntityList[name.ToLower()].OnKill();
            EntityList.Remove(name.ToLower());
        }

        public Entity GetEntity(string name)
        {
            try
            {
                return EntityList[name.ToLower()];
            }
            catch(Exception e)
            {
                Logger.LogError("Failed to get the entity with name: " + name + ". Exception: " + e);
            }
            return new entities.Error();
        }

        public List<Entity> GetEntities()
        {
            List<Entity> entities = new List<Entity>();

            foreach(string name in EntityList.Keys)
            {
                entities.Add(EntityList[name]);
            }

            return entities;
        }
    }
}