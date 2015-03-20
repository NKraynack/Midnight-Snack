using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CS_4730_Prototype
{
    public class InfoText : Text
    {
        private bool visible;

        public InfoText(string msg, Vector2 pos) : base(msg, pos)
        {
            visible = true;
        }

        public void Draw(SpriteBatch sb)
        {
            if (visible)
            {
                //draws a string, params are your font, your message, position, and color
                sb.DrawString(font, message, position, Color.White);
            }
        }

        public void SetVisible(bool b)
        {
            visible = b;
        }
    }
}
