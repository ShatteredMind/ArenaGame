using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CollisionHelper;
using ObjectPool;
using AnimationManager;
using Microsoft.Xna.Framework.Input;

namespace AIManager
{
    public sealed class BossAbility : IEntity, IDisposable
    {
        #region Properties and fields and constructor

        private readonly Texture2D abilityTexture;
        private readonly Animation abilityAnimation;
        private readonly float speed = 500f;
        private Vector2 movement;
        private AnimationPlayer abilityPlayer;
        private bool isDisposed;
        public bool IsDisposed
        {
            get { return isDisposed; }
        }
        private bool isUsed;
        public bool IsUsed
        {
            get { return isUsed; }
            set { isUsed = value; }
        }
        private Rectangle abilityRectangle;
        public Rectangle EntityRectangle
        {
            get { return abilityRectangle; }
            private set { abilityRectangle = value; }
        }
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            private set { position = value; }
        }
        public Vector2 Velocity
        {
            get { return movement; }
        }


        public BossAbility(Texture2D newTexture, Rectangle bossPosition)
        {
            abilityPlayer = new AnimationPlayer();
            abilityTexture = newTexture;
            abilityRectangle = bossPosition;
            abilityAnimation = new Animation(newTexture, 59, 70f, true);
        }

        ~BossAbility()
        {
            Dispose();
        }

        #endregion


        #region Public methods

        public void UpdateAbilityLocation(GameTime gameTime, Vector2 bossPosition)
        {
            position = new Vector2(bossPosition.X, bossPosition.Y);
        }

        public void ManageDirection(GameTime gameTime, Vector2 playerPosition, Rectangle bossPosition)
        {
            if (movement != Vector2.Zero)
            {
                Move(movement, gameTime); return;
            }
            movement = new Vector2(playerPosition.X - bossPosition.X,playerPosition.Y - bossPosition.Y);
            if (movement != Vector2.Zero)
                movement.Normalize();
            Move(movement, gameTime);
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
            GC.SuppressFinalize(this);
        }

        #endregion


        #region Private methods

        private void Move(Vector2 movement, GameTime gameTime)
        {
            position += movement * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            abilityRectangle = new Rectangle((int)position.X, (int)position.Y, abilityTexture.Width / abilityAnimation.FrameCount, abilityTexture.Height);
        }

        #endregion

    }
}
