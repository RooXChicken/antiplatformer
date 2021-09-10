using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer
{
    public class cutscenes
    {
        public cutscenes()
        {
            
        }

        Clock animClock = new Clock();
        bool cutscenePlayed = false;

        public void cutsceneOne(List<entity> entityList, float deltaTime)
        {
            bool cutsceneOver = false;
            bool hasJumped = false;

            while (!cutsceneOver)
            {
                if (entityList[Game.GAME_PLAYER_INDEX].myClass.position.X >= 625.0f)
                {
                    if (!cutscenePlayed)
                    {
                        cutscenePlayed = true;
                        entityList[Game.GAME_PLAYER_INDEX].myClass.gameState = 0;
                        animClock.Restart();
                    }

                    int milli = animClock.ElapsedTime.AsMilliseconds();

                    if (milli < 1500)
                    {
                        entityList[Game.GAME_PLAYER_INDEX].myClass.isWalking = true;
                        entityList[Game.GAME_PLAYER_INDEX].myClass.isFacingRight = true;
                        entityList[Game.GAME_PLAYER_INDEX].myClass.position = new Vector2f(entityList[Game.GAME_PLAYER_INDEX].myClass.position.X + 60 * deltaTime, entityList[Game.GAME_PLAYER_INDEX].myClass.position.Y);
                    }
                    else if(milli < 3500)
                    {
                        entityList[Game.GAME_PLAYER_INDEX].myClass.isWalking = false;
                        entityList[Game.GAME_PLAYER_INDEX].myClass.isFacingRight = true;
                    }
                    else if(milli < 5000)
                    {
                        entityList[Game.GAME_PLAYER_INDEX - 2].myClass.isFacingRight = false;
                    }
                    else if(milli < 5500)
                    {
                        entityList[Game.GAME_PLAYER_INDEX - 2].myClass.animate();
                    }
                }
                cutsceneOver = true;
            }
        }
    }
}
