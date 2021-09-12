namespace antiplatformer
{
    class Program
    {
        public static void Main(string[] args)
        {
            //---LINUX ONLY---
            //it fixes random linux crashes, windows users comment out these lines please!
            //XInitThreads();
            Game game = new Game(args);

            game.init();

            while(game.isRunning())
            {
                game.update();
            }

            game.shutdown();
        }

        //---LINUX ONLY---
        //it fixes random linux crashes, windows users comment out these lines please!
        //[System.Runtime.InteropServices.DllImport("X11")]
        //extern public static int XInitThreads();
    }
}