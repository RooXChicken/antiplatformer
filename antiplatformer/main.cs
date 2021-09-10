namespace antiplatformer
{
    class Program
    {
        public static void Main(string[] args)
        {
            XInitThreads();
            Game game = new Game(args);

            game.init();

            while(game.isRunning())
            {
                game.update();
            }

            game.shutdown();
        }

        //to fix random crashes !!LINUX ONLY!!
        [System.Runtime.InteropServices.DllImport("X11")]
        extern public static int XInitThreads();
    }
}