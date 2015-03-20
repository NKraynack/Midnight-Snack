using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Midnight_Snack
{
    public class GameObject
    {
        protected Vector2 position;
        protected Texture2D texture;
        protected int width;
        protected int height;

        public GameObject(int x, int y, int width, int height)
        {
            Vector2 pos = new Vector2(x, y);
            this.position = pos;
            this.width = width;
            this.height = height;
        }

        public GameObject(Vector2 pos, int width, int height)
        {
            this.position = pos;
            this.width = width;
            this.height = height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        //Move the GameObject by the given amount
        public virtual void MoveByAmount(Vector2 amount)
        {
            position += amount;
        }

        public int GetX()
        {
            return (int)position.X;
        }

        public int GetY()
        {
            return (int)position.Y;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void SetX(int x)
        {
            position.X = x;
        }

        public void SetY(int y)
        {
            position.Y = y;
        }

        public void SetPosition(Vector2 pos)
        {
            position = pos;
        }
    }
}
