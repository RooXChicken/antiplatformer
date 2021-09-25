using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entity.entities
{
    public class Error : Entity
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
            Id = 0;
            Name = "Error" + index;
            JsonPath = "";
            Health = 0;
            Alive = true;

            Sprite = Loader.LoadSprite("res/missing.png");

            Logger.LogError("Hello there player! This is the error entity (hi :D) This message should not appear unless something broke, which in this case, it did. Let me know, will ya!");
        }

        public override void ParseInput(string Paramaters)
        {
            this.Input = Paramaters.Split('>');
            Sprite.Position = Position;
        }

        public override void Update()
        {
            
        }

        public override void Render(RenderWindow window)
        {
            
        }

        public override void OnDamage(int damage)
        {
            
        }
        public override void OnKill()
        {
            
        }
    }
}