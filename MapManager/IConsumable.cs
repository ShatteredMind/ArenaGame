using System;
using PlayerManager;
using Microsoft.Xna.Framework;

namespace MapManager
{
    interface IConsumable
    {
        /// <summary>
        /// All the pickable items in game are 
        /// supposed to be potions(or a healing salve)
        /// </summary>
        int Health { get; }

        /// <summary>
        /// Not really much to explain here. Every potion
        /// can be consumed by a player and that's a special
        /// flag to detect it. Potions are consumed instantly
        /// when picked
        /// </summary>
        bool Consumed { get; set; }

        /// <summary>
        /// Returns true if player's rectangle intersects
        /// with potion's rectangle
        /// </summary>
        /// <param name="player"> player is passed in order to
        /// compare player's position and potion's position</param>
        /// <returns></returns>
        bool TryToConsume(Rectangle playerRectangle);
    }
}