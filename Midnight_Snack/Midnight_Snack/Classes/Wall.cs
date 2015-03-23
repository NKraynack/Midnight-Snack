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
    public class Wall : Unit
    {
        private string formation;

        public Wall(Vector2 pos, int width, int height, int row, int col, string formation) : base(pos, width, height, row, col)
        {
            this.formation = formation;
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("unpassable_map_tile.png");
        }
    }
}
