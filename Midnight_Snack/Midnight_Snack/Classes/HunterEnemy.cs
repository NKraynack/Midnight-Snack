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

        public HunterEnemy(Vector2 pos, int width, int height, int row, int col, int range, int health, Map map)
            : base(pos, width, height, row, col, range, health, map)
        {
            this.map_grid = map.GenerateMapGrid();
        }

        public override void LoadContent(ContentManager content)
        {
            //temporary image
            texture = content.Load<Texture2D>("dracula");
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
            Player closestPlayer = GetClosestPlayer();
            if (closestPlayer != null)
            {
                destination[0] = closestPlayer.GetRow();
                destination[1] = closestPlayer.GetCol();
            }
            else
            {
                destination[0] = player.GetRow();
                destination[1] = player.GetCol();
            }

            return destination;
        }

        //Returns the player at a given tile
        public Player GetPlayer(int row, int col)
        {
            if (row > -1 && col > -1 && row < map.GetNumRows() && col < map.GetNumCols())
            {
                if (map.GetTile(row, col).GetOccupant() != null)
                {
                    if (map.GetTile(row, col).GetOccupant().GetType() == typeof(Player))
                    {
                        //Check if player is alive
                        if (((Player)map.GetTile(row, col).GetOccupant()).IsAlive())
                        {
                            return (Player)map.GetTile(row, col).GetOccupant();
                        }
                    }
                }
            }

            return null;
        }


        //Returns the closest player character to the vampire's location
        public Player GetClosestPlayer()
        {
            int row = this.GetRow();
            int col = this.GetCol();

            for (int i = 1; i < Math.Max(map.GetNumRows(), map.GetNumCols()); i++)
            {
                //left
                col = this.GetCol() - i;
                if (GetPlayer(row, col) != null)
                {
                    return GetPlayer(row, col);
                }
                //bottom left
                for (int j = 0; j < i; j++)
                {
                    row += 1;
                    if (GetPlayer(row, col) != null)
                    {
                        return GetPlayer(row, col);
                    }
                }
                //bottom
                for (int j = 0; j < i * 2; j++)
                {
                    col += 1;
                    if (GetPlayer(row, col) != null)
                    {
                        return GetPlayer(row, col);
                    }
                }
                //right
                for (int j = 0; j < i * 2; j++)
                {
                    row -= 1;
                    if (GetPlayer(row, col) != null)
                    {
                        return GetPlayer(row, col);
                    }
                }
                //top
                for (int j = 0; j < i * 2; j++)
                {
                    col -= 1;
                    if (GetPlayer(row, col) != null)
                    {
                        return GetPlayer(row, col);
                    }
                }
                //top left
                for (int j = 0; j < i; j++)
                {
                    row += 1;
                    if (GetPlayer(row, col) != null)
                    {
                        return GetPlayer(row, col);
                    }
                }
            }

            return null;
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
