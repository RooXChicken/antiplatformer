using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFML.Graphics;

namespace antiplatformer.entityLogic
{
    public class errorEntity
    {
        public int id = 0;
        Sprite sprite;   
        public errorEntity()
        {

        }

        public SFML.Graphics.Sprite getSprite() { return sprite; }

        public void onSpawn()
        {
            sprite = utils.loadSprite("res/missing.png");

            utils.LogError("Hello there player! This is the error entity (hi :D) This message should not appear unless something broke, which in this case, it did. Let me know, will ya!");
        }

        public void update(float deltaTime)
        {

        }

        public void onKill()
        {

        }
    }
}
