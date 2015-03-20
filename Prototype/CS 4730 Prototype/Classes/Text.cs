using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CS_4730_Prototype
{
    public class Text
    {
        protected SpriteFont font;
        protected string message;
        protected Vector2 position;

        public Text(string msg, Vector2 pos)
        {
            message = msg;
            position = pos;
        }

        public void LoadContent(ContentManager content)
        {
            //loads font
            font = content.Load<SpriteFont>("Arial");
        }

        public void Draw(SpriteBatch sb)
        {
            //draws a string, params are your font, your message, position, and color
            sb.DrawString(font, message, position, Color.White);
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void SetPosition(Vector2 pos)
        {
            position = pos;
        }

        public void SetText(string text)
        {
            message = text;
        }

    }
}
