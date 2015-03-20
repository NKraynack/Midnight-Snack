using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Midnight_Snack
{
    public class Text
    {
        protected SpriteFont font;
        protected string message;
        protected Vector2 position;
        private bool visible;

        public Text(string msg, Vector2 pos)
        {
            message = msg;
            position = pos;
            visible = true;
        }

        public void LoadContent(ContentManager content)
        {
            //loads font
            font = content.Load<SpriteFont>("Arial");
        }

        public void Draw(SpriteBatch sb)
        {
            if (visible)
            {
                //draws a string, params are your font, your message, position, and color
                sb.DrawString(font, message, position, Color.White);
            }
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void SetPosition(Vector2 pos)
        {
            position = pos;
        }

        public string GetMessage()
        {
            return message;
        }

        public void SetMessage(string text)
        {
            message = text;
        }

        public bool IsVisible()
        {
            return visible;
        }

        public void SetVisible(bool b)
        {
            visible = b;
        }

    }
}
