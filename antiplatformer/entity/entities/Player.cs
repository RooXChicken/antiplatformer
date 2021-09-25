using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using antiplatformer.audio;

namespace antiplatformer.entity.entities
{
    public class Player : Entity
    {
        public override int Id {get; set;}
        public override string Name {get; set;}
        public override Vector2f Position {get; set;}
        public override string JsonPath {get; set;}
        public override int Health {get; set;}
        public override int MaxHealth {get; set;}
        public override bool Alive {get; set;}
        public override Sprite Sprite {get; set;}

        public Vector2f velocity = new Vector2f(0, 0);
        public int gameState = 999;

        public Clock damageTimer = new Clock();
        public bool canBeDamaged = false;

        public string currentAnimation = "NULL";

        public float movementSpeed = 100;
        public float crouchSpeed = 60;
        public float crouchJumpHeight = 140;
        public float jumpHeight = 160;
        public float diveHeight = 120;
        public float diveSpeed = 0;
        public float gravitySpeed = 360;
        public Vector2f currentCheckpoint = new Vector2f(0, 0);
        public Clock coyoteJump = new Clock();

        private bool canJump;
        public int deathPlane = 250;

        private Animation walk = new Animation("res/sprites/roo/walk/", true);
        private Animation jump = new Animation("res/sprites/roo/jump/", false);
        private Animation fall = new Animation("res/sprites/roo/fall/", false);
        public Sprite idle = Loader.LoadSprite("res/sprites/roo/idle/idle.png");
        private Sprite dive = Loader.LoadSprite("res/sprites/roo/dive/dive.png");
        private Sprite crouch = Loader.LoadSprite("res/sprites/roo/crouch/crouch.png");

        public bool isWalking = false;
        public bool isJumping = false;
        public bool isFalling = false;
        public bool isFacingRight = true;
        public bool isCrouching = false;
        private bool canUncrouch = true;
        public bool isDiving = false;
        public bool diveDirection = true;

        private int tileIndex = 0;

        private RectangleShape hitbox = new RectangleShape();
        private FloatRect nextPos = new FloatRect();

        public override string[] Input {get; set;}

        public override void OnSpawn(int index)
        {
            Id = 2;
            Name = "Player" + index;
            JsonPath = "";
            Health = 6;
            MaxHealth = 6;
            Alive = true;
            
            Sprite = Loader.LoadSprite("res/missing.png");

            SoundEffectManager.LoadSound("res/sfx/jump1.ogg", "jump1");
            SoundEffectManager.LoadSound("res/sfx/jump2.ogg", "jump2");
            SoundEffectManager.LoadSound("res/sfx/jump3.ogg", "jump3");
            SoundEffectManager.LoadSound("res/sfx/jump4.ogg", "jump4");
            SoundEffectManager.LoadSound("res/sfx/jump5.ogg", "jump5");
            SoundEffectManager.LoadSound("res/sfx/hurt1.ogg", "hurt");

            hitbox.Size = new Vector2f(7.5f, 19.5f);
            Sprite.Scale = new Vector2f(1.5f, 1.5f);
            idle.Scale = new Vector2f(1.5f, 1.5f);
        }

        public override void ParseInput(string Paramaters)
        {
            this.Input = Paramaters.Split('>');

            Position = new Vector2f(float.Parse(this.Input[0]), float.Parse(this.Input[1]));
            currentCheckpoint = Position;
            deathPlane = Int32.Parse(this.Input[2]);
        }

        public override void Update()
        {
            velocity.X = 0;

            #region animation

            if (isDiving)
            {
                if (Sprite != dive)
                {
                    currentAnimation = "dive";
                    Sprite = dive;
                    Sprite.Color = Color.White;
                }
            }
            else if(velocity.Y > 0 && !isJumping && !isCrouching)
            {
                fall.animate(0.1f);
                if (Sprite != fall.CurrentFrame)
                {
                    currentAnimation = "fall";
                    Sprite = fall.CurrentFrame;
                    Sprite.Color = Color.White;
                }
            }
            else if (isWalking && !isJumping && !isCrouching) //walk anim
            {
                walk.animate(0.125f);
                if (Sprite != walk.CurrentFrame)
                {
                    currentAnimation = "walk";
                    Sprite = walk.CurrentFrame;
                    Sprite.Color = Color.White;
                }
            }
            else if (isJumping && !isCrouching) //jump anim
            {
                jump.animate(0.075f);
                if (Sprite != jump.CurrentFrame)
                {
                    currentAnimation = "jump";
                    Sprite = jump.CurrentFrame;
                    Sprite.Color = Color.White;
                }
            }
            else if (!isCrouching) //idle anim
            {
                if(Sprite != idle)
                {
                    currentAnimation = "idle";
                    Sprite = idle;
                    Sprite.Color = Color.White;
                }
            }
            else if(isCrouching)//crouch anim
            {
                if(Sprite != crouch)
                {
                    currentAnimation = "crouch";
                    Sprite = crouch;
                    Sprite.Color = Color.White;
                }
            }

            //floatrect
            if (isFacingRight && !isJumping && !isCrouching)
            {
                Sprite.Origin = new Vector2f(0, 0);
                Sprite.Scale = new Vector2f(1.5f, 1.5f);
                idle.Origin = new Vector2f(0, 0);
                idle.Scale = new Vector2f(1.5f, 1.5f);
            }
            else if(!isJumping && !isCrouching)
            {
                Sprite.Origin = new Vector2f(5, 0);
                Sprite.Scale = new Vector2f(-1.5f, 1.5f);
                idle.Origin = new Vector2f(5, 0);
                idle.Scale = new Vector2f(-1.5f, 1.5f);
            }
            else if(isCrouching)
            {
                if (isFacingRight)
                {
                    Sprite.Origin = new Vector2f(1, 0);
                    Sprite.Scale = new Vector2f(1.5f, 1.5f);
                }
                else
                {
                    Sprite.Origin = new Vector2f(6, 0);
                    Sprite.Scale = new Vector2f(-1.5f, 1.5f);
                }
            }
            else
            {
                if (isFacingRight)
                {
                    Sprite.Origin = new Vector2f(2.5f, 0);
                    Sprite.Scale = new Vector2f(1.5f, 1.5f);
                }
                else
                {
                    Sprite.Origin = new Vector2f(7.5f, 0);
                    Sprite.Scale = new Vector2f(-1.5f, 1.5f);
                }
            }

            if(damageTimer.ElapsedTime.AsMilliseconds() < 1000)
            {
                Sprite.Color = new Color(255, 155, 155);
            }
            else
            {
                if(!canBeDamaged)
                {
                    canBeDamaged = true;
                    Sprite.Color = Color.White;
                }
            }

            #endregion

            #region input

            if (GameInput.GetHorizontal() != 0)
            {
                isWalking = true;
                if(GameInput.GetHorizontal() < 0)
                {
                    isFacingRight = false;
                }
                else
                {
                    isFacingRight = true;
                }
                velocity.X = movementSpeed * GameInput.GetHorizontal();
            }
            else
            {
                isWalking = false;
            }

            if(!(gameState >= 1))
            {
                velocity.X = 0;
            }

            if (velocity.Y == 0)
            {
                fall.restart();
                isJumping = false;
                coyoteJump.Restart();
            }
            if (velocity.Y != 0 && coyoteJump.ElapsedTime.AsMilliseconds() > 150)
            {
                canJump = false;
            }

            if (!(gameState >= 2))
            {
                canJump = false;
            }
            if (((GameInput.GetKeyDown(Keyboard.Key.Space) && canJump || GameInput.GetKeyDown(Keyboard.Key.Space) && canJump) || (Joystick.IsButtonPressed(0, 0) && canJump)) && canUncrouch)
            {
                jump.restart();
                velocity.Y = -jumpHeight;
                isJumping = true;
                SoundEffectManager.PlaySound("jump" + Manager.random.Next(1, 5));
                canJump = false;
            }

            //crouch and dive
            if ((((GameInput.GetKeyDown(Keyboard.Key.Down) || GameInput.GetKeyDown(Keyboard.Key.S)) || Joystick.IsButtonPressed(0, 2)) && gameState >= 4) && !canJump)
            {
                //dive
                if (!isDiving)
                {
                    isDiving = true;
                    isJumping = false;
                    diveSpeed = 0;
                    velocity.Y = -diveHeight;
                    if (isFacingRight)
                    {
                        diveDirection = true;
                    }
                    else
                    {
                        diveDirection = false;
                    }
                }
            }
            else
            {
                //crouch
                if (((GameInput.GetKeyDown(Keyboard.Key.Down) || GameInput.GetKeyDown(Keyboard.Key.S)) || GameInput.GetVertical() > 0.2) && canJump && gameState >= 3)
                {
                    if (!isCrouching)
                    {
                        isCrouching = true;
                        movementSpeed = crouchSpeed;
                        jumpHeight = crouchJumpHeight;
                        hitbox.Size = new Vector2f(7.5f, 15);
                        hitbox.Position = new Vector2f(hitbox.Position.X, hitbox.Position.Y + 4.5f);
                    }
                }
                else if (canUncrouch)
                {
                    if (isCrouching)
                    {
                        isCrouching = false;
                        movementSpeed = 100;
                        jumpHeight = 160;
                        hitbox.Size = new Vector2f(7.5f, 19.5f);
                        hitbox.Position = new Vector2f(hitbox.Position.X, hitbox.Position.Y - 4.5f);
                    }
                }
                else if (!canUncrouch)
                {
                    if (!isCrouching)
                    {
                        isCrouching = true;
                        hitbox.Size = new Vector2f(7.5f, 15);
                        hitbox.Position = new Vector2f(hitbox.Position.X, hitbox.Position.Y + 4.5f);
                    }
                }
            }
            canUncrouch = true;

            if (isDiving)
            {
                velocity.X = 0;

                if (diveDirection)
                {
                    if (isFacingRight)
                    {
                        diveSpeed += 4f;
                        if (diveSpeed > 150)
                        {
                            diveSpeed = 150;
                        }
                        velocity.X += diveSpeed;
                    }
                    else
                    {
                        if (diveSpeed > 40)
                            diveSpeed = 40;
                        diveSpeed += 0.5f;
                        velocity.X -= diveSpeed;
                    }
                    isFacingRight = true;

                }
                else
                {
                    if (isFacingRight)
                    {
                        if (diveSpeed > 40)
                            diveSpeed = 40;
                        diveSpeed += 0.5f;
                        velocity.X += diveSpeed;
                    }
                    else
                    {
                        diveSpeed += 4;
                        if (diveSpeed > 150)
                        {
                            diveSpeed = 150;
                        }
                        velocity.X -= diveSpeed;
                    }
                    isFacingRight = false;
                }
            }

            #endregion

            if (velocity.Y <= 360)
            {
                velocity.Y += gravitySpeed * Manager.GetDeltaTime();
            }

            #region collision

            FloatRect playerBounds = hitbox.GetGlobalBounds();
            nextPos = playerBounds;
            nextPos.Left += velocity.X * Manager.GetDeltaTime();
            nextPos.Top += velocity.Y * Manager.GetDeltaTime();

            for (uint i = 0; i < Program.antiPlatformer.loadedLevel.GetVertexArray().VertexCount; i++)
            {
                if (tileIndex == 0)
                {
                    tileIndex = 3;
                    FloatRect wallBounds;
                    Vertex tilemapVertex = Program.antiPlatformer.loadedLevel.GetVertexArray()[i];
                    wallBounds.Left = tilemapVertex.Position.X;
                    wallBounds.Top = tilemapVertex.Position.Y;
                    wallBounds.Width = 8;
                    wallBounds.Height = 8;

                    Vector2f wallpos = new Vector2f(wallBounds.Left, wallBounds.Top);

                    if (nextPos.Left - wallBounds.Left < nextPos.Width && nextPos.Top - wallBounds.Top < nextPos.Top && nextPos.Left + nextPos.Width - wallBounds.Left > 0 && nextPos.Top + nextPos.Height - wallBounds.Top > 0)
                    {
                        if (isCrouching)
                        {
                            FloatRect crouchPos = playerBounds;
                            crouchPos.Top -= 4.5f;
                            crouchPos.Height = 19.5f;

                            if (crouchPos.Intersects(wallBounds))
                            {
                                if (crouchPos.Top > wallBounds.Top && crouchPos.Top + crouchPos.Height > wallBounds.Top + wallBounds.Height && crouchPos.Left < wallBounds.Left + wallBounds.Width && crouchPos.Left + crouchPos.Width > wallBounds.Left)
                                {
                                    canUncrouch = false;
                                    canJump = false;
                                }
                            }
                        }

                        if (nextPos.Intersects(wallBounds) && wallpos != new Vector2f(0, 0)) //player collision
                        {
                            //Logger.Log("eee");
                            if (playerBounds.Left < wallBounds.Left && playerBounds.Left + playerBounds.Width < wallBounds.Left + wallBounds.Width && playerBounds.Top < wallBounds.Top + wallBounds.Height && playerBounds.Top + playerBounds.Height > wallBounds.Top) //right collision
                            {
                                //right
                                velocity.X = 0.0f;
                                Position = new Vector2f(wallBounds.Left - playerBounds.Width, Position.Y);
                            }
                            else if (playerBounds.Left > wallBounds.Left && playerBounds.Left + playerBounds.Width > wallBounds.Left + wallBounds.Width && playerBounds.Top < wallBounds.Top + wallBounds.Height && playerBounds.Top + playerBounds.Height > wallBounds.Top) //Left collision
                            {
                                //left
                                velocity.X = 0.0f;
                                Position = new Vector2f(wallBounds.Left + wallBounds.Width, Position.Y);
                            }
                            else if (playerBounds.Top < wallBounds.Top && playerBounds.Top + playerBounds.Height < wallBounds.Top + wallBounds.Height && playerBounds.Left < wallBounds.Left + wallBounds.Width && playerBounds.Left + playerBounds.Width > wallBounds.Left) //bottom collision
                            {
                                //ground
                                canJump = true;
                                isDiving = false;
                                velocity.Y = 0.0f;
                                Position = new Vector2f(Position.X, wallBounds.Top - playerBounds.Height);
                            }
                            else if (playerBounds.Top > wallBounds.Top && playerBounds.Top + playerBounds.Height > wallBounds.Top + wallBounds.Height && playerBounds.Left < wallBounds.Left + wallBounds.Width && playerBounds.Left + playerBounds.Width > wallBounds.Left) //top collision
                            {
                                //ceiling
                                velocity.Y = 0.0f;
                                Position = new Vector2f(Position.X, wallBounds.Top + wallBounds.Height);
                            }
                        }
                    }
                }
                else if(tileIndex < 0)
                {
                    tileIndex = 3;
                }
                else
                {
                    tileIndex--;
                }
            }

            #endregion

            #region final moving of player based off velocity
            Position = new Vector2f(Position.X + velocity.X * Manager.GetDeltaTime(), Position.Y + velocity.Y * Manager.GetDeltaTime());

            hitbox.Position = Position;
            Sprite.Position = Position;

            #endregion

            //deathplane check
            if (Position.Y > deathPlane)
            {
                OnDamage(1);
                Position = currentCheckpoint;
            }
        }

        public override void OnDamage(int damage)
        {
            if(canBeDamaged)
            {
                SoundEffectManager.PlaySound("hurt");
                canBeDamaged = false;
                Health -= damage;
                //ui.health(health, 0);
                isDiving = false;

                damageTimer.Restart();

                if (Manager.random.Next(0, 10) >= 5)
                {
                    CameraUtilities.CameraShake(15, true);
                }
                else
                {
                    CameraUtilities.CameraShake(15, false);
                }

                if (Health <= 0)
                {
                    Logger.Log("you just died XD");
                    Health = MaxHealth;
                    //Position = new Vector2f(float.Parse(Input[0]), float.Parse(Input[1]));
                    //currentCheckpoint = new Vector2f(float.Parse(Input[0]), float.Parse(Input[1]));
                    //ui.health(health, 0);
                }

                //Game.GAME_MAIN_CHARACTER_HEALTH = health;
            }
        }

        public override void Render(RenderWindow window)
        {
            window.Draw(Sprite);
        }

        public override bool OnSceneDamage(int damage)
        {
            SoundEffectManager.PlaySound("hurt");
            Health -= damage;

            // damageTimer.Restart();

            // if (Manager.random.Next(0, 10) >= 5)
            // {
            //     CameraUtilities.CameraShake(15, true);
            // }
            // else
            // {
            //     CameraUtilities.CameraShake(15, false);
            // }

            if (Health <= 0)
            {
                Logger.Log("you monster.");
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void OnKill()
        {
            
        }
    }
}