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
            foreach (int i in this.dests)
            {
                Console.WriteLine(i);
            }
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
                    int x = destCoords[0];
                    int y = destCoords[1];
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

            ////For right now, just make guard immobile
            //currentDest[0] = this.GetRow();
            //currentDest[1] = this.GetCol();

            return currentDest;
        }

        //enemy move method
        public virtual void EnemyMove(int destRow, int destCol, MapTile dest)
        {
            //Check if destination is within movement range
            //if (Math.Abs(destRow - this.GetRow()) + Math.Abs(destCol - this.GetCol()) <= this.GetMoveRange())
            //{
            //if (dest.IsPassable())
            //{
            GridPoint mov_dest = this.NoObstacles(destCol, destRow);
            int mov_dest_row = mov_dest.getY();
            int mov_dest_col = mov_dest.getX();
            Console.WriteLine("moving to: " + mov_dest);
            Vector2 dest_vector = map.GetTile(mov_dest_row, mov_dest_col).GetPosition();

            Console.WriteLine("moving " + dest_vector.X + ":" + dest_vector.Y + " " + destRow + " " + destCol);
            Move(dest_vector, mov_dest.getY(), mov_dest.getX());
            //}
            //}
        }

        public GridPoint NoObstacles(int mov_x, int mov_y) //Do dijkstra and then return if movable
        {

            Queue<GridPoint> q = new Queue<GridPoint>();
            List<GridPoint> solution = new List<GridPoint>();
            HashSet<GridPoint> discovered = new HashSet<GridPoint>();
            Dictionary<GridPoint, GridPoint> prev = new Dictionary<GridPoint, GridPoint>();
            GridPoint unit_pos = new GridPoint(this.GetCol(), this.GetRow());
            GridPoint current = unit_pos;
            Console.WriteLine("current:" + current);
            GridPoint dest_pos = new GridPoint(mov_x, mov_y);
            q.Enqueue(current);
            discovered.Add(current);
            this.map_grid = map.GenerateMapGrid();
            if (map.GetTile(mov_y, mov_x).GetOccupant() != null
                && (map.GetTile(mov_y, mov_x).GetOccupant().GetType() == typeof(SleepingVillager)
                || (map.GetTile(mov_y, mov_x).GetOccupant().GetType() == typeof(Player))))
            {
                MapTile obstacle = map.GetTile(mov_y, mov_x);
                obstacle.SetModifier("basic");
                obstacle.SetPassable(true);
                map.SetTile(mov_y, mov_x, obstacle);
            }
            this.map_grid = map.GenerateMapGrid();
            if (this.AdjacentToPlayer())
            {
                return unit_pos;
            }

            while (q.Count != 0)
            {
                current = q.Dequeue();
                if (current.Equals(dest_pos))
                {
                    break;
                }
                else
                {
                    /*System.Diagnostics.Debug.WriteLine("starting");
                    System.Diagnostics.Debug.WriteLine("cols:" + max_columns);
                    System.Diagnostics.Debug.WriteLine("rows:" + max_rows);*/
                    foreach (GridPoint node in getNeighbors(map.GetNumCols(), map.GetNumRows(), current, map_grid))
                    {
                        //System.Diagnostics.Debug.WriteLine("looking: " + node.ToString());
                        if (!discovered.Contains(node))
                        {
                            //System.Diagnostics.Debug.WriteLine("adding: " + node.ToString());
                            q.Enqueue(node);
                            prev.Add(node, current);
                            discovered.Add(node);
                        }
                    }
                }
            }
            if (map.GetTile(mov_y, mov_x).GetOccupant() != null
                && map.GetTile(mov_y, mov_x).GetOccupant().GetType() == typeof(SleepingVillager))
            {
                MapTile obstacle = map.GetTile(mov_y, mov_x);
                obstacle.SetModifier("villager");
                obstacle.SetPassable(false);
                map.SetTile(mov_y, mov_x, obstacle);
            }
            if (map.GetTile(mov_y, mov_x).GetOccupant() != null
                && map.GetTile(mov_y, mov_x).GetOccupant().GetType() == typeof(Player))
            {
                MapTile obstacle = map.GetTile(mov_y, mov_x);
                obstacle.SetModifier("player");
                obstacle.SetPassable(false);
                map.SetTile(mov_y, mov_x, obstacle);
            }
            if (!current.Equals(dest_pos))
            {
                return unit_pos;
            }
            for (GridPoint node = dest_pos; node != unit_pos; prev.TryGetValue(node, out node))
            {
                Console.WriteLine("adding " + node);
                solution.Add(node);
            }

            if (solution.Count == 0)
            {
                return unit_pos;
            }
            else if (solution.Count > this.GetMoveRange())
            {
                solution.Reverse();
                return solution.ElementAt(this.GetMoveRange() - 1);
            }
            else
            {
                return solution.ElementAt(0);
            }
        }
    }
}
