using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnimationManager;
using CollisionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerManager;
using ObjectPool;

namespace AIManager
{
    public sealed class Boss : IEntity
    {
        #region Fields, properties and constructor

        private readonly Animation attack;
        private readonly Animation idle;
        private readonly Texture2D attackSheet;
        private readonly Texture2D idleSheet;
        private readonly Texture2D abilityTexture;
        private readonly SpriteEffects flip;
        private readonly int damage = 1;
        private AnimationPlayer animationPlayer;
        private Rectangle npcRectangle;
        private int lives = 50;
        private bool alive;
        private float attackCooldown;
        private Vector2 velocity;
        private Vector2 position;
        private ObjectPool<BossAbility> bossAbilities;
        private Behaviour currentState;
        enum Behaviour
        {
            AttackMode,
            DeathMode,
        }
        public Rectangle EntityRectangle
        {
            get { return npcRectangle; }
        }
        public bool Alive
        {
            get { return alive; }
        }
        public Vector2 Position
        {
            get { return position; }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
        }


        public Boss(Texture2D idleSheet, Texture2D attackSheet, Vector2 position, Texture2D abilityTexture)
        {
            this.idleSheet = idleSheet;
            this.attackSheet = attackSheet;
            this.position = position;
            this.abilityTexture = abilityTexture;
            flip = SpriteEffects.None;
            alive = true;
            idle = new Animation(idleSheet, 89, 100f, true);
            attack = new Animation(attackSheet, 86, 70f, true);
            animationPlayer.Animation = attack;
            currentState = Behaviour.AttackMode;
            bossAbilities = new ObjectPool<BossAbility>(() => new BossAbility(abilityTexture, new Rectangle((int)position.X, (int)position.Y, abilityTexture.Width, abilityTexture.Height)));
        }

        #endregion


        #region Public methods

        public void ManageMapCollision(Rectangle tileRectangle, int xoffSet, int yoffSet)
        {
            if (npcRectangle.TouchTopOf(tileRectangle))
            {
                npcRectangle.Y = tileRectangle.Y - npcRectangle.Height;
                position.Y = npcRectangle.Y;
                velocity.Y = 0f;
            }
            if (position.X < 0) position.X = 0;
            if (position.X > xoffSet - npcRectangle.Width) position.X = xoffSet - npcRectangle.Width;
            if (position.Y < 0) velocity.Y = 1f;
            if (position.Y > yoffSet) position.Y -= attack.frameHeight; 
        }

        public void RemoveLife(Player player, Fireball usedFireball)
        {
            if (usedFireball.EntityRectangle.Intersects(npcRectangle))
            {
                lives--;
                player.Fireballs.RemoveItem(usedFireball);
                usedFireball.Dispose();
            }
        }

        public void ManageAbilities(GameTime gameTime, Vector2 playerPosition)
        {
            if (bossAbilities.Items.Count < 20)
            {
                bossAbilities.AddItem(new BossAbility(abilityTexture, npcRectangle));
            }

            for (int i = 0; i < bossAbilities.Items.Count; i++)
            {
                if (bossAbilities.Items[i].IsDisposed)
                {
                    bossAbilities.RemoveItem(bossAbilities.Items[i]);
                }
            }
            if (bossAbilities.UsedItems.Count > 10)
            {
                bossAbilities.RemoveItem(bossAbilities.GetUsedItemProperties());
            }

            foreach (BossAbility ability in bossAbilities.Items)
            {
                if (ability.IsUsed)
                    ability.ManageDirection(gameTime, playerPosition, npcRectangle);
                if (!ability.IsUsed)
                    ability.UpdateAbilityLocation(gameTime, position);
            }
        }

        public void Update(GameTime gameTime, Player player)
        {
            ChangeBehaviour();
            if (currentState == Behaviour.AttackMode)
            {
                position += velocity;
                if (velocity.Y < 10)
                {
                    velocity.Y += 0.2f;
                }
                animationPlayer.Playanimation(attack);
                ManageAbilities(gameTime, player.Position);
                ManageEntitiesCollision(player);
                AttackPlayer(gameTime, player);
                npcRectangle = new Rectangle((int)position.X, (int)position.Y, attack.FrameWidth, attack.frameHeight);
            }
            if (currentState == Behaviour.DeathMode)
            {
                RemoveBoss(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            animationPlayer.Draw(gameTime, spriteBatch, position, flip);
            foreach (BossAbility ability in bossAbilities.UsedItems)
            {
                ability.Draw(spriteBatch, gameTime);
            }
        }

        #endregion


        #region Private methods

        private void AttackPlayer(GameTime gameTime, Player player)
        {
            attackCooldown += gameTime.ElapsedGameTime.Milliseconds;
            if (!bossAbilities.GetItemProperties().IsUsed && attackCooldown > 600)
            {
                bossAbilities.GetItem().IsUsed = true;
                attackCooldown = 0;
            }
        }

        private void ChangeBehaviour()
        {
            if (currentState == Behaviour.AttackMode)
            {
                if (lives <= 0)
                {
                    currentState = Behaviour.DeathMode;
                }
            }
        }

        private void RemoveBoss(GameTime gameTime)
        {
            alive = false;
        }

        private void ManageEntitiesCollision(Player player)
        {
            for (int i = 0; i < bossAbilities.UsedItems.Count; i++)
            {
                if (bossAbilities.UsedItems[i].EntityRectangle.Intersects(player.EntityRectangle))
                {
                    player.ManageEntitiesCollision(damage, position);
                    bossAbilities.Items[i].Dispose();
                    bossAbilities.UsedItems[i].Dispose();
                    bossAbilities.Items.Remove(bossAbilities.UsedItems[i]);
                    bossAbilities.UsedItems.Remove(bossAbilities.UsedItems[i]);
                }
            }
        }

        #endregion

    }
}
