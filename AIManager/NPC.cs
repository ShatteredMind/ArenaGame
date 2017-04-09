using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnimationManager;
using CollisionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerManager;

namespace AIManager
{
    /// <summary>Every single type of NPC has its own behaviour</summary>
    /// Behaviour State depends on player's position;
    /// For instance, Ghost doesn't collide with any tile on the map
    /// and has only three behavior states. One of them is used to reach
    /// the player, the second one is to attack. Update and Draw methods
    /// depend on the current state. Mostly, it was done in order
    /// to draw animantion properly.
    /// Not to mention, that every single type has its own health and
    /// damage parameters. 
    /// But all npcs have some basic similar methods,
    /// such as remove, attack etc. So, in ordet to prevent code
    /// from repeation abstact class was created
    /// 

    public abstract class NPC
    {

        #region Fields and properties

        protected AnimationPlayer animationPlayer;
        protected Animation movement;
        protected Animation attack;
        protected Animation death;
        protected Texture2D moveSheet;
        protected Texture2D attackSheet;
        protected Texture2D deathSheet;
        protected Vector2 direction; // direction contains value that represents distance between player and npc(as vector)
        protected int damage;
        protected int lives;
        protected float speed;
        /// <summary> Cooldown thing
        /// Cooldowns are used in order to provide proper animation.
        /// They behave as timers, to check that animation is finsihed
        /// </summary>
        protected float attackCooldown;
        protected float deathAnimationCooldown;
        protected bool deathAnimationFinished; // we needed to check if deathanimation is finished and then dispose the npc
        protected SpriteEffects flip;
        protected Vector2 velocity;
        protected Vector2 position;
        /// <summary> Public properties
        /// The main idea was to grab already existing npc with this sheets
        /// and just copy it
        /// </summary>
        public Texture2D MoveSheet
        { 
            get { return moveSheet; } 
        }
        public Texture2D AttackSheet
        {
            get { return attackSheet; }
        }
        public Texture2D DeathSheet
        {
            get { return deathSheet; }
        }
        protected Rectangle npcRectangle;
        public Rectangle EntityRectangle
        {
            get { return npcRectangle; }
        }
        protected bool alive;
        public bool Alive
        {
            get { return alive; }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
        }
        public Vector2 Position
        {
            get { return position; }
        }
        

        #endregion


        #region Public virtual methods

        public virtual void ManageMapCollision(Rectangle tileRectangle, int xoffSet, int yoffSet)
        {
            if (npcRectangle.TouchTopOf(tileRectangle))
            {
                npcRectangle.Y = tileRectangle.Y - npcRectangle.Height;
                position.Y = npcRectangle.Y;
                velocity.Y = 0f;
            }
            if (npcRectangle.TouchLeftOf(tileRectangle))
            {
                position.X = tileRectangle.X - npcRectangle.Width - 2;
            }
            if (npcRectangle.TouchRightOf(tileRectangle))
            {
                position.X = tileRectangle.X + npcRectangle.Width / 2 + 2;
            }
            if (npcRectangle.TouchBootomOf(tileRectangle))
            {
                velocity.Y = 1f;
            }

            if (position.X < 0) position.X = 0;
            if (position.X > xoffSet - npcRectangle.Width) position.X = xoffSet - npcRectangle.Width;
            if (position.Y < 0) velocity.Y = 1f;
        }

        public virtual void RemoveLife(Player player, Fireball usedFireball)
        {
            if (usedFireball.EntityRectangle.Intersects(npcRectangle))
            {
                lives--;
                player.Fireballs.RemoveItem(usedFireball);
                usedFireball.Dispose();
            }
        }

        public virtual void Slow(Player player, Frostbolt usedFrostbolt)
        {
            if (usedFrostbolt.EntityRectangle.Intersects(npcRectangle))
            {
                speed = speed / 2;
                player.FrostBolts.RemoveItem(usedFrostbolt);
                usedFrostbolt.Dispose();
            }
        }

        public virtual void Update(GameTime gameTime, Player player)
        {
        }
     
        public virtual void RemoveNPC(GameTime gameTime)
        {
            deathAnimationCooldown += gameTime.ElapsedGameTime.Milliseconds;
            if (deathAnimationCooldown >= 300)
            {
                deathAnimationFinished = true;
                alive = false;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            animationPlayer.Draw(gameTime, spriteBatch, position, flip);
        }

        #endregion


        #region Protected virual methods

        protected virtual void PursuePlayer(Rectangle playerPosition, GameTime gameTime)
        {
            direction = new Vector2(playerPosition.X - position.X, playerPosition.Y - position.Y);
            if (direction != Vector2.Zero)
                direction.Normalize();
            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            npcRectangle = new Rectangle((int)position.X, (int)position.Y, moveSheet.Width / movement.FrameCount, moveSheet.Height);
        }

        protected virtual void AttackPlayer(GameTime gameTime, Player player)
        {
            npcRectangle = new Rectangle((int)position.X, (int)position.Y, attackSheet.Width / attack.FrameCount, attackSheet.Height);
            attackCooldown += gameTime.ElapsedGameTime.Milliseconds;
            if (attackCooldown > 400)
            {
                player.ManageEntitiesCollision(damage, position);
                attackCooldown = 0;
            }
        }

        #endregion

    }
}
