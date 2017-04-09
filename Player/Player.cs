using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CollisionHelper;
using ObjectPool;
using AnimationManager;
using CameraManager;

namespace PlayerManager
{
    public sealed class Player : IEntity
    {

        #region Private fields and properties

        private AnimationPlayer animation;
        private Animation idle;
        private Animation walk;
        private Animation cast;
        private Animation death;
        private Animation knockback;
        private HealthBar healthBar;
        private Texture2D abilityTexture;
        private Texture2D frostBoltTexture;
        private Vector2 position;
        private Vector2 velocity;
        private KeyboardState oldState;
        private KeyboardState newState;
        private MouseState oldMouseState;
        private MouseState newMouseState;
        private MouseState crutchState;
        private MouseState crutchState1;
        private Rectangle actorRectangle;
        private SpriteEffects flip;
        private bool hasJumped;
        private bool fireballCasted;
        private bool frostboltCasted;
        private bool wasAttacked;
        private bool isAlive;
        private int lives;
        private int enemiesKilled;
        float fireballCastTime;
        float frostboltCastTime;
        float kickbackTime;
        public HealthBar HealthBar
        {
            get { return healthBar; }
        }
        public bool IsAlive
        {
            get { return isAlive; }
        }
        private ObjectPool<Fireball> fireballs;
        private ObjectPool<Frostbolt> frostBolts;
        public ObjectPool<Fireball> Fireballs
        {
            get { return fireballs; }
        }
        public ObjectPool<Frostbolt> FrostBolts
        {
            get { return frostBolts; }
        }
        public Rectangle EntityRectangle
        {
            get { return actorRectangle; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
        }
        public int EnemiesKilled
        {
            get { return enemiesKilled; }
            set { enemiesKilled = value; }
        }


        public Player(Vector2 position) 
        {
            this.position = position;
            isAlive = true;
            lives = 13;
        }

        #endregion


        #region Public methods

        public void Load(ContentManager Content)
        {
            walk = new Animation(Content.Load<Texture2D>("walking"), 62, 70f, true);
            idle = new Animation(Content.Load<Texture2D>("idle"), 62, 800f, true);
            cast = new Animation(Content.Load<Texture2D>("cast"), 37, 10f, true);
            death = new Animation(Content.Load<Texture2D>("death"), 47, 800f, false);
            knockback = new Animation(Content.Load<Texture2D>("knockback"), 46, 10f, false);
            healthBar = new HealthBar(Content.Load<Texture2D>("healthbarsmall"), lives);
            abilityTexture = Content.Load<Texture2D>("fireball");
            frostBoltTexture = Content.Load<Texture2D>("frostbolt");
            fireballs = new ObjectPool<Fireball>(() => new Fireball(abilityTexture, new Rectangle(0, 0, 11, 11)));
            frostBolts = new ObjectPool<Frostbolt>(() => new Frostbolt(frostBoltTexture, new Rectangle(0, 0, 28, 29)));
            animation.Animation = walk;
        }
        
        public void Update(GameTime gameTime, Camera camera)
        {
            if (isAlive)
            {
                position += velocity;
                ManageFireBalls(gameTime, camera);
                ManageFrostBolts(gameTime, camera);
                ManageAnimation(gameTime);
                HandleInput(gameTime);
                CalculatePosition(camera);
                healthBar.Update(gameTime, this.actorRectangle);
            }
            if (!isAlive) animation.Playanimation(death);
        }

        public void ManageMapCollision(Rectangle tileRectangle, int xOffSet, int yOffSet)
        {
            if (actorRectangle.TouchTopOf(tileRectangle))
            {
                actorRectangle.Y = tileRectangle.Y - actorRectangle.Height;
                position.Y = actorRectangle.Y;
                velocity.Y = 0f;
                hasJumped = false;
            }
            if (actorRectangle.TouchLeftOf(tileRectangle))
            {
                position.X = tileRectangle.X - actorRectangle.Width - 2;
            }
            if (actorRectangle.TouchRightOf(tileRectangle))
            {
                position.X = tileRectangle.X + actorRectangle.Width/2 + 2;
            }
            if (actorRectangle.TouchBootomOf(tileRectangle))
            {
                velocity.Y = 1f;
            }
            
            if (position.X < 0) position.X = 0;
            if (position.X > xOffSet - actorRectangle.Width) position.X = xOffSet - actorRectangle.Width;
            if (position.Y < 0) velocity.Y = 1f;
        }

        public void ManageEntitiesCollision(int damage, Vector2 attackSide)
        {
            wasAttacked = true;
            healthBar.ReduceHpAmount(damage);
            if (attackSide.X > position.X)
            {
                position.X = position.X - 7;
            }
            if (attackSide.X < position.X)
            {
                position.X = position.X + 7;
            }
        }

        public void ConsumeItem(int healthToRestore)
        {
            healthBar.RestoreHP(healthToRestore);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            animation.Draw(gameTime, spriteBatch, position, flip);
            foreach (Fireball ability in fireballs.Items)
            {
                if (ability.IsUsed && !ability.IsDisposed) ability.Draw(spriteBatch,gameTime);
            }
            foreach (Frostbolt ability in frostBolts.Items)
            {
                if (ability.IsUsed && !ability.IsDisposed) ability.Draw(spriteBatch, gameTime);
            }
            healthBar.Draw(spriteBatch);
        }

        public void GetOnMap(Vector2 positionToSpawn)
        {
            position = positionToSpawn;
        }

        #endregion


        #region Private methods

        //<summary> ѕо€снение за абилки
        // ≈сть два контейнера, один хранит объекты, которые еще не использованы, а второй объекты, которые наход€тс€ в движении.
        // ѕри использовании они хран€тс€ в обоих контейнерах, поскольку нужно провер€ть их свойстви и одновременно выполн€ть update() + collision();
        // “акже присутствуют два метода updade. ќдин используетс€ дл€ считывани€ позиции персонажа и способности соответственно, второй дл€ движени€ использованных абилок.
        //</summary>
        private void ManageFrostBolts(GameTime gameTime, Camera camera)
        {
            if (frostBolts.Items.Count < 20)
            {
                frostBolts.AddItem(new Frostbolt(frostBoltTexture, actorRectangle));
            }

            for (int i = 0; i < frostBolts.Items.Count; i++)
            {
                if (frostBolts.Items[i].IsDisposed)
                {
                    frostBolts.RemoveItem(frostBolts.Items[i]);
                }
            }

            if (frostBolts.UsedItems.Count > 10)
            {
                frostBolts.RemoveItem(frostBolts.GetUsedItemProperties());
            }

            foreach (Frostbolt ability in frostBolts.Items)
            {
                if (!ability.IsUsed)
                    ability.UpdateLocation(gameTime, actorRectangle);
                if (ability.IsUsed)
                    ability.ManageDirection(gameTime, crutchState1, actorRectangle, camera);                
            }
        }

        private void ManageFireBalls(GameTime gameTime, Camera camera)
        {
            if (fireballs.Items.Count < 70)
            {
                fireballs.AddItem(new Fireball(abilityTexture, actorRectangle));
            }

            for (int i = 0; i < fireballs.Items.Count; i++)
            {
                if (fireballs.Items[i].IsDisposed)
                {
                    fireballs.RemoveItem(fireballs.Items[i]);
                }
            }

            if (fireballs.UsedItems.Count > 50)
            {
                fireballs.RemoveItem(fireballs.GetUsedItemProperties());
            }

            foreach (Fireball ability in fireballs.Items)
            {
                if (!ability.IsUsed)
                    ability.UpdateLocation(gameTime, actorRectangle);
                if (ability.IsUsed)
                    ability.ManageDirection(gameTime, crutchState, actorRectangle, camera);
            }
        }

        private void HandleInput(GameTime gameTime)
        {
            newState = Keyboard.GetState();
            newMouseState = Mouse.GetState();
            if (oldMouseState.LeftButton == ButtonState.Pressed && newMouseState.LeftButton == ButtonState.Released && !fireballCasted)
            {
                if (!fireballs.GetItemProperties().IsUsed)
                {
                    fireballs.GetItem().IsUsed = true;
                    crutchState = oldMouseState;
                    fireballCasted = true;
                }
            }
            if (oldMouseState.RightButton == ButtonState.Pressed && newMouseState.RightButton == ButtonState.Released && !frostboltCasted)
            {
                if (!frostBolts.GetItemProperties().IsUsed)
                {
                    frostBolts.GetItem().IsUsed = true;
                    crutchState1 = oldMouseState;
                    frostboltCasted = true;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                velocity.X = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 4;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                velocity.X = -(float)gameTime.ElapsedGameTime.TotalMilliseconds / 4;
            }
            else velocity.X = 0f;

            if (newState.IsKeyDown(Keys.W) && hasJumped == false && !oldState.IsKeyDown(Keys.W))
            {
                position.Y -= 3;
                velocity.Y -= 5f;
                hasJumped = true;
            }
            oldState = newState;
            oldMouseState = newMouseState;
        }

        private void ManageAnimation(GameTime gameTime)
        {
            if (velocity.X != 0) animation.Playanimation(walk);
            if (velocity.X == 0) animation.Playanimation(idle);
            if (wasAttacked && kickbackTime < 300f)
            {
                animation.Playanimation(knockback);
                kickbackTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            if (wasAttacked && kickbackTime > 200f)
            {
                wasAttacked = false;
                animation.Playanimation(knockback);
                kickbackTime = 0;
            }
            if (crutchState.LeftButton == ButtonState.Pressed && fireballCastTime <= 0.2 && fireballCasted)
            {
                fireballCastTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                animation.Playanimation(cast);
                if (fireballCastTime > 0.2) fireballCasted = false;
            }
            if (oldMouseState.LeftButton == ButtonState.Pressed && !fireballCasted)
            {
                fireballCastTime = 0;
            }
            if (crutchState1.RightButton == ButtonState.Pressed && frostboltCastTime <= 0.5 && frostboltCasted)
            {
                frostboltCastTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                animation.Playanimation(cast);
                if (frostboltCastTime > 0.5) frostboltCasted = false;
            }
            if (oldMouseState.RightButton == ButtonState.Pressed && !frostboltCasted)
            {
                frostboltCastTime = 0;
            }
            if (healthBar.Width <= 0) isAlive = false;
        }

        private void CalculatePosition(Camera camera)
        {
            actorRectangle = new Rectangle((int)position.X, (int)position.Y, 62, 50);
            if (velocity.Y < 10)
            {
                velocity.Y += 0.2f;
            }
            if (newMouseState.X + camera.Border > actorRectangle.X) { flip = SpriteEffects.None; }
            if (newMouseState.X + camera.Border < actorRectangle.X) { flip = SpriteEffects.FlipHorizontally; }
        }

        #endregion

    }
}
