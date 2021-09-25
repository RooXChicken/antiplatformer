using SFML.Graphics;
using SFML.Graphics;
using SFML.System;

namespace antiplatformer.entity.entities
{
    public class SplashScreen : Entity
    {
        public override int Id {get; set;}
        public override string Name {get; set;}
        public override Vector2f Position {get; set;}
        public override string JsonPath {get; set;}
        public override int Health {get; set;}
        public override int MaxHealth {get; set;}
        public override Sprite Sprite {get; set;}
        public override bool Alive {get; set;}

        public override string[] Input {get; set;}

        public Clock splashClock = new Clock();

        public override void OnSpawn(int index)
        {
            Id = 1;
            Name = "SplashScreen" + index;
            JsonPath = "";
            Health = 1;
            Alive = true;

            Sprite = Loader.LoadSprite("res/sprites/splashText.png");
            Sprite.Position = new Vector2f(256 / 3, 144 / 2.25f);
        }

        public override void ParseInput(string Paramaters)
        {
            Input = Paramaters.Split('>');
        }

        public override void Update()
        {
            int alphaValue = (splashClock.ElapsedTime.AsMilliseconds() - 700) / 4;

            alphaValue = Math.ClampInt(alphaValue, 0, 255);

            if(splashClock.ElapsedTime.AsMilliseconds() / 7 > 400)
            {
                Logger.Log("Splash screen over!");
                Alive = false;
            }

            this.Sprite.Color = new Color(255, 255, 255, (byte)alphaValue);
        }

        public override void Render(RenderWindow window)
        {
            window.Draw(this.Sprite);
        }

        public override void OnDamage(int damage)
        {
            
        }
        public override void OnKill()
        {
            
        }
    }
}