using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Mini_Project
{
    /// <summary>
    /// This is the main type for your gameg
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Sky and clouds background..
        Texture2D background;

        //Grass and trees scrolling background
        SpriteTexture mBackgroundOne;
        SpriteTexture mBackgroundTwo;
        SpriteTexture mBackgroundThree;
        SpriteTexture mBackgroundFour;
        SpriteTexture mBackgroundFive;

        

        //Player bug
        Texture2D playerTexture;

        // Represents the player 
        Player player;

        Texture2D explosionTexture;
        List<Animation> explosions;

        // The sound used when the player or a flower blooms and dies
        SoundEffect bloomSound;

        // The music played during gameplay
        Song gameplayMusic;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        static private int TimeOut = 4000;
        private double TimeCount = 0;

        // A movement speed for the player
        float playerMoveSpeed;

        // Enemies
        Texture2D enemyTexture;
        List<Enemy> enemies;

        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        // A random number generator
        Random random;

        Texture2D projectileTexture;
        List<Projectile> projectiles;

        // The rate of fire of the player sting
        TimeSpan fireTime;
        TimeSpan previousFireTime;

        //Number that holds the player score
        int score;
        // The font used to display UI elements
        SpriteFont font;

        enum GameState { Start, InGame, GameOver };
        GameState currentGameState = GameState.Start;

        Texture2D startScreen;
        Texture2D endScreen;

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

            // Initialize the player class
            player = new Player();

            // Set a constant player move speed
            playerMoveSpeed = 8.0f;

            //Enable the FreeDrag gesture.
            TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Tap | GestureType.Hold;
           

            projectiles = new List<Projectile>();

            explosions = new List<Animation>();

            //Set player's score to zero
            score = 0;

            // Set the sting to bloom every quarter second
            fireTime = TimeSpan.FromSeconds(.15f);

            mBackgroundOne = new SpriteTexture();
            mBackgroundOne.Scale = 1.0f;

            mBackgroundTwo = new SpriteTexture();
            mBackgroundTwo.Scale = 1.0f;

            mBackgroundThree = new SpriteTexture();
            mBackgroundThree.Scale = 1.0f;

            mBackgroundFour = new SpriteTexture();
            mBackgroundFour.Scale = 1.0f;

            mBackgroundFive = new SpriteTexture();
            mBackgroundFive.Scale = 1.0f;

            // Initialize the enemies list
            enemies = new List<Enemy>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);

            // Initialize our random number generator
            random = new Random();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>

        private void UpdatePlayer(GameTime gameTime)
        {

            player.Update(gameTime);

            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.FreeDrag)
                {
                    player.Position.Y -= gesture.Delta.Y;
                }
                else
                {
                }
                // close game if player health drops to zero
                if (player.Health <= 0)
                {
                    currentGameState = GameState.GameOver;
                }

                // Get Thumbstick Controls
                player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
                player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

                // Use the Keyboard / Dpad
                if (currentKeyboardState.IsKeyDown(Keys.Left) ||
                currentGamePadState.DPad.Left == ButtonState.Pressed)
                {
                    player.Position.X -= playerMoveSpeed;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Right) ||
                currentGamePadState.DPad.Right == ButtonState.Pressed)
                {
                    player.Position.X += playerMoveSpeed;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Up) ||
                currentGamePadState.DPad.Up == ButtonState.Pressed)
                {
                    player.Position.Y -= playerMoveSpeed;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Down) ||
                currentGamePadState.DPad.Down == ButtonState.Pressed)
                {
                    player.Position.Y += playerMoveSpeed;
                }

                // Make sure that the player does not go out of bounds
                player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width);
                player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height);

                // Shoot sting only every interval we set as the fireTime
                if (gameTime.TotalGameTime - previousFireTime > fireTime)
                {
                    // Reset our current time
                    previousFireTime = gameTime.TotalGameTime;

                    // Add the projectile, but add it to the front and center of the player
                    AddProjectile(player.Position + new Vector2(player.Width / 4, 0));
                }
            }
        }

        



        
 
        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

         

            mBackgroundOne.LoadContent(this.Content, "BigGrass");
            mBackgroundOne.Position = new Vector2(0, 0);

            mBackgroundTwo.LoadContent(this.Content, "Grass");
            mBackgroundTwo.Position = new Vector2(mBackgroundOne.Position.X + mBackgroundOne.Size.Width, 0);

            mBackgroundThree.LoadContent(this.Content, "Grass2");
            mBackgroundThree.Position = new Vector2(mBackgroundTwo.Position.X + mBackgroundTwo.Size.Width, 0);

            mBackgroundFour.LoadContent(this.Content, "Grass3");
            mBackgroundFour.Position = new Vector2(mBackgroundThree.Position.X + mBackgroundThree.Size.Width, 0);

            mBackgroundFive.LoadContent(this.Content, "Grass4");
            mBackgroundFive.Position = new Vector2(mBackgroundFour.Position.X + mBackgroundFour.Size.Width, 0);

            background = Content.Load<Texture2D>("Sky");

            Animation playerAnimation = new Animation();
            playerTexture = Content.Load<Texture2D>("BUG");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 200, 146, 8, 30, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width /8, GraphicsDevice.Viewport.TitleSafeArea.Y
            + GraphicsDevice.Viewport.TitleSafeArea.Height /2);
            player.Initialize(playerAnimation, playerPosition);

            enemyTexture = Content.Load<Texture2D>("FLOWER2");

            projectileTexture = Content.Load<Texture2D>("sting");

            explosionTexture = Content.Load<Texture2D>("bloom");

            // Load the music
            gameplayMusic = Content.Load<Song>("Audio/MusicGamePlay");

            bloomSound = Content.Load<SoundEffect>("Audio/burst");

            // Start the music right away
            PlayMusic(gameplayMusic);

            // Load the score font
            font = Content.Load<SpriteFont>("Fonts/gameFont");

            startScreen = Content.Load<Texture2D>("MainMenu");

            endScreen = Content.Load<Texture2D>("endMenu");
        }

        private void PlayMusic(Song song)
        {
            try
            {
                // Play the music
                MediaPlayer.Play(song);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();

            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 80, 56, 8, 30, Color.White, 1f, true);

            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Add the enemy to the active enemies list
            enemies.Add(enemy);
        }


        private void UpdateProjectiles()
        {
            // Update the Projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();

                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 65, 53, 13, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }

        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
            projectiles.Add(projectile);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddEnemy();
            }

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                if (enemies[i].Active == false)
                {
                    // If not active and health <= 0
                    if (enemies[i].Health <= 0)
                    {
                        // Add an explosion
                        AddExplosion(enemies[i].Position);
                        bloomSound.Play();
                        //Add to the player's score
                        score += enemies[i].Value;
                    }
                    enemies.RemoveAt(i);
                }
            }


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

            if (currentGameState == GameState.Start || currentGameState == GameState.GameOver)
            {
                updateSplashScreen();
            }

            else
            {



                // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
                previousGamePadState = currentGamePadState;
                previousKeyboardState = currentKeyboardState;

                // Read the current state of the keyboard and gamepad and store it
                currentKeyboardState = Keyboard.GetState();
                currentGamePadState = GamePad.GetState(PlayerIndex.One);


                //Update the player
                UpdatePlayer(gameTime);


                if (mBackgroundOne.Position.X < -mBackgroundOne.Size.Width)
                {
                    mBackgroundOne.Position.X = mBackgroundFive.Position.X + mBackgroundFive.Size.Width;
                }

                if (mBackgroundTwo.Position.X < -mBackgroundTwo.Size.Width)
                {
                    mBackgroundTwo.Position.X = mBackgroundOne.Position.X + mBackgroundOne.Size.Width;
                }

                if (mBackgroundThree.Position.X < -mBackgroundThree.Size.Width)
                {
                    mBackgroundThree.Position.X = mBackgroundTwo.Position.X + mBackgroundTwo.Size.Width;
                }

                if (mBackgroundFour.Position.X < -mBackgroundFour.Size.Width)
                {
                    mBackgroundFour.Position.X = mBackgroundThree.Position.X + mBackgroundThree.Size.Width;
                }

                if (mBackgroundFive.Position.X < -mBackgroundFive.Size.Width)
                {
                    mBackgroundFive.Position.X = mBackgroundFour.Position.X + mBackgroundFour.Size.Width;
                }

                Vector2 aDirection = new Vector2(-1, 0);
                Vector2 aSpeed = new Vector2(160, 0);

                mBackgroundOne.Position += aDirection * aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                mBackgroundTwo.Position += aDirection * aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                mBackgroundThree.Position += aDirection * aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                mBackgroundFour.Position += aDirection * aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                mBackgroundFive.Position += aDirection * aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Update the enemies
                UpdateEnemies(gameTime);

                // Update the collision
                UpdateCollision();

                // Update the projectiles
                UpdateProjectiles();

                // Update the explosions
                UpdateExplosions(gameTime);

                base.Update(gameTime);
            }
        }

        private void updateSplashScreen()
        {
            while (TouchPanel.IsGestureAvailable)
            {
                if (TouchPanel.ReadGesture().GestureType == GestureType.Hold)
                {
                    currentGameState = GameState.InGame;
                }
            }
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }


        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect function to 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X,
            (int)player.Position.Y ,
            player.Width / 2,
            player.Height /4);

            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].Position.X,
                (int)enemies[i].Position.Y,
                enemies[i].Width,
                enemies[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    player.Health -= enemies[i].Damage;

                    // Since the enemy collided with the player
                    // destroy it
                    enemies[i].Health = 0;

                    // If the player health is less than zero we died
                    if (player.Health <= 0)
                        player.Active = false;
                }

            }

            // Projectile vs Enemy Collision
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                    enemies[j].Width, enemies[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }


        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            if (currentGameState == GameState.Start)
            {
                DrawStartScreen();
            }
            else if (currentGameState == GameState.InGame)
            {
                // TODO: Add your drawing code here
                spriteBatch.Begin();
                spriteBatch.Draw(background, Vector2.Zero, Color.White);
                mBackgroundOne.Draw(this.spriteBatch);
                mBackgroundTwo.Draw(this.spriteBatch);
                mBackgroundThree.Draw(this.spriteBatch);
                mBackgroundFour.Draw(this.spriteBatch);
                mBackgroundFive.Draw(this.spriteBatch);
                // Draw the Player
                player.Draw(spriteBatch);
                // Draw the Enemies
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Draw(spriteBatch);
                }

                // Draw the Projectiles
                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles[i].Draw(spriteBatch);
                }

                // Draw the explosions
                for (int i = 0; i < explosions.Count; i++)
                {
                    explosions[i].Draw(spriteBatch);
                }

                // Draw the score
                spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                    GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
                // Draw the player health
                spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                    GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);

                spriteBatch.End();
            }
            else if (currentGameState == GameState.GameOver)
            {
                OnExiting();
            }

            base.Draw(gameTime);
        }

        private void OnExiting()
        {
            drawEndScreen();
        }

        private void DrawStartScreen()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(startScreen, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        private void drawEndScreen()
        {
            GraphicsDevice.Clear(Color.AliceBlue);

            spriteBatch.Begin();
            
            string gameover = "Game Over!";

            spriteBatch.Draw(endScreen, Vector2.Zero, Color.White);

            gameover = "Your score: " + score;
            spriteBatch.DrawString(font, gameover,
                new Vector2((GraphicsDevice.Viewport.Width / 2)
                - (font.MeasureString(gameover).X / 2),
                (GraphicsDevice.Viewport.Height / 2)
                - (font.MeasureString(gameover).Y / 2) + 30),
                Color.SaddleBrown);

            gameover = "(Press back button to exit)";
            spriteBatch.DrawString(font, gameover,
                new Vector2((GraphicsDevice.Viewport.Width / 2)
                - (font.MeasureString(gameover).X / 2),
                (GraphicsDevice.Viewport.Height / 2)
                - (font.MeasureString(gameover).Y / 2) + 60),
                Color.SaddleBrown);

            spriteBatch.End();

            

        }
    }
}
