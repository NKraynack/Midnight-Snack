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
    public class VampireEnemy : Enemy
    {
        Player player = Player.GetInstance();
        Map map = Map.GetInstance();

        public VampireEnemy(Vector2 pos, int width, int height, int row, int col, int range, int health)
            : base(pos, width, height, row, col, range, health)
        {
            this.map_grid = map.GenerateMapGrid();
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("vampire_enemy");
            healthBar.LoadContent(content);
            stats.LoadContent(content);
        }

        public override void Update()
        {
            healthBar.Update(position, currentHealth);
            stats.Update(this);

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
                    Debug.WriteLine("Vampire Enemy Turn");

                    //Use any relevant abilities
                    this.UseAbilities();

                    //Handle enemy movement
                    int[] destCoords = GetDestination();
                    MapTile dest = map.GetTile(destCoords[0], destCoords[1]);
                    EnemyMove(destCoords[0], destCoords[1], dest);
                    //Debug.WriteLine("Moved to Destination");

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

        //Determine where to move
        //Returns an int array containing [destRow, destCol]
        public override int[] GetDestination()
        {
            //Destination defaults to current position
            int[] destination = {this.GetRow(), this.GetCol()};

            //Subclasses should override this method and calculate dest here
            SleepingVillager closestVillager = GetClosestVillager();
            if (closestVillager != null)
            {
                
                destination[0] = closestVillager.GetRow();
                destination[1] = closestVillager.GetCol();
                

                //destination = FindPassableAdjTile(closestVillager);
            }
            else
            {
                destination[0] = player.GetRow();
                destination[1] = player.GetCol();
            }
            //Debug.WriteLine("Destination: " + destination[0] + ", " + destination[1]);
            return destination;
        }

        public void Feed(SleepingVillager target)
        {
            //Villager must not already have been drained
            if (!target.IsDrained())
            {
                //Update the villager as drained
                target.SetDrained(true);
                //Update that vampire has used an ability this turn
                this.SetUsedAbilityThisTurn(true);
            }
        }

        public override void UseAbilities()
        {
            //Feed on an adjacent villager if possible
            if (this.GetAdjacentVillager() != null && !this.HasUsedAbilityThisTurn())
            {
                if (!this.GetAdjacentVillager().IsDrained())
                {
                    Debug.WriteLine("Vampire feeding on villager");
                    Feed(this.GetAdjacentVillager());
                }
            }
            //Attack the player if adjacent
            if (this.AdjacentToPlayer() && !this.HasUsedAbilityThisTurn())
            {
                Debug.WriteLine("Vampire attacking player!");
                Attack(player);
            }
        }

        //Returns a villager adjacent to the unit if there is one
        //If there is not an adjacent villager, returns null
        public SleepingVillager GetAdjacentVillager()
        {
            //Check above enemy
            if (this.GetRow() - 1 > -1)
            {
                if (map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy below something");
                    if (map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Enemy below villager");
                        return (SleepingVillager) map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant();
                    }
                }
            }
            //Check upper left
            if (this.GetRow() - 1 > -1 && this.GetCol() - 1 > -1)
            {
                if (map.GetTile(this.GetRow() - 1, this.GetCol() - 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Something at enemy's upper left");
                    if (map.GetTile(this.GetRow() - 1, this.GetCol() - 1).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Villager at enemy's upper left");
                        return (SleepingVillager) map.GetTile(this.GetRow() - 1, this.GetCol() - 1).GetOccupant();
                    }
                }
            }
            //Check upper right
            if (this.GetRow() - 1 > -1 && this.GetCol() + 1 < map.GetNumCols())
            {
                if (map.GetTile(this.GetRow() - 1, this.GetCol() + 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Something at enemy's upper right");
                    if (map.GetTile(this.GetRow() - 1, this.GetCol() + 1).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Villager at enemy's upper right");
                        return (SleepingVillager) map.GetTile(this.GetRow() - 1, this.GetCol() + 1).GetOccupant();
                    }
                }
            }
            //Check below enemy
            if (this.GetRow() + 1 < map.GetNumRows())
            {
                if (map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy above something");
                    if (map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Enemy above villager");
                        return (SleepingVillager) map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant();
                    }
                }
            }
            //Check bottom left
            if (this.GetRow() + 1 < map.GetNumRows() && this.GetCol() - 1 > -1)
            {
                if (map.GetTile(this.GetRow() + 1, this.GetCol() - 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Something at enemy's bottom left");
                    if (map.GetTile(this.GetRow() + 1, this.GetCol() - 1).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Villager at enemy's bottom left");
                        return (SleepingVillager) map.GetTile(this.GetRow() + 1, this.GetCol() - 1).GetOccupant();
                    }
                }
            }
            //Check bottom right
            if (this.GetRow() + 1 < map.GetNumRows() && this.GetCol() + 1 < map.GetNumCols())
            {
                if (map.GetTile(this.GetRow() + 1, this.GetCol() + 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Something at enemy's bottom right");
                    if (map.GetTile(this.GetRow() + 1, this.GetCol() + 1).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Villager at enemy's bottom right");
                        return (SleepingVillager) map.GetTile(this.GetRow() + 1, this.GetCol() + 1).GetOccupant();
                    }
                }
            }
            //Check enemy's left
            if (this.GetCol() - 1 > -1)
            {
                if (map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy right of something");
                    if (map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Enemy right of villager");
                        return (SleepingVillager) map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant();
                    }
                }
            }
            //Check enemy's right
            if (this.GetCol() + 1 < map.GetNumCols())
            {
                if (map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy left of something");
                    if (map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Enemy left of villager");
                        return (SleepingVillager) map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant();
                    }
                }
            }
            return null;
        }

        //Returns the undrained villager at the given tile
        //If there is not an undrained villager at that tile, returns null
        public SleepingVillager GetUndrainedVillager(int row, int col)
        {
            if (row > -1 && col > -1 && row < map.GetNumRows() && col < map.GetNumCols())
            {
                if (map.GetTile(row, col).GetOccupant() != null)
                {
                    if (map.GetTile(row, col).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        //Check if villager is drained
                        if (!((SleepingVillager)map.GetTile(row, col).GetOccupant()).IsDrained())
                        {
                            return (SleepingVillager)map.GetTile(row, col).GetOccupant();
                        }
                    }
                }
            }

            return null;
        }

        //Returns the closest undrained villager to the vampire's location
        //If there are no undrained villagers, returns null
        public SleepingVillager GetClosestVillager()
        {
            int row = this.GetRow();
            int col = this.GetCol();

            //Looks for villagers in a widening counterclockwise spiral around vampire
            for (int i = 1; i < Math.Max(map.GetNumRows(), map.GetNumCols()); i++)
            {
                //left
                col = this.GetCol() - i;
                //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                if (GetUndrainedVillager(row, col) != null)
                {
                    return GetUndrainedVillager(row, col);
                }
                //bottom left
                for (int j = 0; j < i; j++)
                {
                    row += 1;
                    //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                    if (GetUndrainedVillager(row, col) != null)
                    {
                        return GetUndrainedVillager(row, col);
                    }
                }
                //bottom
                for (int j = 0; j < i * 2; j++)
                {
                    col += 1;
                    //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                    if (GetUndrainedVillager(row, col) != null)
                    {
                        return GetUndrainedVillager(row, col);
                    }
                }
                //right
                for (int j = 0; j < i * 2; j++)
                {
                    row -= 1;
                    //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                    if (GetUndrainedVillager(row, col) != null)
                    {
                        return GetUndrainedVillager(row, col);
                    }
                }
                //top
                for (int j = 0; j < i * 2; j++)
                {
                    col -= 1;
                    //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                    if (GetUndrainedVillager(row, col) != null)
                    {
                        return GetUndrainedVillager(row, col);
                    }
                }
                //top left
                for (int j = 0; j < i; j++)
                {
                    row += 1;
                    //Debug.WriteLine("Checking (" + row + ", " + col + ")");
                    if (GetUndrainedVillager(row, col) != null)
                    {
                        return GetUndrainedVillager(row, col);
                    }
                }
            }

            return null;
        }

        public int[] FindPassableAdjTile(Unit unit)
        {
            //List of all the tiles adjacent to the closest villager which are also passable
            List<int[]> validVillagerAdjTiles = new List<int[]>();

            //Check above 
            if (unit.GetRow() - 1 > -1)
            {
                if (map.GetTile(unit.GetRow() - 1, unit.GetCol()).IsPassable())
                {
                    validVillagerAdjTiles.Add(new int[]{unit.GetRow() - 1, unit.GetCol()});
                }
            }
            //Check upper left
            if (unit.GetRow() - 1 > -1 && unit.GetCol() - 1 > -1)
            {
                if (map.GetTile(unit.GetRow() - 1, unit.GetCol() - 1).IsPassable())
                {
                    validVillagerAdjTiles.Add(new int[]{unit.GetRow() - 1, unit.GetCol() - 1});
                }
            }
            //Check upper right
            if (unit.GetRow() - 1 > -1 && unit.GetCol() + 1 < map.GetNumCols())
            {
                if (map.GetTile(unit.GetRow() - 1, unit.GetCol() + 1).IsPassable())
                {
                    validVillagerAdjTiles.Add(new int[]{unit.GetRow() - 1, unit.GetCol() + 1});
                }
            }
            //Check below
            if (unit.GetRow() + 1 < map.GetNumRows())
            {
                if (map.GetTile(unit.GetRow() + 1, unit.GetCol()).IsPassable())
                {
                    validVillagerAdjTiles.Add(new int[]{unit.GetRow() + 1, unit.GetCol()});
                }
            }
            //Check bottom left
            if (unit.GetRow() + 1 < map.GetNumRows() && unit.GetCol() - 1 > -1)
            {
                if (map.GetTile(unit.GetRow() + 1, unit.GetCol() - 1).IsPassable())
                {
                    validVillagerAdjTiles.Add(new int[]{unit.GetRow() + 1, unit.GetCol() - 1});
                }
            }
            //Check bottom right
            if (unit.GetRow() + 1 < map.GetNumRows() && unit.GetCol() + 1 < map.GetNumCols())
            {
                if (map.GetTile(unit.GetRow() + 1, unit.GetCol() + 1).IsPassable())
                {
                    validVillagerAdjTiles.Add(new int[]{unit.GetRow() + 1, unit.GetCol() + 1});
                }
            }
            //Check unit's left
            if (unit.GetCol() - 1 > -1)
            {
                if (map.GetTile(unit.GetRow(), unit.GetCol() - 1).IsPassable())
                {
                    validVillagerAdjTiles.Add(new int[]{unit.GetRow(), unit.GetCol() - 1});
                }
            }
            //Check unit's right
            if (unit.GetCol() + 1 < map.GetNumCols())
            {
                if (map.GetTile(unit.GetRow(), unit.GetCol() + 1).IsPassable())
                {
                    validVillagerAdjTiles.Add(new int[]{unit.GetRow(), unit.GetCol() + 1});
                }
            }

            //Find closest tile
            int closestIndex = 0;
            int shortestDist = Math.Abs(this.GetRow() - validVillagerAdjTiles[0][0]) + Math.Abs(this.GetCol() - validVillagerAdjTiles[0][1]);
            for(int i = 0; i < validVillagerAdjTiles.Count; i++)
            {
                int dist = Math.Abs(this.GetRow() - validVillagerAdjTiles[i][0]) + Math.Abs(this.GetCol() - validVillagerAdjTiles[i][1]);
                if(dist < shortestDist)
                {
                    shortestDist = dist;
                    closestIndex = i;
                }
            }

            return validVillagerAdjTiles[closestIndex];

        }

    }
}
