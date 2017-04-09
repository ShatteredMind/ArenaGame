using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using CameraManager;

namespace UIManager
{
    public sealed class Introduction
    {
        #region Fields, Properties, Constructor

        private readonly Texture2D texture;
        private readonly Texture2D buttonTexture;
        private readonly Rectangle buttonRectangle;
        private readonly Rectangle textureRectangle;
        private Rectangle mouseRectangle;
        private MouseState newState;
        private MouseState oldState;
        bool finished;
        public bool Finished
        {
            get { return finished; }
        }

        public Introduction(Texture2D texture, Texture2D buttonTexture)
        {
            this.texture = texture;
            this.buttonTexture = buttonTexture;
            buttonRectangle = new Rectangle(655,430, buttonTexture.Width, buttonTexture.Height);
            textureRectangle = new Rectangle(530,250, texture.Width, texture.Height);
        }

        #endregion

        public void Update(GameTime gameTime)
        {
            newState = Mouse.GetState();
            CheckMouseClick(newState);
            oldState = newState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,textureRectangle,Color.White);
            spriteBatch.Draw(buttonTexture, buttonRectangle, Color.White);
        }

        private void CheckMouseClick(MouseState mouseState)
        {
            mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            if (mouseRectangle.Intersects(buttonRectangle) && oldState.LeftButton == ButtonState.Pressed && newState.LeftButton == ButtonState.Released)
            {
                finished = true;
            }
            else finished = false;
        }
    }
}
