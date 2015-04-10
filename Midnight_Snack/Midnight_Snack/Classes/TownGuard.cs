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
    public class TownGuard : Enemy
    {
        Player player = Player.GetInstance();
        Map map = Map.GetInstance();
        int[] dests;
        
        public TownGuard(Vector2 pos, int width, int height, int row, int col, int range, int health, int[] destList)
            : base(pos, width, height, row, col, range, health)
        {
            this.map_grid = map.GenerateMapGrid();
            this.dests = destList;
            
        }

        public override void LoadContent(ContentManager content)
        {
            //temp until I either draw one or find one
            texture = content.Load<Texture2D>("town_guard");
            healthBar.LoadContent(content);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (alive)
            {
                base.Draw(spriteBatch);

                spriteBatch.Draw(texture, position, Color.White);
            }
        }

        public override void Update()
        {
            base.Update();

            if (!alive)
            {
                //Remove enemy from play
                map.GetTile(this.GetRow(), this.GetCol()).SetOccupant(null);
                //End enemy's turn
                hasEndedTurn = true;
            }
            else
            {
                //If it's this enemy's turn, have it move and use an ability (if possible)
                if (unitsTurn)
                {
                    Debug.WriteLine("Enemy Turn");

                    //Use any relevant abilities
                    this.UseAbilities();

                    //Handle enemy movement
                    int[] destCoords = GetDestination();
                    
                    Debug.WriteLine("Before move: Row: " + this.GetRow() + " Col: " + this.GetCol());
                    Debug.WriteLine("Row: " + destCoords[0] + " " + "Col: " + destCoords[1]);
                    MapTile dest = map.GetTile(destCoords[0], destCoords[1]);
                    int[] preCoords = { this.GetRow(), this.GetCol() };
                    EnemyMove(destCoords[0], destCoords[1], dest);
                    
                    /*
                    if (preCoords[0] != this.GetRow() && preCoords[1] != this.GetCol())
                    {
                        Debug.WriteLine("before the move" + step);
                        step = (step + 2) % (dests.Length);
                        Debug.WriteLine("after the move" + step);
                    }
                    */
                    //If did not use any abilities before moving, try now
                    this.UseAbilities();

                    //End enemy's turn
                    hasEndedTurn = true;
                }
            }

        }

        public override int[] GetDestination()
        {
            int[] ret_val = { this.GetRow(), this.GetCol() };
            return ret_val;
        }

        public override void EnemyMove(int destRow, int destCol, MapTile dest)
        {
            //no need for bounds checks if it's a set path anyway
                    
            Vector2 dest_vector = map.GetTile(destRow, destCol).GetPosition();

            Console.WriteLine("moving " + dest_vector.X + ":" + dest_vector.Y + " " + destRow + " " + destCol);
            Move(dest_vector, destRow, destCol);
        }
    }
}
