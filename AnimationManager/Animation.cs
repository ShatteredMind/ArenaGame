using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AnimationManager
{
    public sealed class Animation
    {
        #region Properties,fields

        private readonly Texture2D texture;
        private readonly int frameCount;
        private readonly int frameWidth;
        private readonly float frameTime;
        private readonly bool isLooping;
        public Texture2D Texture
        {
            get { return texture; }
        }
        public int frameHeight
        {
            get { return texture.Height; }
        }
        public float FrameTime
        {
            get { return frameTime; }
        }
        public bool IsLooping
        {
            get { return isLooping; }
        }
        public int FrameWidth
        {
            get { return frameWidth; }
        }
        public int FrameCount
        {
            get { return frameCount; }
        }

        #endregion

        public Animation(Texture2D newTexture, int FrameWidth, float newFrameTime, bool newisLooping)
        {
            texture = newTexture;
            frameWidth = FrameWidth;
            frameTime = newFrameTime;
            isLooping = newisLooping;
            frameCount = texture.Width / frameWidth;
        }
    }
}
