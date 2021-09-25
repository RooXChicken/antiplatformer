using System.Threading;

namespace antiplatformer
{
    public class CameraUtilities
    {
        public static void CameraShake(int degrees, bool right)
        {
            Thread shake = new Thread(() => {});

            shake = new Thread(() => {
            if (right)
            {
                for (int i = 0; i < degrees; i++)
                {
                    Program.antiPlatformer.camera.Rotation = i;
                    Thread.Sleep(5);
                }
                for (int i = degrees; i > 0; i--)
                {
                    Program.antiPlatformer.camera.Rotation = i;
                    Thread.Sleep(5);
                }
            }
            else
            {
                for (int i = 0; i < degrees; i++)
                {
                    Program.antiPlatformer.camera.Rotation = -i;
                    Thread.Sleep(5);
                }
                for (int i = degrees; i > 0; i--)
                {
                    Program.antiPlatformer.camera.Rotation = -i;
                    Thread.Sleep(5);
                }
            }
            Program.antiPlatformer.camera.Rotation = 0; 
            return; });

            shake.Start();
        }
    }
}