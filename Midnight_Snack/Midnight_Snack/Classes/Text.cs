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
        private bool available;

        public Text(string msg, Vector2 pos)
        {
            message = msg;
            position = pos;
            visible = true;
            available = true;
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

        public bool IsAvailable()
        {
            return available;
        }

        public void SetAvailable(bool b)
        {
            available = b;
        }

    }
}
