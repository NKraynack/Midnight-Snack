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
        Player player = Player.GetInstance();
        protected int strength; //How much the enemy hits for
        protected char[,] map_grid; //The grid for the map to generate shortest path

        public Enemy(Vector2 pos, int width, int height, int row, int col, int range, int health, Map map)
            : base(pos, width, height, row, col, range, health, map)
        {
            this.map_grid = map.GenerateMapGrid();

            strength = 3;
        }

        public override void LoadContent(ContentManager content)
        {
            //temp until I either draw one or find one
            texture = content.Load<Texture2D>("goomba");
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
                    MapTile dest = map.GetTile(destCoords[0], destCoords[1]);
                    EnemyMove(destCoords[0], destCoords[1], dest);

                    //If did not use any abilities before moving, try now
                    this.UseAbilities();

                    //End enemy's turn
                    hasEndedTurn = true;
                }
            }

        }

        //Enemy uses any relevant abilities
        //Subclasses should override this method
        public virtual void UseAbilities()
        {
            //Attack the player if adjacent
            if (this.AdjacentToPlayer() && !this.HasUsedAbilityThisTurn())
            {
                Debug.WriteLine("Enemy attacking player!");
                Attack(player);
            }
        }

        //Determine where to move
        //Returns an int array containing [destRow, destCol]
        public virtual int[] GetDestination()
        {
            //Destination defaults to current position
            //int[] destination = {this.GetRow(), this.GetCol()};
            int[] destination = { player.GetRow(), player.GetCol() };
            //Subclasses should override this method and calculate dest here

            return destination;
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
            //for (int i = 0; i < map_grid.GetLength(0); i++)
            //{
            //    for (int j = 0; j < map_grid.GetLength(1); j++)
            //    {
            //        Console.Write(map_grid[i, j]);
            //    }
            //    Console.WriteLine();
            //}
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
            if (!current.Equals(dest_pos))
            {
                return unit_pos;
            }
            for (GridPoint node = dest_pos; node != unit_pos; prev.TryGetValue(node, out node))
            {
                Console.WriteLine("adding " + node);
                solution.Add(node);
            }
            if (solution.Count != 0)
            {
                solution.RemoveAt(0);
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

        public List<GridPoint> getNeighbors(int x_limit, int y_limit, GridPoint cur_point, char[,] grid)
        {
            /*
            System.Diagnostics.Debug.WriteLine("getting neighbors");
            System.Diagnostics.Debug.WriteLine("xlimit:" + x_limit);
            System.Diagnostics.Debug.WriteLine("ylimit" + y_limit);
            System.Diagnostics.Debug.WriteLine(cur_point.ToString());
             * */
            List<GridPoint> neighbors = new List<GridPoint>();
            int enemy_x = cur_point.getX();
            int enemy_y = cur_point.getY();

            //bottom right
            if ((enemy_x + 1 < x_limit) && (enemy_y + 1 < y_limit) && (enemy_x + 1 >= 0) && (enemy_y + 1 >= 0) &&
                grid[enemy_x + 1, enemy_y + 1] != 'x')
            {
                neighbors.Add(new GridPoint(enemy_x + 1, enemy_y + 1));
            }
            //bottom
            if ((enemy_x < x_limit) && (enemy_y + 1 < y_limit) && (enemy_x >= 0) && (enemy_y + 1 >= 0) &&
                grid[enemy_x, enemy_y + 1] != 'x')
            {
                neighbors.Add(new GridPoint(enemy_x, enemy_y + 1));
            }
            //bottom left
            if ((enemy_x - 1 < x_limit) && (enemy_y + 1 < y_limit) && (enemy_x - 1 >= 0) && (enemy_y + 1 >= 0) &&
                grid[enemy_x - 1, enemy_y + 1] != 'x')
            {
                neighbors.Add(new GridPoint(enemy_x - 1, enemy_y + 1));
            }
            //left
            if ((enemy_x - 1 < x_limit) && (enemy_y < y_limit) && (enemy_x - 1 >= 0) && (enemy_y >= 0) &&
                grid[enemy_x - 1, enemy_y] != 'x')
            {
                neighbors.Add(new GridPoint(enemy_x - 1, enemy_y));
            }
            //upper left
            if ((enemy_x - 1 < x_limit) && (enemy_y - 1 < y_limit) && (enemy_x - 1 >= 0) && (enemy_y - 1 >= 0) &&
                grid[enemy_x - 1, enemy_y - 1] != 'x')
            {
                neighbors.Add(new GridPoint(enemy_x - 1, enemy_y - 1));
            }
            //top
            if ((enemy_x < x_limit) && (enemy_y - 1 < y_limit) && (enemy_x >= 0) && (enemy_y - 1 >= 0) &&
                grid[enemy_x, enemy_y - 1] != 'x')
            {
                neighbors.Add(new GridPoint(enemy_x, enemy_y - 1));
            }
            //top right
            if ((enemy_x + 1 < x_limit) && (enemy_y - 1 < y_limit) && (enemy_x + 1 >= 0) && (enemy_y - 1 >= 0) &&
                grid[enemy_x + 1, enemy_y - 1] != 'x')
            {
                neighbors.Add(new GridPoint(enemy_x + 1, enemy_y - 1));
            }
            //right
            if ((enemy_x + 1 < x_limit) && (enemy_y < y_limit) && (enemy_x + 1 >= 0) && (enemy_y >= 0) &&
                grid[enemy_x + 1, enemy_y] != 'x')
            {
                neighbors.Add(new GridPoint(enemy_x + 1, enemy_y));
            }
            //System.Diagnostics.Debug.WriteLine("neighbors: " + neighbors.Count);
            return neighbors;
        }

        public bool AdjacentToPlayer()
        {
            //Check above enemy
            if (this.GetRow() - 1 > -1)
            {
                if (map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy below something");
                    if (map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant().GetType() == typeof(Player))
                    {
                        Debug.WriteLine("Enemy below player");
                        return true;
                    }
                }
            }
            //Check upper left
            if (this.GetRow() - 1 > -1 && this.GetCol() - 1 > -1)
            {
                if (map.GetTile(this.GetRow() - 1, this.GetCol() - 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Something at enemy's upper left");
                    if (map.GetTile(this.GetRow() - 1, this.GetCol() - 1).GetOccupant().GetType() == typeof(Player))
                    {
                        Debug.WriteLine("Player at enemy's upper left");
                        return true;
                    }
                }
            }
            //Check upper right
            if (this.GetRow() - 1 > -1 && this.GetCol() + 1 < map.GetNumCols())
            {
                if (map.GetTile(this.GetRow() - 1, this.GetCol() + 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Something at enemy's upper right");
                    if (map.GetTile(this.GetRow() - 1, this.GetCol() + 1).GetOccupant().GetType() == typeof(Player))
                    {
                        Debug.WriteLine("Player at enemy's upper right");
                        return true;
                    }
                }
            }
            //Check below enemy
            if (this.GetRow() + 1 < map.GetNumRows())
            {
                if (map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy above something");
                    if (map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant().GetType() == typeof(Player))
                    {
                        Debug.WriteLine("Enemy above player");
                        return true;
                    }
                }
            }
            //Check bottom left
            if (this.GetRow() + 1 < map.GetNumRows() && this.GetCol() - 1 > -1)
            {
                if (map.GetTile(this.GetRow() + 1, this.GetCol() - 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Something at enemy's bottom left");
                    if (map.GetTile(this.GetRow() + 1, this.GetCol() - 1).GetOccupant().GetType() == typeof(Player))
                    {
                        Debug.WriteLine("Player at enemy's bottom left");
                        return true;
                    }
                }
            }
            //Check bottom right
            if (this.GetRow() + 1 < map.GetNumRows() && this.GetCol() + 1 > map.GetNumCols())
            {
                if (map.GetTile(this.GetRow() + 1, this.GetCol() + 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Something at enemy's bottom right");
                    if (map.GetTile(this.GetRow() + 1, this.GetCol() + 1).GetOccupant().GetType() == typeof(Player))
                    {
                        Debug.WriteLine("Player at enemy's bottom right");
                        return true;
                    }
                }
            }
            //Check left of enemy
            if (this.GetCol() - 1 > -1)
            {
                if (map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy right of something");
                    if (map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant().GetType() == typeof(Player))
                    {
                        Debug.WriteLine("Enemy right of player");
                        return true;
                    }
                }
            }
            //Check right of enemy
            if (this.GetCol() + 1 < map.GetNumCols())
            {
                if (map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy left of something");
                    if (map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant().GetType() == typeof(Player))
                    {
                        Debug.WriteLine("Enemy left of player");
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Attack(MobileUnit target)
        {
            //Target must still be alive
            if (target.IsAlive())
            {
                //Update the target's health
                int targetHealth = target.GetCurrentHealth() - strength;
                target.SetCurrentHealth(targetHealth);
                //Updated map tile of target
                MapTile tile = map.GetTile(target.GetRow(), target.GetCol());
                tile.SetOccupant(target);
                //Update that unit has used an ability this turn
                this.SetUsedAbilityThisTurn(true);
            }
        }
    }
}
