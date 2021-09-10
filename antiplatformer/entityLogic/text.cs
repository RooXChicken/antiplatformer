using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class textEntity
    {
        public int id = 3;
        public string name = "text";
        public string textString = "PLACEHOLDER";
        public Text text;
        public Vector2f position;
        public bool destroy = false;

        public textEntity(string textInput)
        {
            string[] input = textInput.Split('>');
            position = new Vector2f(float.Parse(input[1]), float.Parse(input[2]));
            textString = input[0];
            text = new Text(textString, Game.GAME_MAIN_FONT);
            text.Position = position;
            text.Scale = new Vector2f(float.Parse(input[3]), float.Parse(input[3]));
        }

        public Text getSprite() { return text; }

        public void onSpawn()
        {
            //nothing lol
        }

        public void update(float deltaTime)
        {
            //nothing else lol
        }

        public void onKill()
        {
            text.DisplayedString = "ERROR!!! This should be removed!!! If it is not, send a screenshot to the developer :)";
        }
    }
}
