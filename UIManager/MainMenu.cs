using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UIManager
{
    public sealed class MainMenu
    {
        #region Fields and properties

        Texture2D backGround;
        Texture2D playButton;
        Texture2D exitButton;
        Texture2D optionsButton;
        Vector2 playButtonPosition;
        Vector2 exitButtonPosition;
        Vector2 optionsButtonPosition;
        Color colour;
        Rectangle playButtonRectangle;
        public Rectangle PlayButtonRectangle
        {
            get { return playButtonRectangle; }
        }
        Rectangle exitButtonRectangle;
        public Rectangle ExitButtonRectangle
        {
            get { return exitButtonRectangle; }
        }
        Rectangle optionsButtonRectangle;
        public Rectangle OptionsButtonRectangle
        {
            get { return optionsButtonRectangle; }
        }

        #endregion

        public MainMenu(Viewport graphics, Texture2D bg, Texture2D pb, Texture2D eb, Texture2D ob)
        {
            colour = new Color(255, 255, 255, 255);
            backGround = bg;
            playButton = pb;
            exitButton = eb;
            optionsButton = ob;
            playButtonPosition = new Vector2(graphics.Width / 2 - pb.Width/2, graphics.Height / 2 - 100);
            exitButtonPosition = new Vector2(graphics.Width / 2 - pb.Width/2, graphics.Height / 2 - 50);
            optionsButtonPosition = new Vector2(graphics.Width / 2 - pb.Width/2, graphics.Height / 2);
            playButtonRectangle = new Rectangle((int)playButtonPosition.X, (int)playButtonPosition.Y, (int)playButton.Width, (int)playButton.Height);
            exitButtonRectangle = new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, (int)exitButton.Width, (int)exitButton.Height);
            optionsButtonRectangle = new Rectangle((int)optionsButtonPosition.X, (int)optionsButtonPosition.Y, (int)optionsButton.Width, (int)optionsButton.Height);
        }

        public void Update(GameTime gameTime)
        {
           
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backGround, new Rectangle(0, 0, 1280, 800), Color.White);
            spriteBatch.Draw(playButton, playButtonPosition, Color.White);
            spriteBatch.Draw(exitButton, exitButtonPosition, Color.White);
            spriteBatch.Draw(optionsButton, optionsButtonPosition, Color.White);
        }

        public void ChangeColour()
        {
            
        }
    }
}
