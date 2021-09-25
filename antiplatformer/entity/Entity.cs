using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace antiplatformer.entity
{
    public abstract class Entity
    {
        public abstract int Id {get; set;}
        public abstract string Name {get; set;}
        public abstract Vector2f Position {get; set;}
        public abstract string JsonPath {get; set;}
        public abstract int Health {get; set;}
        public abstract int MaxHealth {get; set;}
        public abstract Sprite Sprite {get; set;}
        public abstract bool Alive {get; set;}

        public abstract string[] Input {get; set;}

        public abstract void OnSpawn(int index);
        public abstract void ParseInput(string Paramaters);

        public abstract void Update();
        public abstract void Render(RenderWindow window);
        
        public abstract void OnDamage(int damage);
        public abstract void OnKill();
    }
}