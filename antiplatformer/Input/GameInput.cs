using SFML.System;
using SFML.Window;

namespace antiplatformer
{
    public class GameInput
    {
        private static Vector2f JoyStickPosition = new Vector2f(0, 0);
        private static Vector2f KeyboardPosition = new Vector2f(0, 0);

        public static void Update()
        {
            KeyboardPosition.X = 0;
            KeyboardPosition.Y = 0;
            JoyStickPosition.X = 0;
            JoyStickPosition.Y = 0;

            if(Manager.GAME_HAS_FOCUS)
            {
                if(Joystick.IsConnected(0))
                {
                    JoyStickPosition.X = Joystick.GetAxisPosition(0, Joystick.Axis.X) / 100;
                    JoyStickPosition.Y = Joystick.GetAxisPosition(0, Joystick.Axis.Y) / 100;
                }
                else
                {
                    JoyStickPosition.X = 0;
                    JoyStickPosition.Y = 0;
                }

                if((GetKeyDown(Keyboard.Key.Left) || GetKeyDown(Keyboard.Key.A)) && !((GetKeyDown(Keyboard.Key.Right) || GetKeyDown(Keyboard.Key.D))))
                {
                    KeyboardPosition.X = -1;
                }
                else if(!(GetKeyDown(Keyboard.Key.Left) || GetKeyDown(Keyboard.Key.A)) && ((GetKeyDown(Keyboard.Key.Right) || GetKeyDown(Keyboard.Key.D))))
                {
                    KeyboardPosition.X = 1;
                }

                if((GetKeyDown(Keyboard.Key.Up) || GetKeyDown(Keyboard.Key.W)) && !((GetKeyDown(Keyboard.Key.Down) || GetKeyDown(Keyboard.Key.S))))
                {
                    KeyboardPosition.Y = -1;
                }
                else if(!(GetKeyDown(Keyboard.Key.Up) || GetKeyDown(Keyboard.Key.W)) && ((GetKeyDown(Keyboard.Key.Down) || GetKeyDown(Keyboard.Key.S))))
                {
                    KeyboardPosition.Y = 1;
                }
            }
        }

        public static bool GetKeyDown(Keyboard.Key key)
        {
            if(Manager.GAME_HAS_FOCUS)
            {
                if(Keyboard.IsKeyPressed(key))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static float GetHorizontal()
        {
            if(JoyStickPosition.X < 0 || JoyStickPosition.X > 0)
            {
                return JoyStickPosition.X;
            }
            else
            {
                return KeyboardPosition.X;
            }
        }

        public static float GetVertical()
        {
            if(JoyStickPosition.Y < 0 || JoyStickPosition.Y > 0)
            {
                return JoyStickPosition.Y;
            }
            else
            {
                return KeyboardPosition.Y;
            }
        }
    }
}