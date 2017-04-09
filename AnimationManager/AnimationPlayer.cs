using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimationManager
{
    public struct AnimationPlayer
    {
        #region Properties and fields

        Animation animation;
        public Animation Animation
        {
            get { return animation; }
            set { animation = value; }
        }

        int frameIndex;
        public int FrameIndex
        {
            get { return frameIndex; }
            set { frameIndex = value; }
        }

        private float timer;

        public Vector2 Origin
        {
            get { return new Vector2(0, 0); }
        }

        #endregion

        public void Playanimation(Animation newAnimation)
        {
            if (animation == newAnimation)
            {
                return;
            }

            animation = newAnimation;
            frameIndex = 0;
            timer = 0;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (animation == null)
                throw new NotSupportedException("You done fucked up");
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1.5f;
            while (timer >= animation.FrameTime)
            {
                timer -= animation.FrameTime;
                if (animation.IsLooping)
                {
                    frameIndex = (frameIndex + 1) % animation.FrameCount; // used to repeat the animation
                }
                else
                {
                    frameIndex = Math.Min(frameIndex + 1, animation.FrameCount - 1); // used to play animation only once
                }
            }
            Rectangle rectangle = new Rectangle(frameIndex * Animation.FrameWidth, 0, Animation.FrameWidth, Animation.frameHeight);
            spriteBatch.Draw(Animation.Texture, position, rectangle, Color.White, 0, Origin, 1f, spriteEffects, 0);
        }
    }
}
