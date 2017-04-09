using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CollisionHelper;
using ObjectPool;
using AnimationManager;
using Microsoft.Xna.Framework.Input;
using CameraManager;

namespace PlayerManager
{
    /// <summary>Some info</summary>
    /// It would be better to combine all public methods
    /// into one Update method and remove all this publicity, 
    /// but whatever. Ability is updating and drawing based on bool
    /// parameter isUsed. If ability is used we gotta calculate the 
    /// direction of ability and maintain its speed. If ability is not
    /// used, we have to calculate ability's start point(our player).
    /// And the last, but not least disposing ability, if it collided with
    /// some of the object or simple went off the map.
    public class Fireball : IEntity, IDisposable
    {
        #region Properties and fields

        protected readonly Texture2D abilityTexture;
        private readonly float speed = 500f;
        protected Vector2 velocity;
        protected Vector2 direction;
        protected Vector2 movement;
        protected bool disposed = false;
        protected AnimationPlayer abilityPlayer;
        protected Animation abilityAnimation;
        protected bool isDisposed;
        public bool IsDisposed
        {
            get { return isDisposed;}
        }
        protected bool isUsed;
        public bool IsUsed
        {
            get { return isUsed; }
            set { isUsed = value; }
        }
        protected Rectangle abilityRectangle;
        public Rectangle EntityRectangle
        {
            get { return abilityRectangle; }
            private set { abilityRectangle = value; }
        }
        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            private set { position = value; }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
        }


        #endregion

        public Fireball(Texture2D newTexture, Rectangle playerPosition)
        {
            abilityPlayer = new AnimationPlayer();
            abilityTexture = newTexture;
            abilityRectangle = playerPosition;
            abilityAnimation = new Animation(newTexture, 11, 70f, true);
        }

        ~Fireball()
        {
            Dispose(false);
        }

        public void UpdateLocation(GameTime gameTime, Rectangle playerPosition)
        {
            position = new Vector2(playerPosition.X,playerPosition.Y);
        }

        public void ManageDirection(GameTime gameTime, MouseState mouse, Rectangle playerPosition, Camera camera)
        {
            if (movement != Vector2.Zero)
            {
                Move(movement, gameTime); return; 
            }
            movement = new Vector2(mouse.X - playerPosition.X + camera.Border, mouse.Y - playerPosition.Y);
            if (movement != Vector2.Zero)
                movement.Normalize();
            Move(movement, gameTime);
        }

        private void Move(Vector2 movement, GameTime gameTime)
        {
            position += movement * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            abilityRectangle = new Rectangle((int)position.X, (int)position.Y, abilityTexture.Width / abilityAnimation.FrameCount, abilityTexture.Height);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {          
           abilityPlayer.Playanimation(abilityAnimation);
           abilityPlayer.Draw(gameTime, spriteBatch, position, SpriteEffects.None);
        }

        public void ManageMapCollision(Rectangle tileRectangle, int xOffset, int yOffset)
        {
            if (isUsed)
            {
                if (abilityRectangle.TouchRightOf(tileRectangle))
                {
                    isDisposed = true;
                }
                if (abilityRectangle.TouchLeftOf(tileRectangle))
                {
                    isDisposed = true;
                }
                if (abilityRectangle.X < 0) isDisposed = true;
                if (abilityRectangle.X > xOffset) isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                if (disposing)
                {
                    isDisposed = true;
                }
                disposed = true;
            }
        }
    }
}
