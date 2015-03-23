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
    class Enemy : MobileUnit 
    {
        
        public Enemy(Vector2 pos, int width, int height, int row, int col, int range, int health) 
            : base(pos, width, height, row, col, range, health)
        {
            
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);   
        }
    }
}
