using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

namespace antiplatformer.entityLogic
{
    public class playerEntity
    {
        public string name = "Roo";
        public int id = 1;
        public int gameState = 999;
        public TileMap collisionMap;
        public JObject entityData = JObject.Parse(File.ReadAllText("res/entities/player.json"));
        public bool destroy = false;
        private Random rand = new Random();

        public string[] input = { };

        public Vector2f position = new Vector2f(0, 0);
        public Vector2f velocity = new Vector2f(0, 0);
        public Vector2f joystickPos = new Vector2f(0, 0);
        public Sprite sprite;
        public int maxHealth = 6;
        public int health = 6;
        public Clock damageTimer = new Clock();
        public bool canBeDamaged = false;

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

        private SoundBuffer jumpSoundBuf = new SoundBuffer("res/sfx/jump1.ogg");
        private Sound jumpSound;
        private SoundBuffer hurtSoundBuf = new SoundBuffer("res/sfx/hurt1.ogg");
        private Sound hurtSound;

        private animation walk = new animation("res/sprites/" + Game.GAME_MAIN_CHARACTER_NAME + "/walk/", true);
        private animation jump = new animation("res/sprites/" + Game.GAME_MAIN_CHARACTER_NAME + "/jump/", false);
        public Sprite idle = utils.loadSprite("res/sprites/" + Game.GAME_MAIN_CHARACTER_NAME + "/idle/idle.png");
        private Sprite dive = utils.loadSprite("res/sprites/" + Game.GAME_MAIN_CHARACTER_NAME + "/dive/dive.png");
        private Sprite crouch = utils.loadSprite("res/sprites/" + Game.GAME_MAIN_CHARACTER_NAME + "/crouch/crouch.png");

        public bool isWalking = false;
        public bool isJumping = false;
        public bool isFacingRight = true;
        public bool isCrouching = false;
        private bool canUncrouch = true;
        public bool isDiving = false;
        public bool diveDirection = true;

        private int tileIndex = 0;

        private RectangleShape hitbox = new RectangleShape();
        private FloatRect nextPos = new FloatRect();

        public playerEntity(string paramaters)
        {
            input = paramaters.Split('>');

            position = new Vector2f(float.Parse(entityData["x"].ToString()), float.Parse(entityData["y"].ToString()));
            maxHealth = Int32.Parse(entityData["maxHealth"].ToString());
            health = Int32.Parse(entityData["currentHealth"].ToString());
            movementSpeed = float.Parse(entityData["movementSpeed"].ToString());
            crouchSpeed = float.Parse(entityData["crouchSpeed"].ToString());
            gravitySpeed = float.Parse(entityData["gravitySpeed"].ToString());

            try
            {
                position = new Vector2f(float.Parse(input[0]), float.Parse(input[1]));
                currentCheckpoint = position;
                deathPlane = Int32.Parse(input[2]);
            }
            catch(Exception e)
            {
                utils.LogError("Player did not have position or death plane variables as paramaters! Here is the exception if it helps: " + e);
            }
        }

        public Sprite getSprite() { return sprite; }
        public void getTilemap(TileMap tilemap) { collisionMap = tilemap; }

        public void parseInput()
        {
            position = new Vector2f(float.Parse(entityData["x"].ToString()), float.Parse(entityData["y"].ToString()));
            maxHealth = Int32.Parse(entityData["maxHealth"].ToString());
            health = Int32.Parse(entityData["currentHealth"].ToString());
            movementSpeed = float.Parse(entityData["movementSpeed"].ToString());
            crouchSpeed = float.Parse(entityData["crouchSpeed"].ToString());
            gravitySpeed = float.Parse(entityData["gravitySpeed"].ToString());

            try
            {
                position = new Vector2f(float.Parse(input[0]), float.Parse(input[1]));
                currentCheckpoint = position;
                deathPlane = Int32.Parse(input[2]);
            }
            catch (Exception e)
            {
                utils.LogError("Player did not have position or death plane variables as paramaters! Here is the exception if it helps: " + e);
            }
        }

        public void onSpawn()
        {
            jumpSound = new Sound(jumpSoundBuf);
            jumpSound.Volume = Game.GAME_MAIN_AUDIO_VOLUME;
            hurtSound = new Sound(hurtSoundBuf);
            hurtSound.Volume = Game.GAME_MAIN_AUDIO_VOLUME;

            sprite = utils.loadSprite("res/missing.png");

            hitbox.Size = new Vector2f(7.5f, 19.5f);

            sprite.Scale = new Vector2f(1.5f, 1.5f);
            idle.Scale = new Vector2f(1.5f, 1.5f);

            if(Game.GAME_MAIN_CHARACTER_HEALTH != 0)
            {
                health = Game.GAME_MAIN_CHARACTER_HEALTH;
            }

            ui.health(health, 0);
        }

        public void update(float deltaTime)
        {
            velocity.X = 0;

            if (Joystick.IsConnected(0))
            {
                joystickPos.X = Joystick.GetAxisPosition(0, Joystick.Axis.X);
                joystickPos.Y = Joystick.GetAxisPosition(0, Joystick.Axis.Y);
            }

            #region animation

            if (isDiving)
            {
                if (sprite != dive)
                {
                    sprite = dive;
                    sprite.Color = Color.White;
                }
            }
            else if (isWalking && !isJumping && !isCrouching) //walk anim
            {
                walk.animate(0.125f);
                if (sprite != walk.curFrame)
                {
                    sprite = walk.curFrame;
                    sprite.Color = Color.White;
                }
            }
            else if (isJumping && !isCrouching) //jump anim
            {
                jump.animate(0.075f);
                if (sprite != jump.curFrame)
                {
                    sprite = jump.curFrame;
                    sprite.Color = Color.White;
                }
            }
            else if (!isCrouching) //idle anim
            {
                if(sprite != idle)
                {
                    sprite = idle;
                    sprite.Color = Color.White;
                }
            }
            else if(isCrouching)//crouch anim
            {
                if(sprite != crouch)
                {
                    sprite = crouch;
                    sprite.Color = Color.White;
                }
            }

            //floatrect
            if (isFacingRight && !isJumping && !isCrouching)
            {
                sprite.Origin = new Vector2f(0, 0);
                sprite.Scale = new Vector2f(1.5f, 1.5f);
                idle.Origin = new Vector2f(0, 0);
                idle.Scale = new Vector2f(1.5f, 1.5f);
            }
            else if(!isJumping && !isCrouching)
            {
                sprite.Origin = new Vector2f(5, 0);
                sprite.Scale = new Vector2f(-1.5f, 1.5f);
                idle.Origin = new Vector2f(5, 0);
                idle.Scale = new Vector2f(-1.5f, 1.5f);
            }
            else if(isCrouching)
            {
                if (isFacingRight)
                {
                    sprite.Origin = new Vector2f(1, 0);
                    sprite.Scale = new Vector2f(1.5f, 1.5f);
                }
                else
                {
                    sprite.Origin = new Vector2f(6, 0);
                    sprite.Scale = new Vector2f(-1.5f, 1.5f);
                }
            }
            else
            {
                if (isFacingRight)
                {
                    sprite.Origin = new Vector2f(2.5f, 0);
                    sprite.Scale = new Vector2f(1.5f, 1.5f);
                }
                else
                {
                    sprite.Origin = new Vector2f(7.5f, 0);
                    sprite.Scale = new Vector2f(-1.5f, 1.5f);
                }
            }

            if(damageTimer.ElapsedTime.AsMilliseconds() < 1000)
            {
                sprite.Color = new Color(255, 155, 155);
            }
            else
            {
                if(!canBeDamaged)
                {
                    canBeDamaged = true;
                    sprite.Color = Color.White;
                }
            }

            #endregion

            #region input

            if (Keyboard.IsKeyPressed(Keyboard.Key.Left) || Keyboard.IsKeyPressed(Keyboard.Key.A) || joystickPos.X < -0.1)
            {
                isWalking = true;
                isFacingRight = false;
                velocity.X = -movementSpeed;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Right) || Keyboard.IsKeyPressed(Keyboard.Key.D) || joystickPos.X > 0.1)
            {
                isWalking = true;
                isFacingRight = true;
                velocity.X = movementSpeed;
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
            if (((Keyboard.IsKeyPressed(Keyboard.Key.Space) && canJump || Keyboard.IsKeyPressed(Keyboard.Key.Up) && canJump) || (Joystick.IsButtonPressed(0, 0) && canJump)) && canUncrouch)
            {
                jump.restart();
                velocity.Y = -jumpHeight;
                isJumping = true;
                jumpSound.Play();
                canJump = false;
            }

            //crouch and dive
            if ((((Keyboard.IsKeyPressed(Keyboard.Key.Down) || Keyboard.IsKeyPressed(Keyboard.Key.S)) || Joystick.IsButtonPressed(0, 2)) && gameState >= 4) && !canJump)
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
                if (((Keyboard.IsKeyPressed(Keyboard.Key.Down) || Keyboard.IsKeyPressed(Keyboard.Key.S)) || joystickPos.Y > 0.2) && canJump && gameState >= 3)
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
                        movementSpeed = float.Parse(entityData["movementSpeed"].ToString());
                        jumpHeight = float.Parse(entityData["jumpHeight"].ToString());
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
                velocity.Y += gravitySpeed * deltaTime;
            }

            #region collision

            FloatRect playerBounds = hitbox.GetGlobalBounds();
            nextPos = playerBounds;
            nextPos.Left += velocity.X * deltaTime;
            nextPos.Top += velocity.Y * deltaTime;

            for (uint i = 0; i < collisionMap.getVertexArray().VertexCount; i++)
            {
                if (tileIndex == 0)
                {
                    tileIndex = 3;
                    FloatRect wallBounds;
                    Vertex tilemapVertex = collisionMap.getVertexArray()[i];
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
                            if (playerBounds.Left < wallBounds.Left && playerBounds.Left + playerBounds.Width < wallBounds.Left + wallBounds.Width && playerBounds.Top < wallBounds.Top + wallBounds.Height && playerBounds.Top + playerBounds.Height > wallBounds.Top) //right collision
                            {
                                //right
                                velocity.X = 0.0f;
                                position.X = wallBounds.Left - playerBounds.Width;
                            }
                            else if (playerBounds.Left > wallBounds.Left && playerBounds.Left + playerBounds.Width > wallBounds.Left + wallBounds.Width && playerBounds.Top < wallBounds.Top + wallBounds.Height && playerBounds.Top + playerBounds.Height > wallBounds.Top) //Left collision
                            {
                                //left
                                velocity.X = 0.0f;
                                position.X = wallBounds.Left + wallBounds.Width;
                            }
                            else if (playerBounds.Top < wallBounds.Top && playerBounds.Top + playerBounds.Height < wallBounds.Top + wallBounds.Height && playerBounds.Left < wallBounds.Left + wallBounds.Width && playerBounds.Left + playerBounds.Width > wallBounds.Left) //bottom collision
                            {
                                //ground
                                canJump = true;
                                isDiving = false;
                                velocity.Y = 0.0f;
                                position.Y = wallBounds.Top - playerBounds.Height;
                            }
                            else if (playerBounds.Top > wallBounds.Top && playerBounds.Top + playerBounds.Height > wallBounds.Top + wallBounds.Height && playerBounds.Left < wallBounds.Left + wallBounds.Width && playerBounds.Left + playerBounds.Width > wallBounds.Left) //top collision
                            {
                                //ceiling
                                velocity.Y = 0.0f;
                                position.Y = wallBounds.Top + wallBounds.Height;
                            }
                        }
                    }
                }
                else
                {
                    tileIndex--;
                }
            }

            #endregion

            #region final moving of player based off velocity
            position.X += velocity.X * deltaTime;
            position.Y += velocity.Y * deltaTime;

            hitbox.Position = position;
            sprite.Position = position;

            #endregion

            //deathplane check
            if (position.Y > deathPlane)
            {
                position = currentCheckpoint;
                onDamage(1);
            }
        }

        public void onDamage(int damage)
        {
            if(canBeDamaged)
            {
                canBeDamaged = false;
                health -= damage;
                ui.health(health, 0);
                isDiving = false;

                hurtSound.Play();

                damageTimer.Restart();

                if (rand.Next(0, 10) >= 5)
                {
                    Game.cameraShake(15, true);
                }
                else
                {
                    Game.cameraShake(15, false);
                }

                if (health <= 0)
                {
                    utils.Log("you just died XD");
                    health = maxHealth;
                    position = new Vector2f(float.Parse(input[0]), float.Parse(input[1]));
                    currentCheckpoint = new Vector2f(float.Parse(input[0]), float.Parse(input[1]));
                    ui.health(health, 0);
                }

                Game.GAME_MAIN_CHARACTER_HEALTH = health;
            }
        }

        public void onKill()
        {

        }
    }
}
