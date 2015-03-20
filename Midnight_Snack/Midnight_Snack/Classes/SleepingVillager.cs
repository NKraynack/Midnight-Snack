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
    public class SleepingVillager : Unit
    {
        private bool drained;

        public SleepingVillager(Vector2 pos, int width, int height, int row, int col) : base(pos, width, height, row, col)
        {
            drained = false;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("mario.png");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (drained)
            {
                spriteBatch.Draw(texture, position, Color.Gray);
            }
            else
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
        }

        public void SetDrained(bool b)
        {
            drained = b;
        }

    }
}
