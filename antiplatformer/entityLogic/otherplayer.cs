using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

namespace antiplatformer.entityLogic
{
    public class otherplayer
    {
        public string name = "otherplayer";
        public int id = 10;
        public int gameState = 999;

        public string[] input = { };

        public Vector2f position = new Vector2f(0, 0);
        public Sprite sprite;
    
        public string currentAnimation = "NULL";

        private animation walk = new animation("res/sprites/" + Game.GAME_MAIN_CHARACTER_NAME + "/walk/", true);
        private animation jump = new animation("res/sprites/" + Game.GAME_MAIN_CHARACTER_NAME + "/jump/", false);
        public Sprite idle = utils.loadSprite("res/sprites/" + Game.GAME_MAIN_CHARACTER_NAME + "/idle/idle.png");
        private Sprite dive = utils.loadSprite("res/sprites/" + Game.GAME_MAIN_CHARACTER_NAME + "/dive/dive.png");
        private Sprite crouch = utils.loadSprite("res/sprites/" + Game.GAME_MAIN_CHARACTER_NAME + "/crouch/crouch.png");

        public otherplayer(string paramaters)
        {
            
        }

        public Sprite getSprite() { return sprite; }

        public void parseInput()
        {
            
        }

        public void onSpawn()
        {
            
        }

        public void update(string paramaters)
        {
            string[] input = paramaters.Split('<');
            foreach(string item in input)
            {
                if(item.Contains("pos="))
                {
                    string[] temppos = item.Substring(4).Split(',');
                    position = new Vector2f(float.Parse(temppos[0]), float.Parse(temppos[1]));
                }
                else if(item.Contains("anim="))
                {
                    currentAnimation = item.Substring(5);
                }
            }

            #region animation

            switch(currentAnimation)
            {
                case "idle":
                    if(sprite != idle)
                    {
                        sprite = idle;
                        sprite.Color = Color.White;
                    }
                    break;
                case "walk":
                    walk.animate(0.125f);
                    if (sprite != walk.curFrame)
                    {
                        sprite = walk.curFrame;
                        sprite.Color = Color.White;
                    }
                    break;
                case "dive":
                    if (sprite != dive)
                    {
                        sprite = dive;
                        sprite.Color = Color.White;
                    }
                    break;
                case "crouch":
                    if(sprite != crouch)
                    {
                        sprite = crouch;
                        sprite.Color = Color.White;
                    }
                    break;

                default:
                    if(sprite != idle)
                    {
                        sprite = idle;
                        sprite.Color = Color.White;
                    }
                    break;
            }

            //floatrect
            // if (isFacingRight && !isJumping && !isCrouching)
            // {
            //     sprite.Origin = new Vector2f(0, 0);
            //     sprite.Scale = new Vector2f(1.5f, 1.5f);
            //     idle.Origin = new Vector2f(0, 0);
            //     idle.Scale = new Vector2f(1.5f, 1.5f);
            // }
            // else if(!isJumping && !isCrouching)
            // {
            //     sprite.Origin = new Vector2f(5, 0);
            //     sprite.Scale = new Vector2f(-1.5f, 1.5f);
            //     idle.Origin = new Vector2f(5, 0);
            //     idle.Scale = new Vector2f(-1.5f, 1.5f);
            // }
            // else if(isCrouching)
            // {
            //     if (isFacingRight)
            //     {
            //         sprite.Origin = new Vector2f(1, 0);
            //         sprite.Scale = new Vector2f(1.5f, 1.5f);
            //     }
            //     else
            //     {
            //         sprite.Origin = new Vector2f(6, 0);
            //         sprite.Scale = new Vector2f(-1.5f, 1.5f);
            //     }
            // }
            // else
            // {
            //     if (isFacingRight)
            //     {
            //         sprite.Origin = new Vector2f(2.5f, 0);
            //         sprite.Scale = new Vector2f(1.5f, 1.5f);
            //     }
            //     else
            //     {
            //         sprite.Origin = new Vector2f(7.5f, 0);
            //         sprite.Scale = new Vector2f(-1.5f, 1.5f);
            //     }
            // }

            #endregion
        }

        public void onKill()
        {

        }
    }
}
