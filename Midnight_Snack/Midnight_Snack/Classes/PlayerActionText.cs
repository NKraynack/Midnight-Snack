using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Midnight_Snack
{
    public class PlayerActionText : Text
    {
        private bool visible;
        private bool available;
        private int xOffset;
        private int yOffset;

        public PlayerActionText(string msg, Vector2 pos, int xOffset, int yOffset) : base(msg, pos)
        {
            visible = true;
            available = true;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }

        public void Update(Vector2 playerPos)
        {
            position = playerPos;
            position.X += 70 + xOffset;
            position.Y += 15 + yOffset;
        }

        public void Draw(SpriteBatch sb)
        {
            if (visible)
            {
                if (available)
                {
                    //draws a string, params are your font, your message, position, and color
                    sb.DrawString(font, message, position, Color.White);
                }
                //Gray out unavailable actions
                else
                {
                    sb.DrawString(font, message, position, Color.Gray);
                }
            }
            
        }

        public void SetVisible(bool b)
        {
            visible = b;
        }

        public void SetAvailable(bool b)
        {
            available = b;
        }
    }
}
