using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entity.entities
{
    public class Decoration : Entity
    {
        public override int Id {get; set;}
        public override string Name {get; set;}
        public override Vector2f Position {get; set;}
        public override string JsonPath {get; set;}
        public override int Health {get; set;}
        public override int MaxHealth {get; set;}
        public override bool Alive {get; set;}
        public override Sprite Sprite {get; set;}

        public override string[] Input {get; set;}

        public override void OnSpawn(int index)
        {
            Id = 4;
            Name = "Decoration" + index;
            JsonPath = "";
            Health = 1;
            Alive = true;

            Sprite = Loader.LoadSprite("res/missing.png");
        }

        public override void ParseInput(string Paramaters)
        {
            this.Input = Paramaters.Split('>');
            Sprite = Loader.LoadSprite(this.Input[2]);

            Sprite.Position = new Vector2f(float.Parse(this.Input[0]), float.Parse(this.Input[1]));
        }

        public override void Update()
        {
            
        }

        public override void Render(RenderWindow window)
        {
            window.Draw(Sprite);
        }

        public override void OnDamage(int damage)
        {
            
        }
        public override void OnKill()
        {
            
        }
    }
}