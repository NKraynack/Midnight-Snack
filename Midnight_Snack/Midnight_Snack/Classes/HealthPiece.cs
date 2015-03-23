using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Midnight_Snack
{
    public class HealthPiece : GameObject
    {
        private bool empty;
        private Texture2D fullTexture;
        private Texture2D emptyTexture;

        public HealthPiece(Vector2 pos, int width, int height) : base(pos, width, height)
        {
            empty = false;
        }

        public bool IsEmpty()
        {
            return empty;
        }

        public void SetEmpty(bool b)
        {
            empty = b;
        }

        public override void LoadContent(ContentManager content)
        {
            emptyTexture = content.Load<Texture2D>("empty_health_pip.png");
            fullTexture = content.Load<Texture2D>("health_pip.png");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (empty)
            {
                spriteBatch.Draw(emptyTexture, position, Color.White);
            }
            else
            {
                spriteBatch.Draw(fullTexture, position, Color.White);
            }
        }
    }
}
