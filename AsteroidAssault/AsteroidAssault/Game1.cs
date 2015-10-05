using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AsteroidAssault
    // Robert Foder
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
       

        // pg. 90
        enum GameStates { TitleScreen, Playing, PlayerDead, GameOver };
        //pg. 102
        GameStates gameState = GameStates.TitleScreen; //pg. 162
        Texture2D titleScreen;
        Texture2D spriteSheet;
        //pg. 102
        StarField starField;
         //pg. 110
        AsteroidManager asteroidManager;
         //pg. 125
        PlayerManager playerManager;
         //pg. 138
        EnemyManager enemyManager;
        //pg. 150 declarations
        ExplosionManager explosionManager;
        //pg. 156
        CollisionManager collisionManager;

        //pg. 162
        SpriteFont pericles14;

        private float playerDeathDelayTime = 10f;
        private float playerDeathTimer = 0f;
        private float titleScreenTimer = 0f;
        private float titleScreenDelayTime = 1f;

        private int playerStartingLives = 3;
        private Vector2 playerStartLocation = new Vector2(390, 550);
        private Vector2 scoreLocation = new Vector2(20, 10);
        private Vector2 livesLocation = new Vector2(20, 25);


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //pg 90
            titleScreen = Content.Load<Texture2D>(@"TitleScreen");
            spriteSheet = Content.Load<Texture2D>(@"spriteSheet");

           // pg. 103
            starField = new StarField(
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height,
                200,
                new Vector2(0, 30f),
                spriteSheet,
                new Rectangle(0, 450, 2, 2));
            //pg. 110
            asteroidManager = new AsteroidManager(
                10,
                spriteSheet,
                new Rectangle(0, 0, 50, 50),
                20,
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height);
            // pg. 125
            playerManager = new PlayerManager(
                spriteSheet,
                new Rectangle(0, 150, 50, 50),
                3,
                new Rectangle(
                    0,
                    0,
                    this.Window.ClientBounds.Width,
                    this.Window.ClientBounds.Height));
        
            //pg. 138
            enemyManager = new EnemyManager(
                spriteSheet,
                new Rectangle(0, 200, 50, 50),
                6,
                playerManager,
                new Rectangle(
                    0,
                    0,
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height));
            //pg. 150 loadcontent
            explosionManager = new ExplosionManager(
                spriteSheet,
                new Rectangle(0, 100, 50, 50),
                3,
                new Rectangle(0, 450, 2, 2));
            //pg. 156
            collisionManager = new CollisionManager(
                asteroidManager,
                playerManager,
                enemyManager,
                explosionManager);
            //pg. 160
            SoundManager.Initialize(Content);

            //pg. 162
            pericles14 = Content.Load<SpriteFont>(@"Pericles14");
                    
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //pg. 163
        private void resetGame()
        {
            playerManager.playerSprite.Location = playerStartLocation;
            foreach (Sprite asteroid in asteroidManager.Asteroids)
            {
                asteroid.Location = new Vector2(-500, -500);
            }
            enemyManager.Enemies.Clear();
            enemyManager.Active = true;
            playerManager.PlayerShotManager.Shots.Clear();
            enemyManager.EnemyShotManager.Shots.Clear();
            playerManager.Destroyed = false;
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            // pg. 90
            switch (gameState)
            {
                case GameStates.TitleScreen:
                    //pg. 163
                    titleScreenTimer +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (titleScreenTimer >= titleScreenDelayTime)
                    {
                        if ((Keyboard.GetState().IsKeyDown(Keys.Space)) ||
                            (GamePad.GetState(PlayerIndex.One).Buttons.A ==
                            ButtonState.Pressed))
                        {
                            playerManager.LivesRemaining = playerStartingLives;
                            playerManager.PlayerScore = 0;
                            resetGame();
                            gameState = GameStates.Playing;
                        }
                    }
                    break;

                case GameStates.Playing:
                    //pg. 103
                    starField.Update(gameTime);
                    //pg.111
                    asteroidManager.Update(gameTime);
                    //pg. 125
                    playerManager.Update(gameTime);
                    //pg. 138
                    enemyManager.Update(gameTime);
                    //pg. 150 update
                    explosionManager.Update(gameTime);
                    //pg. 156
                    collisionManager.CheckCollisions();
                    //pg. 163-64
                    if (playerManager.Destroyed)
                    {
                        playerDeathTimer = 0f;
                        enemyManager.Active = false;
                        playerManager.LivesRemaining--;
                        if (playerManager.LivesRemaining < 0)
                        {
                            gameState = GameStates.GameOver;
                        }
                        else
                        {
                            gameState = GameStates.PlayerDead;
                        }
                    }
                    
                    break;

                case GameStates.PlayerDead:
                    //pg. 164
                    playerDeathTimer +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;

                    starField.Update(gameTime);
                    asteroidManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    playerManager.PlayerShotManager.Update(gameTime);
                    explosionManager.Update(gameTime);

                    if (playerDeathTimer >= playerDeathDelayTime)
                    {
                        resetGame();
                        gameState = GameStates.Playing;
                    }
                    break;

                case GameStates.GameOver:
                    //pg. 164-65
                    playerDeathTimer +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;
                    starField.Update(gameTime);
                    asteroidManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    playerManager.PlayerShotManager.Update(gameTime);
                    explosionManager.Update(gameTime);
                    if (playerDeathTimer >= playerDeathDelayTime)
                    {
                        gameState = GameStates.TitleScreen;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        { 
            //pg. 103
            GraphicsDevice.Clear(Color.Black);
            // pg 90
            spriteBatch.Begin();

            if (gameState == GameStates.TitleScreen)
            {
                spriteBatch.Draw(titleScreen,
                    new Rectangle(0, 0, this.Window.ClientBounds.Width,
                        this.Window.ClientBounds.Height),
                        Color.White);
            }

            if ((gameState == GameStates.Playing) ||
                (gameState == GameStates.PlayerDead) ||
                (gameState == GameStates.GameOver))
            {
                //pg. 103
                starField.Draw(spriteBatch);
                //pg. 111
                asteroidManager.Draw(spriteBatch);
                //pg. 125
                playerManager.Draw(spriteBatch);
                //pg. 138
                enemyManager.Draw(spriteBatch);
                //pg. 150 draw
                explosionManager.Draw(spriteBatch);
            

            //pg. 167
            spriteBatch.DrawString(
                pericles14,
                "Score: " + playerManager.PlayerScore.ToString(),
                scoreLocation,
                Color.White);

            if (playerManager.LivesRemaining >= 0)
            {
                spriteBatch.DrawString(
                    pericles14,
                    "Ships Remaining: " +
                    playerManager.LivesRemaining.ToString(),
                    livesLocation,
                    Color.White);
            }
        }
        if ((gameState == GameStates.GameOver))
    {
        spriteBatch.DrawString(
         pericles14,
           "G A M E  O V E R !",
           new Vector2(
               this.Window.ClientBounds.Width / 2 -
               pericles14.MeasureString
               ("G A M E  O V E R !").X / 2,
               50),
               Color.White);

            }

            spriteBatch.End();

            
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
