using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MapManager;
using AIManager;
using CollisionHelper;
using PlayerManager;
using ObjectPool;
using UIManager;
using CameraManager;


namespace MapManager
{
    public sealed class Level : ILevel
    {
        #region Fields, Properties and contructor

        private readonly Texture2D background;
        private readonly int width, height;
        private readonly int enemiesToKill = 2;
        private readonly Ogre ogre; // a variable to pass it's parameters to spawner
        private readonly Ghost ghost; // a variable to pass it's parameters to spawner
        private readonly Item potion;
        private readonly Spawner<Ghost> ghostSpawner;
        private readonly Spawner<Ogre> ogreSpawner;
        private readonly Spawner<Item> potionSpawner;
        private readonly Introduction introduction;
        private readonly SpriteFont text;
        private readonly Boss boss;
        private List<Item> items = new List<Item>();
        private List<NPC> npcs = new List<NPC>();
        private float spawnCd;
        private Stage currentStage;
        private Random random = new Random();
        public enum Stage
        {
            Introduction,
            MainStage,
            BossFight,
            Finished,
            Lost
        }
        public static List<CollisionTiles> collisionTiles = new List<CollisionTiles>();
        public List<CollisionTiles> CollisionTiles
        {
            get { return collisionTiles; }
            set { collisionTiles = value; }
        }
        public List<Item> Items
        {
            get { return items; }
        }
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }        
        public List<NPC> NPCS
        {
            get { return npcs; }
        }
        public Introduction Introduction
        {
            get { return introduction; }
        }
        public Stage CurrentStage
        {
            get { return currentStage; }
        }


        public Level(int[,] map, int size, Texture2D background, Ghost ghost, Ogre ogre, Boss boss, Item potion, Introduction introduction, ContentManager Content)
        {
            this.background = background;
            this.ogre = ogre;
            this.ghost = ghost;
            this.boss = boss;
            this.potion = potion;
            this.introduction = introduction;
            text = Content.Load<SpriteFont>("text");
            width = map.GetLength(1) * size;
            height = map.GetLength(0) * size;
            ogreSpawner = new Spawner<Ogre>();
            ghostSpawner = new Spawner<Ghost>();
            potionSpawner = new Spawner<Item>();
            currentStage = Stage.Introduction;
        }


        #endregion


        #region Public methods

        public void Update(GameTime gameTime, Player player, Camera camera)
        {
            ChangeState(player);
            if (currentStage == Stage.Introduction)
            {
                introduction.Update(gameTime);
                return;
            }
            if (currentStage == Stage.MainStage)
            {
                ManageItems(gameTime,player);
                ManageEntitiesCollision(player);
                ManageNpc(gameTime, player);
                return;
            }
            if (currentStage == Stage.BossFight)
            {
                ManageEntitiesCollision(player);
                boss.Update(gameTime, player);
                foreach (CollisionTiles tile in collisionTiles)
                {
                    boss.ManageMapCollision(tile.rect1, width, height);
                }
            }
            if (currentStage == Stage.Finished)
            {
                 
            }
        }

        public void ManageMapCollision(Player player)
        {
            foreach (CollisionTiles tile in collisionTiles)
            {
                foreach (NPC npc in npcs)
                {
                    npc.ManageMapCollision(tile.rect1, width, height);
                }
                foreach (Item item in items)
                {
                    item.ManageMapCollision(tile.rect1, width, height);
                }
                player.ManageMapCollision(tile.rect1, width, height);
            }
        }

        public void ManageEntitiesCollision(Player player)
        {
            if (currentStage == Stage.MainStage)
            {
                foreach (NPC npc in npcs)
                {
                    for (int i = 0; i < player.Fireballs.UsedItems.Count; i++)
                    {
                        npc.RemoveLife(player, player.Fireballs.UsedItems[i]);
                    }
                    for (int j = 0; j < player.FrostBolts.UsedItems.Count; j++)
                    {
                        npc.Slow(player, player.FrostBolts.UsedItems[j]);
                    }
                }
            }

            if (currentStage == Stage.BossFight)
            {
                for (int i = 0; i < player.Fireballs.UsedItems.Count; i++)
                {
                    boss.RemoveLife(player, player.Fireballs.UsedItems[i]);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, width, height), Color.White);
            foreach (CollisionTiles tile in collisionTiles)
            {
                tile.Draw(spriteBatch);
            }
            if (currentStage == Stage.Introduction)
            {
                introduction.Draw(spriteBatch);
            }
            if (currentStage == Stage.MainStage)
            {
                foreach (Item item in items)
                {
                    item.Draw(spriteBatch);
                }
                foreach (NPC npc in npcs)
                {
                    npc.Draw(spriteBatch, gameTime);
                }
            }
            if (currentStage == Stage.BossFight)
            {
                boss.Draw(spriteBatch, gameTime);
            }
            if (currentStage == Stage.Finished)
            {
                spriteBatch.DrawString(text, "YOU WIN!!!", camera.Center - text.MeasureString("YOU WIN!!!") / 2, Color.White);
            }
            if (currentStage == Stage.Lost)
            {
                spriteBatch.DrawString(text, "YOU LOST", camera.Center - text.MeasureString("YOU LOST") / 2, Color.White);
            }
        }

        #endregion


        #region Private methods

        private void ManageItems(GameTime gameTime, Player player)
        {
            if (items.Count < 1)
            {
                items.Add(potionSpawner.SpawnMonster(potion.Texture, potion.Rectangle, new Vector2(random.Next(width - potion.Texture.Width), random.Next(400, 600))));
            }
            foreach (Item item in items)
            {
                item.Update(gameTime);
            }
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].TryToConsume(player.EntityRectangle))
                {
                    player.ConsumeItem(items[i].Health);
                    items.Remove(items[i]);
                }
            }
        }

        private void ManageNpc(GameTime gameTime, Player player)
        {
            if (currentStage == Stage.MainStage)
            {
                spawnCd += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (spawnCd > 2 && npcs.Count < 20)
                {
                    npcs.Add(ogreSpawner.SpawnMonster(ogre.MoveSheet, ogre.AttackSheet, ogre.DeathSheet, new Vector2(random.Next(2000), random.Next(400, 700))));
                  //  npcs.Add(ghostSpawner.SpawnMonster(ghost.MoveSheet, ghost.AttackSheet, ghost.DeathSheet, new Vector2(random.Next(2000), random.Next(700))));
                    spawnCd = 0;
                }
                for (int i = 0; i < npcs.Count; i++)
                {
                    if (!npcs[i].Alive)
                    {
                        npcs.Remove(npcs[i]);
                        player.EnemiesKilled++;
                    }
                }
                foreach (NPC npc in npcs)
                {
                    npc.Update(gameTime, player);
                }
            }
        }

        private void RemoveAllNPCS()
        {
            for (int i = 0; i < npcs.Count; i++)
            {
                npcs.Remove(npcs[i]);
            }
        }

        private void ChangeState(Player player)
        {
            if (currentStage == Stage.MainStage || currentStage == Stage.BossFight)
            {
                if (!player.IsAlive)
                {
                    currentStage = Stage.Lost;
                }
            }
            if (currentStage == Stage.Introduction)
            {
                if (introduction.Finished)
                {
                    currentStage = Stage.MainStage;
                }
            }
            if (currentStage == Stage.MainStage)
            {
                if (enemiesToKill == player.EnemiesKilled)
                {
                    RemoveAllNPCS();
                    currentStage = Stage.BossFight;
                }
            }
            if (currentStage == Stage.BossFight)
            {
                if (!boss.Alive)
                {
                    currentStage = Stage.Finished;
                }
            }
        }

        #endregion

    }
}
