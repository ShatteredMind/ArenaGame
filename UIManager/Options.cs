using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UIManager
{
    public sealed class Options
    {
        Texture2D backGround;
        Texture2D backButton;
        Vector2 backButtonPosition;
        Color colour;
        Rectangle backButtonRectangle;
        public Rectangle BackButtonRectangle
        {
            get { return backButtonRectangle; }
        }

        public Options(Viewport graphics, Texture2D bg, Texture2D bb)
        {
            colour = new Color(255, 255, 255, 255);
            backGround = bg;
            backButton = bb;
            backButtonPosition = new Vector2(graphics.Width - 200, graphics.Height - 100);
            backButtonRectangle = new Rectangle(graphics.Width - 200, graphics.Height - 100, (int)backButton.Width, (int)backButton.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backGround, new Rectangle(0, 0, 1280, 800), Color.White);
            spriteBatch.Draw(backButton, backButtonPosition, Color.White);
        }
    }
}
