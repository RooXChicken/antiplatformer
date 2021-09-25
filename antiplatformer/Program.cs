namespace antiplatformer
{
    class Program
    {
        public static AntiPlatformer antiPlatformer;
        static void Main(string[] args)
        {
            XInitThreads();

            antiPlatformer = new AntiPlatformer(args);

            antiPlatformer.init();
            
            while(antiPlatformer.isRunning())
            {
                antiPlatformer.update();
                antiPlatformer.render();
            }
        }

        [System.Runtime.InteropServices.DllImport("X11")]
        extern public static int XInitThreads();
    }
}
