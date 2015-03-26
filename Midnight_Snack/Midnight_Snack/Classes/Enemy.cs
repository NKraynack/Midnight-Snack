using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Map map;
        Player player;
        public Enemy(Vector2 pos, int width, int height, int row, int col, int range, int health, Player p, Map m) 
            : base(pos, width, height, row, col, range, health)
        {
            player = p;
            map = m;
        }

        public override void LoadContent(ContentManager content)
        {
            //temp until I either draw one or find one
            texture = content.Load<Texture2D>("goomba.png");
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
            //Debug.WriteLine("Check check check");

            //If it's this enemy's turn, have it move and use an ability (if possible)
            if(unitsTurn)
            {
                //For testing purposes enemy just moves one tile to the left each turn
                
                if (this.AdjacentToPlayer()) {
                    //insert attack method
                    //player.SetAlive(false);
                }
                
                //End enemy's turn
                hasEndedTurn = true;
            }
        }

        public bool AdjacentToPlayer()
        {
            
            if (player.GetCol() == this.GetCol() - 1 && player.GetRow() == this.GetRow())
            {
                //Debug.WriteLine("Enemy next to player");
                return true;
            }
            else if (player.GetCol() == this.GetCol() + 1 && player.GetRow() == this.GetRow())
            {
                //Debug.WriteLine("Enemy next to player");
                return true;
            }
            else if (player.GetCol() == this.GetCol() && player.GetRow() == this.GetRow() - 1)
            {
                //Debug.WriteLine("Enemy next to player");
                return true;
            }
            else if (player.GetCol() == this.GetCol() && player.GetRow() == this.GetRow() + 1)
            {
                //Debug.WriteLine("Enemy next to player");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
