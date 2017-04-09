using System;
using System.Threading;
using Microsoft.Xna.Framework;

namespace ArenaGame
{

#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

