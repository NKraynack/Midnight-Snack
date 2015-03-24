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
    public class Enemy : MobileUnit 
    {
        
        public Enemy(Vector2 pos, int width, int height, int row, int col, int range, int health) 
            : base(pos, width, height, row, col, range, health)
        {
            
        }

        public override void LoadContent(ContentManager content)
        {
            //temp until I either draw one or find one
            texture = content.Load<Texture2D>("dracula.png");
            healthBar.LoadContent(content);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(texture, position, Color.White);   
        }

        public override void Update()
        {
            base.Update();

            if (!alive)
            {
                //Remove enemy from play
            }

            //If it's this enemy's turn, have it move and use an ability (if possible)
            if(unitsTurn)
            {
                //For testing purposes enemy just moves one tile to the left each turn


                //End enemy's turn
                hasEndedTurn = true;
            }
        }
    }
}
