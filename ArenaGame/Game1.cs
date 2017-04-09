using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AIManager;
using PlayerManager;
using System.IO;
using CameraManager;
using UIManager;
using MapManager;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ArenaGame
{
    class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MainMenu menu;
        Options options;
        GameState currentGameState;
        Thread loadThread;
        bool isLoading;
        MouseState mouseState;
        MouseState previousMouseState;
        Texture2D loadingScreen;
        Song battleTheme;
        Song menuTheme;
        SpriteFont winText;
        Level level1;
        KeyboardState newState;
        KeyboardState oldState;
        MapGenerator<Level> mapGenerator;
        Ghost ghost;
        Boss boss;
        Ogre ogre;
        Item item;
        Player player;
        Camera camera;
        float statisticCoolDown; // cooldown to show statistics
        enum GameState
        {
            Level1,
            MainMenu,
            Loading,
            Options,
            Suspended,
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            mapGenerator = new MapGenerator<Level>();
            menu = new MainMenu(GraphicsDevice.Viewport, Content.Load<Texture2D>("menu/BG"), Content.Load<Texture2D>("menu/Start"), Content.Load<Texture2D>("menu/Exit"), Content.Load<Texture2D>("menu/Options"));
            options = new Options(GraphicsDevice.Viewport, Content.Load<Texture2D>("menu/BG"), Content.Load<Texture2D>("menu/Back"));
            loadingScreen = Content.Load<Texture2D>("loadscreen");
            menuTheme = Content.Load<Song>("menutheme");
            battleTheme = Content.Load<Song>("battletheme");
            winText = Content.Load<SpriteFont>("text");
            player = new Player(new Vector2(0,0));
            IsMouseVisible = true;
            camera = new Camera(GraphicsDevice.Viewport);
            camera.ViewportX = 1280;
            camera.ViewportY = 800;
            currentGameState = GameState.MainMenu;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            camera.Update(player.Position);
            mouseState = Mouse.GetState();
            if (currentGameState == GameState.MainMenu)
            {

            }
            if (currentGameState == GameState.Loading && !isLoading) // !isLoading prevents load method from repeating 60 times(update method is called 60 times per second)
            {
                loadThread = new Thread(Level1Load);
                isLoading = true;
                loadThread.Start();
            }
            if (currentGameState == GameState.Suspended)
            {
                CheckStates(mouseState.X, mouseState.Y);
            }
            if (currentGameState == GameState.Level1)
            {
                Level1Update(gameTime);
                CheckStates(mouseState.X, mouseState.Y);
                if ((level1.CurrentStage == MapManager.Level.Stage.Lost) || (level1.CurrentStage == MapManager.Level.Stage.Finished))
                {
                    statisticCoolDown += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                CheckStates(mouseState.X, mouseState.Y);
            }
            if (currentGameState == GameState.Level1 && isLoading)
            {
                Level1Load();
                isLoading = false;
            }
            previousMouseState = mouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend
                , null, null, null, null,
                camera.Transform);
            if (currentGameState == GameState.MainMenu)
            {
                menu.Draw(spriteBatch);
            }
            if (currentGameState == GameState.Options)
            {
                options.Draw(spriteBatch);
            }
            if (currentGameState == GameState.Loading)
            {
                spriteBatch.Draw(loadingScreen, new Rectangle(0, 0, 1280, 800), Color.White);
            }
            if (currentGameState == GameState.Level1)
            {
                level1.Draw(spriteBatch, gameTime,camera);
                if (level1.Introduction.Finished)
                {
                    player.Draw(spriteBatch, gameTime);
                }
            }
            if (currentGameState == GameState.Suspended)
            {
                level1.Draw(spriteBatch, gameTime, camera);
                player.Draw(spriteBatch, gameTime);
                spriteBatch.DrawString(winText, "PAUSE", camera.Center - winText.MeasureString("PAUSE") / 2, Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        
        private void CheckStates(int x, int y)
        {
            Rectangle mouseRectangle = new Rectangle(x, y, 1, 1);
            if (currentGameState == GameState.Level1)
            {
                if (((level1.CurrentStage == MapManager.Level.Stage.Lost) || (level1.CurrentStage == MapManager.Level.Stage.Finished)) && statisticCoolDown > 5)
                {
                    ReturnToBasics();
                }
                newState = Keyboard.GetState();
                if (newState.IsKeyDown(Keys.Escape) && !oldState.IsKeyDown(Keys.Escape))
                {
                    currentGameState = GameState.Suspended;
                }
                oldState = newState;
            }
            if (currentGameState == GameState.Suspended)
            {
                newState = Keyboard.GetState();
                if (newState.IsKeyDown(Keys.Escape) && !oldState.IsKeyDown(Keys.Escape))
                {
                    currentGameState = GameState.Level1;
                }
                oldState = newState;
            }
            if (currentGameState == GameState.MainMenu)
            {
                if (mouseRectangle.Intersects(menu.PlayButtonRectangle))
                {
                    menu.ChangeColour(); // gotta change colour when intersects
                    currentGameState = GameState.Loading;
                    isLoading = false;
                }
                else if (mouseRectangle.Intersects(menu.OptionsButtonRectangle))
                {
                    currentGameState = GameState.Options;
                }
                else if (mouseRectangle.Intersects(menu.ExitButtonRectangle))
                {
                    Exit();
                }
            }
            if (currentGameState == GameState.Options)
            {
                if (mouseRectangle.Intersects(options.BackButtonRectangle))
                {
                    currentGameState = GameState.MainMenu;
                }
            }
        }

        private void Level1Load()
        {
            MediaPlayer.Stop();
            ghost = new Ghost(Content.Load<Texture2D>("ghost_walking"), Content.Load<Texture2D>("ghost_attack"), Content.Load<Texture2D>("ghost_rip"), new Vector2(0,0));
            ogre = new Ogre(Content.Load<Texture2D>("ogre_walking"), Content.Load<Texture2D>("ogre_attack"), Content.Load<Texture2D>("ogre_death"), new Vector2(0,0));
            item = new Item(Content.Load<Texture2D>("bottle1"), new Rectangle(0, 0, 30, 30), new Vector2(0,-10));
            boss = new Boss(Content.Load<Texture2D>("boss/bossIdle"), Content.Load<Texture2D>("boss/bossAttack"), new Vector2(1000, 700), Content.Load<Texture2D>("boss/bossFireball"));
            Introduction introduction = new Introduction(Content.Load<Texture2D>("int"),Content.Load<Texture2D>("OKBUTTON"));
            Tiles.Content = Content;
            level1 = mapGenerator.Generate(new int[,]{
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,5,5,5,5,5,5,0,6,6,6,5,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,6,6,6,6,6,6,6,0,0,0,0,0,0,5,5,5,5,5,5,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,6,6,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,6,6,6,6,0,0,5,5,5,5,5,5,0,0,0,0,0,0},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,5,5,5,5,5,5,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,5,5},
               {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,6,6,6},
               {5,5,5,0,0,0,0,0,0,0,5,5,5,5,5,5,5,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,6,6,6},
               {6,6,6,5,5,5,5,5,5,5,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,6,6,6,6},
               {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,6,6,6,6,6},
               {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                 }, 32,  Content.Load<Texture2D>("backgrounds/background0"), ghost, ogre, boss, item, introduction, Content);
            player.Load(Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            currentGameState = GameState.Level1;
           // MediaPlayer.Play(battleTheme);
            isLoading = true;
            player.GetOnMap(new Vector2(0, 700));
        }
      
        private void Level1Update(GameTime gameTime)
        {
            Ogre b = new Ogre();
            Ghost c = new Ghost();
            if (level1.Introduction.Finished)
            {
                player.Update(gameTime, camera);
            }
            level1.Update(gameTime, player, camera);
            Parallel.ForEach(level1.CollisionTiles, (tile) => // it was too much for a single map(level) too handle all the collisions
            {
                foreach (NPC npc in level1.NPCS)
                {
                    npc.ManageMapCollision(tile.rect1, level1.Width, level1.Height);
                }
                foreach (Item item in level1.Items)
                {
                    item.ManageMapCollision(tile.rect1, level1.Width, level1.Height);
                }
                player.ManageMapCollision(tile.rect1, level1.Width, level1.Height);
            }); 
            
        }

        private void ReturnToBasics()
        {
            MediaPlayer.Stop();
            camera = new Camera(GraphicsDevice.Viewport);
            camera.ViewportX = 1280;
            camera.ViewportY = 800;
            player = new Player(new Vector2(0, 0));
            isLoading = false;
            statisticCoolDown = 0;
            currentGameState = GameState.MainMenu;
            MediaPlayer.Play(menuTheme);
        }

    }
}
