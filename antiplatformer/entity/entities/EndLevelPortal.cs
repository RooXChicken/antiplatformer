using SFML.Graphics;
using SFML.System;
using System;

namespace antiplatformer.entity.entities
{
    public class EndLevelPortal : Entity
    {
        public override int Id {get; set;}
        public override string Name {get; set;}
        public override Vector2f Position {get; set;}
        public override string JsonPath {get; set;}
        public override int Health {get; set;}
        public override int MaxHealth {get; set;}
        public override bool Alive {get; set;}
        public override Sprite Sprite {get; set;}

        public string newScenePath = "NULL";
        public bool facingRight = false;

        public override string[] Input {get; set;}

        public override void OnSpawn(int index)
        {
            Id = 6;
            Name = "EndLevelPortal" + index;
            JsonPath = "";
            Health = 1;
            Alive = true;
            Sprite = Loader.LoadSprite("res/sprites/levels/default/decorations/portal.png");
        }

        public override void ParseInput(string Paramaters)
        {
            this.Input = Paramaters.Split('>');

            Position = new Vector2f(float.Parse(this.Input[0]), float.Parse(this.Input[1]));
            Sprite.Position = Position;
            if(!Boolean.Parse(this.Input[2]))
            {
                facingRight = false;
                Sprite.Scale = new Vector2f(-1, 1);
            }
            else
            {
                facingRight = true;
            }
            newScenePath = this.Input[3];
        }

        public override void Update()
        {
            if(Sprite.GetGlobalBounds().Intersects(Program.antiPlatformer.entityManager.GetEntity("Player0").Sprite.GetGlobalBounds()))
            {
                Program.antiPlatformer.sceneLoader.LoadScene(newScenePath);
                Alive = false;
            }
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