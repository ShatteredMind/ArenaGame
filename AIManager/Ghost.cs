using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnimationManager;
using CollisionHelper;
using PlayerManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AIManager
{
    public sealed class Ghost : NPC
    {
        private Behaviour currentState;
        public Ghost() { }
        enum Behaviour
        {
            AttackMode,
            PursueMode,
            DeathMode,
        }


        #region Public methods and Constructor

        public Ghost(Texture2D moveSheet, Texture2D attackSheet, Texture2D deathSheet, Vector2 positionToSpawn)
        {
            currentState = Behaviour.PursueMode; // Ghost will never appear near the player. Mostly all of them will be spawned higher than the player
            this.moveSheet = moveSheet;
            this.attackSheet = attackSheet;
            this.deathSheet = deathSheet;
            position = positionToSpawn;
            damage = 5;
            speed = 150f;
            alive = true;
            deathAnimationFinished = false;
            lives = 4;
            movement = new Animation(moveSheet, 44, 100f, true);
            attack = new Animation(attackSheet, 45, 60f, true);
            death = new Animation(deathSheet, 45, 60f, false);
            animationPlayer.Animation = attack;            
        } 

        public override void Update(GameTime gameTime, Player player)
        {
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
                RemoveGhost(gameTime);
            }
            ManageAnimation(player.EntityRectangle, player, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            animationPlayer.Draw(gameTime, spriteBatch, position, flip);
        }

        public void ManageEntitiesCollision(Player player, Fireball usedFireball, Frostbolt usedFrostbolt)
        {
            base.RemoveLife(player, usedFireball);
            base.Slow(player, usedFrostbolt);
        }

        #endregion


        #region Private and protected methods

        protected override void PursuePlayer(Rectangle playerPosition, GameTime gameTime)
        {
           base.PursuePlayer(playerPosition,gameTime);
        }

        protected override void AttackPlayer(GameTime gameTime, Player player)
        {
            base.AttackPlayer(gameTime, player);
        }

        private void RemoveGhost(GameTime gameTime)
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

        private void ChangeBehaviour(Rectangle playerPosition)
        {
            if (!npcRectangle.Intersects(playerPosition))
            {
                currentState = Behaviour.PursueMode;
            }
            if (npcRectangle.Intersects(playerPosition))
            {
                currentState = Behaviour.AttackMode;
            }
            if (lives <= 0)
            {
                currentState = Behaviour.DeathMode;
            }
        }

        #endregion

    }
}
