using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entity.entities
{
    public class DustSprite : Entity
    {
        public override int Id {get; set;}
        public override string Name {get; set;}
        public override Vector2f Position {get; set;}
        public Vector2f velocity = new Vector2f(0, 0);
        public float position2 = 0;
        public float position3 = 0;
        public float movementSpeed = 0;
        public override string JsonPath {get; set;}
        public override int Health {get; set;}
        public override int MaxHealth {get; set;}
        public override bool Alive {get; set;}
        public override Sprite Sprite {get; set;}

        bool direction = true;

        public override string[] Input {get; set;}

        public override void OnSpawn(int index)
        {
            Id = 5;
            Name = "DustSprite" + index;
            JsonPath = "";
            Health = 1;
            Alive = true;

            Sprite = Loader.LoadSprite("res/sprites/enemies/dustSprite.png");
        }

        public override void ParseInput(string Paramaters)
        {
            this.Input = Paramaters.Split('>');

            Position = new Vector2f(float.Parse(this.Input[0]) + 1, float.Parse(this.Input[1]));
            position2 = float.Parse(this.Input[2]);
            position3 = float.Parse(this.Input[3]);
            movementSpeed = float.Parse(this.Input[4]);
            Sprite.Position = Position;
        }

        public override void Update()
        {
            velocity.X = 0;
            foreach(Player p in Program.antiPlatformer.entityManager.GetEntities().OfType<Player>())
            {
                if(Sprite.GetGlobalBounds().Intersects(p.Sprite.GetGlobalBounds()))
                {
                    if(p.Position.Y - (Sprite.Position.Y - Sprite.GetGlobalBounds().Height) < 0 && p.velocity.Y > 0)
                    {
                        p.velocity = new Vector2f(0, -120);
                        p.isDiving = false;
                        p.isJumping = true;
                        Alive = false;
                    }
                    else
                    {
                        p.OnDamage(1);
                    }
                }
            }
            move();
        }

        private void move()
        {
            if(Position.X < position2 && direction)
            {
                velocity.X += movementSpeed;
            }
            else if(Position.X > position3)
            {
                direction = false;
                Sprite.Scale = new Vector2f(-1, 1);
                velocity.X += -movementSpeed;
                Sprite.Origin = new Vector2f(5, 0);

            }
            else
            {
                direction = true;
                Sprite.Scale = new Vector2f(1, 1);
                Sprite.Origin = new Vector2f(0, 0);
            }

            Position = new Vector2f(Position.X + velocity.X * Manager.GetDeltaTime(), Position.Y + velocity.Y * Manager.GetDeltaTime());

            Sprite.Position = Position;
            //Logger.Log(Position.X.ToString() + "," + Position.Y.ToString());
        }

        public override void Render(RenderWindow window)
        {
            window.Draw(Sprite);
        }

        public override void OnDamage(int damage)
        {
            
        }
        public override bool OnSceneDamage(int damage)
        {
            return true;
        }
        public override void OnKill()
        {
            
        }
    }
}