using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlayerManager
{
    public sealed class HealthBar
    {
        Texture2D hbTexture;
        Vector2 position;
        Rectangle rectangle;
        private int width;
        public int Width
        {
            get { return width; }
        }
        int height;
        private int damage;
        public Texture2D Texture
        {
            get { return hbTexture; }
        }

        public HealthBar(Texture2D hb, int lives)
        {
            hbTexture = hb;
            width = hbTexture.Width;
            height = hbTexture.Height;
            damage = width / lives;
        }

        public void Update(GameTime gameTime, Rectangle playerPosition)
        {
            position = new Vector2(playerPosition.X, playerPosition.Y - playerPosition.Height/2 );
            rectangle = new Rectangle(0, 0, width, height);
        }

        public void ReduceHpAmount(int damage)
        {
            width -= damage;
        }

        public void RestoreHP(int amount)
        {
            if (width + amount <= hbTexture.Width) width += amount;
            else width = hbTexture.Width;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
           spriteBatch.Draw(hbTexture, position, rectangle, Color.White);
        }
    }
}
