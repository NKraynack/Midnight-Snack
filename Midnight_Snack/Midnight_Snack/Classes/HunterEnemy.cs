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
    class HunterEnemy : Enemy
    {
        Player player = Player.GetInstance();
        Map map = Map.GetInstance();

        public HunterEnemy(Vector2 pos, int width, int height, int row, int col, int range, int health)
            : base(pos, width, height, row, col, range, health)
        {
            this.map_grid = map.GenerateMapGrid();

            //If health is greater than 5, this hunter is a master hunter
            if(maxHealth > 5)
            {
                strength = 4;
            }
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("vampire_hunter");
            //If health is greater than 5, this hunter is a master hunter
            if(maxHealth > 5)
            {
                texture = content.Load<Texture2D>("master_vampire_hunter");
            }
            healthBar.LoadContent(content);
            attackStr.LoadContent(content);
        }

        public override void Update()
        {
            healthBar.Update(position, currentHealth);
            attackStr.SetPosition(new Vector2(position.X, position.Y + 75));

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

                    //Handle enemy movement
                    int[] destCoords = GetDestination();
                    MapTile dest = map.GetTile(destCoords[0], destCoords[1]);
                    EnemyMove(destCoords[0], destCoords[1], dest);

                    //Use ability only onces per turn regardless of move
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

        //Determine where to move
        //Returns an int array containing [destRow, destCol]
        public override int[] GetDestination()
        {
            //Destination defaults to current position
            int[] destination = { this.GetRow(), this.GetCol() };

            //Subclasses should override this method and calculate dest here

            destination[0] = player.GetRow();
            destination[1] = player.GetCol();

            return destination;
        }

        public override void UseAbilities()
        {
            //Attack the player if adjacent
            if (this.AdjacentToPlayer() && !this.HasUsedAbilityThisTurn())
            {
                Attack(player);
            }
        }

    }
}
