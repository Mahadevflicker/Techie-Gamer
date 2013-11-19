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


namespace Mini_Project
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class Enemy
    {
        // Animation representing the enemy
        public Animation EnemyAnimation;

        // The position of the enemy flower relative to the top left corner of thescreen
        public Vector2 Position;

        // The state of the enemy flower
        public bool Active;

        // The hit points of the enemy, if this goes to zero the enemy dies
        public int Health;

        // The amount of damage the enemy inflicts on the Bee
        public int Damage;

        // The amount of score the enemy will give to the player
        public int Value;

        // Get the width of the enemy flower
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }

        // Get the height of the enemy flower
        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }

        // The speed at which the enemy moves
        float enemyMoveSpeed;

        public void Initialize(Animation animation, Vector2 position)
        {
            // Load the enemy flower texture
            EnemyAnimation = animation;

            // Set the position of the enemy
            Position = position;

            // We initialize the enemy to be active so it will be update in the game
            Active = true;


            // Set the health of the enemy
            Health = 10;

            // Set the amount of damage the enemy can do
            Damage = 10;

            // Set how fast the enemy moves
            enemyMoveSpeed = 6f;


            // Set the score value of the enemy
            Value = 100;

        }

        public void Update(GameTime gameTime)
        {
            // The enemy always moves to the left so decrement it's xposition
            Position.X -= enemyMoveSpeed;

            // Update the position of the Animation
            EnemyAnimation.Position = Position;

            // Update Animation
            EnemyAnimation.Update(gameTime);

            // If the enemy is past the screen or its health reaches 0 then deactivateit
            if (Position.X < -Width || Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this objet fromthe 
                // active game list
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            EnemyAnimation.Draw(spriteBatch);
        }

    }

}