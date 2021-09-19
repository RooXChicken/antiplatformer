using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entityLogic
{
    public class textEntity
    {
        public int id = 3;
        public string name = "Text";
        public string textString = "PLACEHOLDER";
        public Text text;
        public Sprite sprite = utils.loadSprite("res/missing.png");
        public Vector2f position;
        public bool destroy = false;

        public string[] input = { };

        public textEntity(string textInput)
        {
            input = textInput.Split('>');
            position = new Vector2f(float.Parse(input[0]), float.Parse(input[1]));
            textString = input[2];
            text = new Text(textString, Game.GAME_FONT);
            text.Position = position;
            text.Scale = new Vector2f(float.Parse(input[3]), float.Parse(input[3]));
        }

        public Text getSprite() { return text; }

        public void parseInput()
        {
            position = new Vector2f(float.Parse(input[0]), float.Parse(input[1]));
            textString = input[2];
            text = new Text(textString, Game.GAME_FONT);
            text.Position = position;
            text.Scale = new Vector2f(float.Parse(input[3]), float.Parse(input[3]));
        }

        public void onSpawn()
        {
            //nothing lol
        }

        public void update(float deltaTime)
        {
            text.Position = position;
            sprite.Position = position;
        }

        public void onKill()
        {
            text.DisplayedString = "ERROR!!! This should be removed!!! If it is not, send a screenshot to the developer :)";
        }
    }
}
