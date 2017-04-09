using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CollisionHelper;
using PlayerManager;

namespace MapManager
{
    public sealed class Item : IConsumable
    {

        #region Fields, Properties and Constructor

        private readonly Texture2D texture;
        private readonly int health = 10;
        private Rectangle rect;
        private Vector2 position;
        private Vector2 velocity;
        private bool consumed;
        public int Health
        {
            get { return health; }
        }
        public bool Consumed
        {
            get { return consumed; }
            set { consumed = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
        }

        public Rectangle Rectangle
        {
            get { return rect; }
        }

        public Item(Texture2D texture, Rectangle rectangle, Vector2 position)
        {
            this.texture = texture;
            this.rect = rectangle;
            this.position = position;
        }

        #endregion

        public void Update(GameTime gameTime)
        {
            position += velocity;
            rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            if (velocity.Y < 10)
            {
                velocity.Y += 0.1f;
            }
        }

        public void ManageMapCollision(Rectangle tileRectangle, int xoffSet, int yoffSet)
        {

            if (rect.TouchTopOf(tileRectangle))
            {
                rect.Y = tileRectangle.Y - rect.Height;
                velocity.Y = 0f;
            }
            if (rect.TouchLeftOf(tileRectangle))
            {
                position.X = tileRectangle.X - rect.Width - 2;
            }
            if (rect.TouchRightOf(tileRectangle))
            {
                position.X = tileRectangle.X + rect.Width / 2 + 2;
            }
            if (position.Y < 0) velocity.Y = 1f;
            if (position.Y > yoffSet - rect.Height) position.Y = yoffSet - rect.Height;            
        }

        public bool TryToConsume(Rectangle playerRectangle)
        {
            return playerRectangle.Intersects(rect);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,rect,Color.White);
        }
    }
}
