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
        Map map = Map.GetInstance();
        MapTile consec_tile;
        int consec_row;
        int consec_col;

        public ClericEnemy(Vector2 pos, int width, int height, int row, int col, int range, int health)
            : base(pos, width, height, row, col, range, health)
        {
            this.map_grid = map.GenerateMapGrid();
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("cleric");
            healthBar.LoadContent(content);
            attackStr.LoadContent(content);
        }

        public override void Update()
        {
            healthBar.Update(position, currentHealth);
            attackStr.SetPosition(new Vector2(position.X, position.Y + 75));
            //Don't display attack strength on clerics since they can't attack
            attackStr.SetVisible(false);

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
                //Remove consecrated ground
                if (consec_tile != null)
                {
                    consec_tile.SetModifier("basic");
                    map.SetTile(consec_row, consec_col, consec_tile);
                }
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

                    //Use ability only once per turn regardless of move
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
        
        public override void UseAbilities()
        {
            Debug.WriteLine("Heal Target: " + this.GetAdjacentDamagedEnemy());
            //Heal an adjacent damaged enemy if there is one
            if (this.GetAdjacentDamagedEnemy() != null && !this.HasUsedAbilityThisTurn())
            {
                Debug.WriteLine("Cleric Healing");
                Heal(this.GetAdjacentDamagedEnemy());
            }
            //If adjacent to player, place garlic on player's tile
            if (this.AdjacentToPlayer() && !this.HasUsedAbilityThisTurn())
            {
                PlaceGarlic(player.GetRow(), player.GetCol());
            }
            //Otherwise just use consecrate
            if (!this.HasUsedAbilityThisTurn())
            {
                Consecrate();
            }
        }

        //Heals the target by the cleric's strength amount
        public void Heal(Enemy target)
        {
            //Target must still be alive
            if (target.IsAlive())
            {
                //Update the target's health
                int targetHealth = target.GetCurrentHealth() + strength;
                if(targetHealth > target.GetMaxHealth())
                {
                    targetHealth = target.GetMaxHealth();
                }
                target.SetCurrentHealth(targetHealth);
                //Updated map tile of target
                MapTile tile = map.GetTile(target.GetRow(), target.GetCol());
                tile.SetOccupant(target);
                //Update that unit has used an ability this turn
                this.SetUsedAbilityThisTurn(true);
            }
        }

        //Places garlic on the given location
        public void PlaceGarlic(int row, int col)
        {
            //Target location must be adjacent to cleric
            if (Math.Abs(this.GetRow() - row) <= 1 && Math.Abs(this.GetCol() - col) <= 1)
            {
                map = Map.GetInstance();
                MapTile garlicTile = map.GetTile(row, col);
                garlicTile.SetModifier("garlic");
                map.SetTile(row, col, garlicTile);
            }
            //Update that unit has used an ability this turn
            this.SetUsedAbilityThisTurn(true);
        }

        //Randomly makes consecrated ground nearby or on player
        public void Consecrate()
        {
            if (consec_tile != null)
            {
                //consec_tile.SetModifier("basic");
                //map.SetTile(consec_row, consec_col, consec_tile);
            }

            map = Map.GetInstance();
            int player_row = player.GetRow();
            int player_col = player.GetCol();

            consec_tile = map.GetTile(player_row, player_col);
            consec_row = player_row;
            consec_col = player_col;
            //Don't consecrate over the lair
            if ((consec_row != map.GetLairRow() || consec_col != map.GetLairCol()))
            {
                consec_tile.SetModifier("consecrated");
                map.SetTile(player_row, player_col, consec_tile);
            }


            /*
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

                switch (seed)
                {
                    case 0: //player
                        consec_tile = map.GetTile(player_row, player_col);
                        consec_row = player_row;
                        consec_col = player_col;
                        if (!consec_tile.IsPassable() || (consec_row == map.GetLairRow() && consec_col == map.GetLairCol()))
                        {
                            break;
                        }
                        consec_tile.SetModifier("consecrated");
                        map.SetTile(player_row, player_col, consec_tile);
                        valid_seed = true;
                        break;
                    case 1: //right
                        if (!right_no)
                        {
                            consec_tile = map.GetTile(player_row, player_col + 1);
                            consec_row = player_row;
                            consec_col = player_col + 1;
                            if (!consec_tile.IsPassable() || (consec_row == map.GetLairRow() && consec_col == map.GetLairCol()))
                            {
                                break;
                            }
                            consec_tile.SetModifier("consecrated");
                            map.SetTile(player_row, player_col + 1, consec_tile);
                            valid_seed = true;
                        }
                        break;
                    case 2: //bottom right
                        if (!right_no && !bottom_no)
                        {
                            consec_tile = map.GetTile(player_row + 1, player_col + 1);
                            consec_row = player_row + 1;
                            consec_col = player_col + 1;
                            if (!consec_tile.IsPassable() || (consec_row == map.GetLairRow() && consec_col == map.GetLairCol()))
                            {
                                break;
                            }
                            consec_tile.SetModifier("consecrated");
                            map.SetTile(player_row + 1, player_col + 1, consec_tile);
                            valid_seed = true;
                        }
                        break;
                    case 3: //bottom
                        if (!bottom_no)
                        {
                            consec_tile = map.GetTile(player_row + 1, player_col);
                            consec_row = player_row + 1;
                            consec_col = player_col;
                            if (!consec_tile.IsPassable() || (consec_row == map.GetLairRow() && consec_col == map.GetLairCol()))
                            {
                                break;
                            }
                            consec_tile.SetModifier("consecrated");
                            map.SetTile(player_row + 1, player_col, consec_tile);
                            valid_seed = true;
                        }
                        break;
                    case 4: //bottom left
                        if (!left_no && !bottom_no)
                        {
                            consec_tile = map.GetTile(player_row + 1, player_col - 1);
                            consec_row = player_row + 1;
                            consec_col = player_col - 1;
                            if (!consec_tile.IsPassable() || (consec_row == map.GetLairRow() && consec_col == map.GetLairCol()))
                            {
                                break;
                            }
                            consec_tile.SetModifier("consecrated");
                            map.SetTile(player_row + 1, player_col - 1, consec_tile);
                            valid_seed = true;
                        }
                        break;
                    case 5: //left
                        if (!left_no)
                        {
                            consec_tile = map.GetTile(player_row, player_col - 1);
                            consec_row = player_row;
                            consec_col = player_col - 1;
                            if (!consec_tile.IsPassable() || (consec_row == map.GetLairRow() && consec_col == map.GetLairCol()))
                            {
                                break;
                            }
                            consec_tile.SetModifier("consecrated");
                            map.SetTile(player_row, player_col - 1, consec_tile);
                            valid_seed = true;
                        }
                        break;
                    case 6: //top left
                        if (!top_no && !left_no)
                        {
                            consec_tile = map.GetTile(player_row - 1, player_col - 1);
                            consec_row = player_row - 1;
                            consec_col = player_col - 1;
                            if (!consec_tile.IsPassable() || (consec_row == map.GetLairRow() && consec_col == map.GetLairCol()))
                            {
                                break;
                            }
                            consec_tile.SetModifier("consecrated");
                            map.SetTile(player_row - 1, player_col - 1, consec_tile);
                            valid_seed = true;
                        }
                        break;
                    case 7: //top
                        if (!top_no)
                        {
                            consec_tile = map.GetTile(player_row - 1, player_col);
                            consec_row = player_row - 1;
                            consec_col = player_col;
                            if (!consec_tile.IsPassable() || (consec_row == map.GetLairRow() && consec_col == map.GetLairCol()))
                            {
                                break;
                            }
                            consec_tile.SetModifier("consecrated");
                            map.SetTile(player_row - 1, player_col, consec_tile);
                            valid_seed = true;
                        }
                        break;
                    case 8: //top right
                        if (!top_no && !right_no)
                        {
                            consec_tile = map.GetTile(player_row - 1, player_col + 1);
                            consec_row = player_row - 1;
                            consec_col = player_col + 1;
                            if (!consec_tile.IsPassable() || (consec_row == map.GetLairRow() && consec_col == map.GetLairCol()))
                            {
                                break;
                            }
                            consec_tile.SetModifier("consecrated");
                            map.SetTile(player_row - 1, player_col + 1, consec_tile);
                            valid_seed = true;
                        }
                        break;
                }

            } while (!valid_seed);
            */

            //Update that unit has used an ability this turn
            this.SetUsedAbilityThisTurn(true);

            //Update that unit has used an ability this turn
            this.SetUsedAbilityThisTurn(true);
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

        //Returns the enemy at the given tile
        //If there is not an enemy at that tile, returns null
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

        //Returns a damaged enemy adjacent to the unit if there is one
        //If there is not an adjacent damaged enemy, returns null
        public Enemy GetAdjacentDamagedEnemy()
        {
            //Check above
            if (this.GetRow() - 1 > -1)
            {
                if (GetEnemy(this.GetRow() - 1, this.GetCol()) != null)
                {
                    if (GetEnemy(this.GetRow() - 1, this.GetCol()).IsDamaged())
                    {
                        return GetEnemy(this.GetRow() - 1, this.GetCol());
                    }
                }
            }
            //Check upper left
            if (this.GetRow() - 1 > -1 && this.GetCol() - 1 > -1)
            {
                if (GetEnemy(this.GetRow() - 1, this.GetCol() - 1) != null)
                {
                    if (GetEnemy(this.GetRow() - 1, this.GetCol() - 1).IsDamaged())
                    {
                        return GetEnemy(this.GetRow() - 1, this.GetCol() - 1);
                    }
                }
            }
            //Check upper right
            if (this.GetRow() - 1 > -1 && this.GetCol() + 1 < map.GetNumCols())
            {
                if (GetEnemy(this.GetRow() - 1, this.GetCol() + 1) != null)
                {
                    if (GetEnemy(this.GetRow() - 1, this.GetCol() + 1).IsDamaged())
                    {
                        return GetEnemy(this.GetRow() - 1, this.GetCol() + 1);
                    }
                }
            }
            //Check below
            if (this.GetRow() + 1 < map.GetNumRows())
            {
                if (GetEnemy(this.GetRow() + 1, this.GetCol()) != null)
                {
                    if (GetEnemy(this.GetRow() + 1, this.GetCol()).IsDamaged())
                    {
                        return GetEnemy(this.GetRow() + 1, this.GetCol());
                    }
                }
            }
            //Check bottom left
            if (this.GetRow() + 1 < map.GetNumRows() && this.GetCol() - 1 > -1)
            {
                if (GetEnemy(this.GetRow() + 1, this.GetCol() - 1) != null)
                {
                    if (GetEnemy(this.GetRow() + 1, this.GetCol() - 1).IsDamaged())
                    {
                        return GetEnemy(this.GetRow() + 1, this.GetCol() - 1);
                    }
                }
            }
            //Check bottom right
            if (this.GetRow() + 1 < map.GetNumRows() && this.GetCol() + 1 < map.GetNumCols())
            {
                if (GetEnemy(this.GetRow() + 1, this.GetCol() + 1) != null)
                {
                    if (GetEnemy(this.GetRow() + 1, this.GetCol() + 1).IsDamaged())
                    {
                        return GetEnemy(this.GetRow() + 1, this.GetCol() + 1);
                    }
                }
            }
            //Check left
            if (this.GetCol() - 1 > -1)
            {
                if (GetEnemy(this.GetRow(), this.GetCol() - 1) != null)
                {
                    if (GetEnemy(this.GetRow(), this.GetCol() - 1).IsDamaged())
                    {
                        return GetEnemy(this.GetRow(), this.GetCol() - 1);
                    }
                }
            }
            //Check right
            if (this.GetCol() + 1 < map.GetNumCols())
            {
                if (GetEnemy(this.GetRow(), this.GetCol() + 1) != null)
                {
                    if (GetEnemy(this.GetRow(), this.GetCol() + 1).IsDamaged())
                    {
                        return GetEnemy(this.GetRow(), this.GetCol() + 1);
                    }
                }
            }
            return null;
        }
    }
}
