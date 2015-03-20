using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS_4730_Prototype
{
    public class GameObject
    {
        protected Vector2 Position;
        protected Texture2D Texture;
        protected int ObjectWidth;
        protected int ObjectHeight;

        public GameObject(int x, int y, int width, int height)
        {
            Vector2 pos = new Vector2(x, y);
            this.Position = pos;
            this.ObjectWidth = width;
            this.ObjectHeight = height;
        }

        public GameObject(Vector2 pos, int width, int height)
        {
            this.Position = pos;
            this.ObjectWidth = width;
            this.ObjectHeight = height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

        //Move the GameObject by the given amount
        public virtual void Move(Vector2 amount)
        {
            Position += amount;
        }

        public int GetX()
        {
            return (int)Position.X;
        }

        public int GetY()
        {
            return (int)Position.Y;
        }

        public Vector2 GetPosition()
        {
            return Position;
        }

        public void SetX(int x)
        {
            Position.X = x;
        }

        public void SetY(int y)
        {
            Position.Y = y;
        }

        public void SetPosition(Vector2 pos)
        {
            Position = pos;
        }
    }
}
