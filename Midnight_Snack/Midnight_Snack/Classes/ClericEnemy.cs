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
    class ClericEnemy : Enemy
    {
        Player player = Player.GetInstance();

        public ClericEnemy(Vector2 pos, int width, int height, int row, int col, int range, int health, Map map)
            : base(pos, width, height, row, col, range, health, map)
        {
            this.map_grid = map.GenerateMapGrid();
        }

        public override void LoadContent(ContentManager content)
        {
            //temp until I either draw one or find one
            texture = content.Load<Texture2D>("cleric");
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
                    Debug.WriteLine("Cleric Enemy Turn");

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

        //Randomly makes consecrated ground nearby or on player
        public override void UseAbilities()
        {
            int player_row = player.GetRow();
            int player_col = player.GetCol();
            Random rnd = new Random();
            //Used to get a random tile on or around player
            //flag to tell it to generate a new seed
            bool valid_seed = false;
            //flags to tell where not to look
            bool left_no = false;
            bool right_no = false;
            bool top_no = false;
            bool bottom_no = false;

            if (player_row - 1 < 0)
            {
                top_no = true;
            } 
            if (player_row + 1 >= map.GetNumRows())
            {
                bottom_no = true;
            }
            if (player_col - 1 < 0)
            {
                left_no = true;
            }
            if (player_col + 1 >= map.GetNumCols())
            {
                right_no = true;
            }
            int seed = rnd.Next(0, 8);
            do
            {
                seed = rnd.Next(0, 8);

                MapTile obstacle;
                switch (seed)
                {
                    case 0: //player
                        obstacle = map.GetTile(player_row, player_col);
                        obstacle.SetModifier("consecrated");
                        map.SetTile(player_row, player_col, obstacle);
                        valid_seed = true;
                        break;
                    case 1: //right
                        if (!right_no) 
                        {
                            obstacle = map.GetTile(player_row, player_col + 1);
                            obstacle.SetModifier("consecrated");
                            map.SetTile(player_row, player_col + 1, obstacle);
                            valid_seed = true;
                        }
                        break;
                    case 2: //bottom right
                        if (!right_no && !bottom_no)
                        {
                            obstacle = map.GetTile(player_row + 1, player_col + 1);
                            obstacle.SetModifier("consecrated");
                            map.SetTile(player_row + 1, player_col + 1, obstacle);
                            valid_seed = true;
                        }
                        break;
                    case 3: //bottom
                        if (!bottom_no)
                        {
                            obstacle = map.GetTile(player_row + 1, player_col);
                            obstacle.SetModifier("consecrated");
                            map.SetTile(player_row + 1, player_col, obstacle);
                            valid_seed = true;
                        }
                        break;
                    case 4: //bottom left
                        if (!left_no && !bottom_no)
                        {
                            obstacle = map.GetTile(player_row + 1, player_col - 1);
                            obstacle.SetModifier("consecrated");
                            map.SetTile(player_row + 1, player_col - 1, obstacle);
                            valid_seed = true;
                        }
                        break;
                    case 5: //left
                        if (!left_no)
                        {
                            obstacle = map.GetTile(player_row, player_col - 1);
                            obstacle.SetModifier("consecrated");
                            map.SetTile(player_row, player_col - 1, obstacle);
                            valid_seed = true;
                        }
                        break;
                    case 6: //top left
                        if (!top_no && !left_no)
                        {
                            obstacle = map.GetTile(player_row - 1, player_col - 1);
                            obstacle.SetModifier("consecrated");
                            map.SetTile(player_row - 1, player_col - 1, obstacle);
                            valid_seed = true;
                        }
                        break;
                    case 7: //top
                        if (!top_no)
                        {
                            obstacle = map.GetTile(player_row - 1, player_col);
                            obstacle.SetModifier("consecrated");
                            map.SetTile(player_row - 1, player_col, obstacle);
                            valid_seed = true;
                        }
                        break;
                    case 8: //top right
                        if (!top_no && !right_no)
                        {
                            obstacle = map.GetTile(player_row - 1, player_col + 1);
                            obstacle.SetModifier("consecrated");
                            map.SetTile(player_row - 1, player_col + 1, obstacle);
                            valid_seed = true;
                        }
                        break;
                }

            } while (!valid_seed);
        }

        //Determine where to move
        //Returns an int array containing [destRow, destCol]
        public override int[] GetDestination()
        {
            //Destination defaults to current position
            int[] destination = { this.GetRow(), this.GetCol() };

            //Subclasses should override this method and calculate dest here
            Enemy closestEnemy = GetClosestEnemy();
            if (closestEnemy != null)
            {
                destination[0] = closestEnemy.GetRow();
                destination[1] = closestEnemy.GetCol();
            }
            else
            {
                //Don't move
                destination[0] = this.GetRow();
                destination[1] = this.GetCol();
            }

            return destination;
        }

        //Returns the undrained villager at the given tile
        //If there is not an undrained villager at that tile, returns null
        public Enemy GetEnemy(int row, int col)
        {
            if (row > -1 && col > -1 && row < map.GetNumRows() && col < map.GetNumCols())
            {
                if (map.GetTile(row, col).GetOccupant() != null)
                {
                    if (map.GetTile(row, col).GetOccupant().GetType().IsSubclassOf(typeof(Enemy)))
                    {
                        return (Enemy)map.GetTile(row, col).GetOccupant();
                    }
                }
            }

            return null;
        }

        //Returns the closest undrained villager to the vampire's location
        //If there are no enemies, returns null
        public Enemy GetClosestEnemy()
        {
            int row = this.GetRow();
            int col = this.GetCol();

            //Looks for villagers in a widening counterclockwise spiral around vampire
            for (int i = 1; i < Math.Max(map.GetNumRows(), map.GetNumCols()); i++)
            {
                //left
                col = this.GetCol() - i;
                //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                if (GetEnemy(row, col) != null)
                {
                    return GetEnemy(row, col);
                }
                //bottom left
                for (int j = 0; j < i; j++)
                {
                    row += 1;
                    //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                    if (GetEnemy(row, col) != null)
                    {
                        return GetEnemy(row, col);
                    }
                }
                //bottom
                for (int j = 0; j < i * 2; j++)
                {
                    col += 1;
                    //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                    if (GetEnemy(row, col) != null)
                    {
                        return GetEnemy(row, col);
                    }
                }
                //right
                for (int j = 0; j < i * 2; j++)
                {
                    row -= 1;
                    //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                    if (GetEnemy(row, col) != null)
                    {
                        return GetEnemy(row, col);
                    }
                }
                //top
                for (int j = 0; j < i * 2; j++)
                {
                    col -= 1;
                    //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                    if (GetEnemy(row, col) != null)
                    {
                        return GetEnemy(row, col);
                    }
                }
                //top left
                for (int j = 0; j < i; j++)
                {
                    row += 1;
                    //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                    if (GetEnemy(row, col) != null)
                    {
                        return GetEnemy(row, col);
                    }
                }
            }

            return null;
        }
    }
}
