using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIManager;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PlayerManager;

namespace MapManager
{
    /// <summary>
    /// Since we're trying to create a 2D-tile(square rectangle) game,
    /// map can be easily represented as a 2D array. 
    /// Also, map handles all the collisions between objects
    /// (using the CollisionHelper)
    /// </summary>
    public interface ILevel
    {    
        /// <summary> NPCS
        /// List contains all npcs that alive at the moment
        /// and list itself helps a lot to remove and add them
        /// when needed
        /// </summary>
        List<NPC> NPCS { get; }

        /// <summary> Widht and height
        /// Map has to pass it width and height to the camera and
        /// main app, in order to provide proper work.
        /// </summary>
        int Width { get; }
        int Height { get; }

        /// <summary> ManageEntitiesCollision
        /// The name of this method says for itself.
        /// We just pass the player, that contains
        /// his required parameters(position, rectangle)
        /// and manipulate with them and the objects on
        /// the map
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="player"></param>
        void ManageEntitiesCollision(Player player);
        
    }
}
