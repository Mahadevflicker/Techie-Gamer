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
    class SpriteTexture
    {
        //The current position of the Sprite
        public Vector2 Position = new Vector2(0, 0);

        //The size of the Sprite
        public Rectangle Size;

        //Used to size the Sprite up or down from the original image
        public float Scale = 1.0f;

        //The texture object used when drawing the sprite
        private Texture2D mSpriteTexture;

        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            mSpriteTexture = theContentManager.Load<Texture2D>(theAssetName);
            Size = new Rectangle(0, 0, (int)(mSpriteTexture.Width * Scale), (int)(mSpriteTexture.Height * Scale));
        }

        //Draw the sprite to the screen
        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(mSpriteTexture, Position,
                new Rectangle(0, 0, mSpriteTexture.Width, mSpriteTexture.Height), Color.White,
                0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

    }
}
