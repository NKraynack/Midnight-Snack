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
    public class Obstacle : Unit
    {
        private string obstacleType;

        public Obstacle(Vector2 pos, int width, int height, int row, int col, string type) : base(pos, width, height, row, col)
        {
            this.obstacleType = type;
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>(obstacleType);
        }
    }
}
