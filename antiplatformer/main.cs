namespace antiplatformer
{
    class Program
    {
        public static void Main(string[] args)
        {
#if !WINDOWS //Thank you to NedoProgrammer on GitHub for this fix!
            XInitThreads();
#endif

            Game game = new Game(args);

            game.init();

            while (game.isRunning())
            {
                game.update();
            }

            game.shutdown();
        }

#if !WINDOWS
        [System.Runtime.InteropServices.DllImport("X11")]
        extern public static int XInitThreads();
#endif
    }
}
