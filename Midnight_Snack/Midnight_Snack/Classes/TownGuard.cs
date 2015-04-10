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
        int[] currentDest;
        
        public TownGuard(Vector2 pos, int width, int height, int row, int col, int range, int health, int[] destList)
            : base(pos, width, height, row, col, range, health)
        {
            this.map_grid = map.GenerateMapGrid();
            this.dests = destList;
            currentDest = new int[]{dests[2], dests[3]};
            
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("town_guard");
            healthBar.LoadContent(content);
        }

        public override void Update()
        {
            healthBar.Update(position, currentHealth);
            if (currentHealth <= 0)
            {
                alive = false;
            }

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
                    MapTile dest = map.GetTile(destCoords[0], destCoords[1]);
                    EnemyMove(destCoords[0], destCoords[1], dest);
                    
                    //If did not use any abilities before moving, try now
                    this.UseAbilities();

                    //End enemy's turn
                    hasEndedTurn = true;
                }
                else
                {
                    //Reset enemy options
                    SetMovedThisTurn(false);
                    SetUsedAbilityThisTurn(false);
                }
            }

        }

        public override int[] GetDestination()
        {
            if (this.GetRow() == dests[0] && this.GetCol() == dests[1])
            {
                currentDest[0] = dests[2];
                currentDest[1] = dests[3];
            }
            else if (this.GetRow() == dests[2] && this.GetCol() == dests[3])
            {
                currentDest[0] = dests[0];
                currentDest[1] = dests[1];
            }
            Debug.WriteLine("Destination: (" + currentDest[0] + ", " + currentDest[1] + ")");

            //For right now, just make guard immobile
            currentDest[0] = this.GetRow();
            currentDest[1] = this.GetCol();

            return currentDest;
        }
    }
}
