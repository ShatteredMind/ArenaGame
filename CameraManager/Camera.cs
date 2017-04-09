using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CameraManager
{
    public sealed class Camera
    {
        #region Properties and fields(Unfortunately, some of them haven't been used)

        private float border;
        private Matrix transform;
        private Vector2 center;
        private Viewport viewport;
        private float zoom = 1;
        public float Border
        {
            get { return border; }
        }
        public Matrix Transform
        {
            get { return transform; }
        }
        public Vector2 Center
        {
            get { return center; }
        }
        public float X
        {
            get { return center.X; }
            set { center.X = value; }
        }
        public float Y
        {
            get { return center.Y;}
            set { center.Y = value; }
        }
        public float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
                if (zoom < 0.1f)
                    zoom = 0.1f;
            }
        }
        public int ViewportX
        {
            get { return viewport.X; }
            set { viewport.X = value; }
        }
        public int ViewportY
        {
            get { return viewport.Y; }
            set { viewport.Y = value; }
        }

        #endregion

        public Camera(Viewport newViewport)
        {
            viewport = newViewport;
        }

        public void Update(Vector2 playerPosition) // camera follows the player
        {
            if (playerPosition.X - viewport.X / 2 <= 0)
            {
                center = new Vector2(viewport.X / 2, viewport.Y / 2);
                border = 0;
            }
            else if (playerPosition.X + 640 >= 2000)
            {
                center = new Vector2(1360, viewport.Y / 2);
                border = 720;
            }
            else
            {
                center = new Vector2(playerPosition.X, viewport.Y / 2);
                border = playerPosition.X - 640;
            }
            transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) *
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * 
                Matrix.CreateTranslation(new Vector3(viewport.X/2,viewport.Y/2,0));

        }
    }
}
