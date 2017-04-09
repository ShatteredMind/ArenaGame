using System;
using AnimationManager;
using CollisionHelper;
using PlayerManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AIManager
{
    public sealed class Ogre : NPC, IEntity
    {
        private Behaviour currentState;
        public Ogre() { }
        enum Behaviour
        {
            AttackMode,
            PursueMode,
            DeathMode,
        }


        #region Public methods and constructor

        public Ogre(Texture2D _moveSheet, Texture2D _attackSheet, Texture2D _deathSheet, Vector2 positionToSpawn)
        {
            currentState = Behaviour.PursueMode;
            moveSheet = _moveSheet;
            attackSheet = _attackSheet;
            deathSheet = _deathSheet;
            position = positionToSpawn;
            damage = 10;
            speed = 100f;
            alive = true;
            deathAnimationFinished = false;
            lives = 5;
            movement = new Animation(moveSheet, 46, 100f, true);
            attack = new Animation(attackSheet, 46, 60f, true);
            death = new Animation(deathSheet, 46, 50f, false);
            animationPlayer.Animation = attack;
        }

        public override void Update(GameTime gameTime, Player player)
        {
            position += velocity;
            if (velocity.Y < 10)
            {
                velocity.Y += 0.2f;
            }
            ChangeBehaviour(player.EntityRectangle);
            if (currentState == Behaviour.PursueMode && player.IsAlive)
            {
                PursuePlayer(player.EntityRectangle, gameTime);
            }
            if (currentState == Behaviour.AttackMode)
            {
                AttackPlayer(gameTime, player);
            }
            if (currentState == Behaviour.DeathMode)
            {
                RemoveOgre(gameTime);
            }
            ManageAnimation(player.EntityRectangle, player, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            animationPlayer.Draw(gameTime, spriteBatch, position, flip);
        }

        public override void ManageMapCollision(Rectangle tileRectangle, int xoffSet, int yoffSet)
        {
            if (npcRectangle.TouchTopOf(tileRectangle))
            {
                npcRectangle.Y = tileRectangle.Y - npcRectangle.Height;
                position.Y = npcRectangle.Y;
                velocity.Y = 0f;
            }
            if (npcRectangle.TouchLeftOf(tileRectangle))
            {
                position.X = tileRectangle.X - npcRectangle.Width - 2;
            }
            if (npcRectangle.TouchRightOf(tileRectangle))
            {
                position.X = tileRectangle.X + npcRectangle.Width/2 + 13;
            }
            if (npcRectangle.TouchBootomOf(tileRectangle))
            {
                velocity.Y = 1f;
            }

            if (position.X < 0) position.X = 0;
            if (position.X > xoffSet - npcRectangle.Width) position.X = xoffSet - npcRectangle.Width;
            if (position.Y < 0) velocity.Y = 1f;
        }

        #endregion


        #region Private and protected methods

        protected override void PursuePlayer(Rectangle playerPosition, GameTime gameTime)
        {
            base.PursuePlayer(playerPosition, gameTime);
        }

        protected override void AttackPlayer(GameTime gameTime, Player player)
        {
            base.AttackPlayer(gameTime, player);
        }

        private void RemoveOgre(GameTime gameTime)
        {
            base.RemoveNPC(gameTime);
        }

        private void ManageAnimation(Rectangle playerPosition, Player player, GameTime gameTime)
        {
            if (currentState == Behaviour.PursueMode)
            {
                animationPlayer.Playanimation(movement);
            }
            if (currentState == Behaviour.AttackMode)
            {
                animationPlayer.Playanimation(attack);
            }
            if (currentState == Behaviour.DeathMode)
            {
                animationPlayer.Playanimation(death);
            }
            if (position.X >= playerPosition.X) flip = SpriteEffects.FlipHorizontally;
            if (position.X < playerPosition.X) flip = SpriteEffects.None;
        }

        public void ManageEntitiesCollision(Player player, Fireball usedFireball, Frostbolt usedFrostbolt)
        {
            base.RemoveLife(player, usedFireball);
            base.Slow(player, usedFrostbolt);
        }

        private void ChangeBehaviour(Rectangle playerPosition)
        {
            if (!npcRectangle.Intersects(playerPosition) && lives > 0)
            {
                currentState = Behaviour.PursueMode;
                return; // Return is very important!
            }
            if (npcRectangle.Intersects(playerPosition) && lives > 0)
            {
                currentState = Behaviour.AttackMode;
                return;
            }
            if (lives <= 0)
            {
                currentState = Behaviour.DeathMode;
                return;
            }
        }

        #endregion

    }
}
