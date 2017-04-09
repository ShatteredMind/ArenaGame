using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MapManager;
using AIManager;


namespace ArenaGame
{
    public class MapGenerator<T> where T : ILevel
    {
        public T Generate(params object[] args)
        {
            int[,] map = (int[,])args[0];
            int size = (int)args[1];
            for (int x = 0; x < map.GetLength(1); x++)
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    int tileDetector = map[y, x];
                    int width = (x + 1) * size;
                    int height = (y + 1) * size;

                    if (tileDetector > 0)
                        Level.collisionTiles.Add(new CollisionTiles(tileDetector, new Rectangle(x * size, y * size, size, size)));
                }
            try
            {
                return (T)Activator.CreateInstance(typeof(T), args);
            }
            catch (Exception) { throw new Exception("Check if everything is loaded"); }
        }
    }
}
