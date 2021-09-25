using SFML.Graphics;
using SFML.System;
using System.Linq;

namespace antiplatformer.entity.entities
{
    public class Checkpoint : Entity
    {
        public override int Id {get; set;}
        public override string Name {get; set;}
        public override Vector2f Position {get; set;}
        public override string JsonPath {get; set;}
        public override int Health {get; set;}
        public override int MaxHealth {get; set;}
        public override bool Alive {get; set;}
        public override Sprite Sprite {get; set;}
        public Sprite checkSprite = Loader.LoadSprite("res/sprites/misc/empty.png");

        public override string[] Input {get; set;}

        public float pos = 0;

        public override void OnSpawn(int index)
        {
            Id = 3;
            Name = "Checkpoint" + index;
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
            pos = float.Parse(Input[0]);
            Sprite.Position = Position;
            checkSprite.Position = Position;
        }

        public override void Update()
        {
            Vector2f pPos = Program.antiPlatformer.entityManager.GetEntity("Player0").Position;
            if (pPos.X > pos)
            {
                Alive = false;
                foreach(Player e in Program.antiPlatformer.entityManager.GetEntities().OfType<Player>())
                {
                    if (e.currentCheckpoint.X < pPos.X)
                    {
                        e.currentCheckpoint = Position;
                    }
                }
            }
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