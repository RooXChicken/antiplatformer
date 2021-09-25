using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace antiplatformer.entity.entities
{
    public class Text : Entity
    {
        public override int Id {get; set;}
        public override string Name {get; set;}
        public override Vector2f Position {get; set;}
        public override string JsonPath {get; set;}
        public override int Health {get; set;}
        public override int MaxHealth {get; set;}
        public override bool Alive {get; set;}
        public override Sprite Sprite {get; set;}

        public string textString = "PLACEHOLDER";
        public SFML.Graphics.Text text;

        public override string[] Input {get; set;}

        public override void OnSpawn(int index)
        {
            Id = 7;
            Name = "Text" + index;
            JsonPath = "";
            Health = 1;
            Alive = true;

            Sprite = Loader.LoadSprite("res/missing.png");
        }

        public override void ParseInput(string Paramaters)
        {
            this.Input = Paramaters.Split('>');

            Position = new Vector2f(float.Parse(this.Input[0]), float.Parse(this.Input[1]));
            Sprite.Position = Position;
            textString = this.Input[2];
            text = new SFML.Graphics.Text(textString, Manager.font);
            text.Position = Position;
            text.Scale = new Vector2f(float.Parse(this.Input[3]), float.Parse(this.Input[3]));
        }

        public override void Update()
        {
            
        }

        public override void Render(RenderWindow window)
        {
            window.Draw(text);
        }

        public override void OnDamage(int damage)
        {
            
        }
        public override void OnKill()
        {
            text.DisplayedString = "ERROR!!! This should be removed!!! If it is not, send a screenshot to the developer :) i wanna see too!";
        }
    }
}