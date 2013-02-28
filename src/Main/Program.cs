using System;

namespace ChairWars
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //Console.Read();

            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

