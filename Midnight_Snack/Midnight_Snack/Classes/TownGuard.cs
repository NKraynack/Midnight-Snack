using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Midnight_Snack.Classes
{
    class TownGuard : Enemy
    {
        Player player = Player.GetInstance();
        private char[,] map_grid; //The grid for the map to generate shortest path
        public TownGuard(Vector2 pos, int width, int height, int row, int col, int range, int health, Map map)
            : base(pos, width, height, row, col, range, health, map)
        {
            this.map_grid = map.GenerateMapGrid();
        }

        public override void LoadContent(ContentManager content)
        {
            //temp until I either draw one or find one
            texture = content.Load<Texture2D>("goomba.png");
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

                    //Attack the player if adjacent
                    if (this.AdjacentToPlayer())
                    {
                        Debug.WriteLine("Attacking player!");
                        //insert attack method
                        Attack(player);
                    }

                    //enemy movement
                    MapTile dest = map.GetTile(this.GetRow(), this.GetCol() - 2);
                    EnemyMove(dest.GetPosition(), this.GetRow(), this.GetCol() - 2, dest);

                    //End enemy's turn
                    hasEndedTurn = true;
                }
            }

        }

        public override void EnemyMove(Vector2 pos, int row, int col, MapTile dest)
        {
            if (col - 2 > 0)
            {
                if (this.NoObstacles(this.GetCol() - 2, this.GetRow()) && dest.IsPassable())
                {
                    Move(pos, row, col);
                }
            }
        }
    }
}
