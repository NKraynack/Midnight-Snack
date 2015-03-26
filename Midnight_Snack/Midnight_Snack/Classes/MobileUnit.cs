using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Midnight_Snack
{

    public struct GridPoint : IEquatable<GridPoint>
    {
        private int x;
        private int y;

        public GridPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }
        public int getX()
        {
            return x;
        }
        public int getY()
        {
            return y;
        }
        public static bool operator ==(GridPoint p1, GridPoint p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }

        public static bool operator !=(GridPoint p1, GridPoint p2)
        {
            return !(p1.x == p2.x && p1.y == p2.y);
        }

        public bool Equals(GridPoint other)
        {
            if (other == null)
                return false;

            return this.x == other.x && this.y == other.y;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            // Suitable nullity checks etc, of course :)
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GridPoint))
                return false;
            var p = (GridPoint)obj;
            return this.x == p.x && this.y == p.y;
        }
    }

    public class MobileUnit : Unit
    {
        protected int moveRange;  //Number of tiles the unit can move in one turn
        protected int maxHealth;  //The greatest amount of health this unit can have
        protected int currentHealth;  //The current amount of health this unit has
        protected bool movedThisTurn; //Has the unit moved this turn?
        protected bool usedAbilityThisTurn;   //Has the unit used an ability this turn?
        protected bool alive; //Is the unit alive?
        public HealthBar healthBar; //The unit's health bar
        private char[,] map_grid; //The grid for the map to generate shortest path
        private int max_columns;
        private int max_rows;
        protected Map map;

        public MobileUnit(Vector2 pos, int width, int height, int row, int col, int range, int health, Map m) : base(pos, width, height, row, col)
        {
            moveRange = range;
            maxHealth = health;
            currentHealth = health;
            movedThisTurn = false;
            usedAbilityThisTurn = false;
            alive = true;
            unitsTurn = false;
            healthBar = new HealthBar(new Vector2(position.X, position.Y - 10), maxHealth);
            map = m;
        }

        //Moves the unit to the given position
        public bool Move(Vector2 pos, int row, int col)
        {
            if (NoObstacles(col, row)) //check if can move
            {
                MapTile prev = map.GetTile(this.GetRow(), this.GetCol());
                prev.SetPassable(true);
                map.SetTile(this.GetRow(), this.GetCol(), prev);

                SetPosition(pos);
                SetRow(row);
                SetCol(col);
                movedThisTurn = true;

                MapTile obstacle = map.GetTile(this.GetRow(), this.GetCol());
                obstacle.SetPassable(false);
                map.SetTile(this.GetRow(), this.GetCol(), obstacle);

                return true;
            }
            return false;
        }

        private bool NoObstacles(int mov_x, int mov_y) //Do dijkstra and then return if movable
        {
            Queue<GridPoint> q = new Queue<GridPoint>();
            List<GridPoint> solution = new List<GridPoint>();
            HashSet<GridPoint> discovered = new HashSet<GridPoint>();
            Dictionary<GridPoint, GridPoint> prev = new Dictionary<GridPoint, GridPoint>();
            GridPoint player_pos = new GridPoint(this.GetCol(), this.GetRow());
            GridPoint current = player_pos;
            GridPoint cursor_pos = new GridPoint(mov_x, mov_y);
            q.Enqueue(current);
            discovered.Add(current);
            while (q.Count != 0)
            {
                current = q.Dequeue();
                if (current.Equals(cursor_pos))
                {
                    break;
                }
                else
                {
                    /*System.Diagnostics.Debug.WriteLine("starting");
                    System.Diagnostics.Debug.WriteLine("cols:" + max_columns);
                    System.Diagnostics.Debug.WriteLine("rows:" + max_rows);*/
                    foreach (GridPoint node in getNeighbors(max_columns, max_rows, current, map_grid))
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
            if (!current.Equals(cursor_pos))
            {
                return false;
            }
            //trace back links in hashmap and then build path from here
            for (GridPoint node = cursor_pos; node != player_pos; prev.TryGetValue(node, out node))
            {
                solution.Add(node);
            }
            //if optimal path size is smaller than movement range return true
            return solution.Count <= this.GetMoveRange();
        }

        private List<GridPoint> getNeighbors(int x_limit, int y_limit, GridPoint cur_point, char[,] grid)
        {
            /*
            System.Diagnostics.Debug.WriteLine("getting neighbors");
            System.Diagnostics.Debug.WriteLine("xlimit:" + x_limit);
            System.Diagnostics.Debug.WriteLine("ylimit" + y_limit);
            System.Diagnostics.Debug.WriteLine(cur_point.ToString());
             * */
            List<GridPoint> neighbors = new List<GridPoint>();
            int player_x = cur_point.getX();
            int player_y = cur_point.getY();

            //bottom right
            if ((player_x + 1 < x_limit) && (player_y + 1 < y_limit) && (player_x + 1 >= 0) && (player_y + 1 >= 0) &&
                grid[player_x + 1, player_y + 1] != 'x')
            {
                neighbors.Add(new GridPoint(player_x + 1, player_y + 1));
            }
            //bottom
            if ((player_x < x_limit) && (player_y + 1 < y_limit) && (player_x >= 0) && (player_y + 1 >= 0) &&
                grid[player_x, player_y + 1] != 'x')
            {
                neighbors.Add(new GridPoint(player_x, player_y + 1));
            }
            //bottom left
            if ((player_x - 1 < x_limit) && (player_y + 1 < y_limit) && (player_x - 1 >= 0) && (player_y + 1 >= 0) &&
                grid[player_x - 1, player_y + 1] != 'x')
            {
                neighbors.Add(new GridPoint(player_x - 1, player_y + 1));
            }
            //left
            if ((player_x - 1 < x_limit) && (player_y < y_limit) && (player_x - 1 >= 0) && (player_y >= 0) &&
                grid[player_x - 1, player_y] != 'x')
            {
                neighbors.Add(new GridPoint(player_x - 1, player_y));
            }
            //upper left
            if ((player_x - 1 < x_limit) && (player_y - 1 < y_limit) && (player_x - 1 >= 0) && (player_y - 1 >= 0) &&
                grid[player_x - 1, player_y - 1] != 'x')
            {
                neighbors.Add(new GridPoint(player_x - 1, player_y - 1));
            }
            //top
            if ((player_x < x_limit) && (player_y - 1 < y_limit) && (player_x >= 0) && (player_y - 1 >= 0) &&
                grid[player_x, player_y - 1] != 'x')
            {
                neighbors.Add(new GridPoint(player_x, player_y - 1));
            }
            //top right
            if ((player_x + 1 < x_limit) && (player_y - 1 < y_limit) && (player_x + 1 >= 0) && (player_y - 1 >= 0) &&
                grid[player_x + 1, player_y - 1] != 'x')
            {
                neighbors.Add(new GridPoint(player_x + 1, player_y - 1));
            }
            //right
            if ((player_x + 1 < x_limit) && (player_y < y_limit) && (player_x + 1 >= 0) && (player_y >= 0) &&
                grid[player_x + 1, player_y] != 'x')
            {
                neighbors.Add(new GridPoint(player_x + 1, player_y));
            }
            //System.Diagnostics.Debug.WriteLine("neighbors: " + neighbors.Count);
            return neighbors;
        }

        public void UseAbility(string ability)
        {
            //Override in subclasses
        }

        //Set the current map here
        public void SetMapGrid(char[,] mapGrid)
        {
            this.map_grid = mapGrid;
            max_columns = map_grid.GetLength(0);
            max_rows = map_grid.GetLength(1);

        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public void SetCurrentHealth(int health)
        {
            currentHealth = health;
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }

        public void SetMaxHealth(int health)
        {
            maxHealth = health;
        }

        public int GetMoveRange()
        {
            return moveRange;
        }

        public void SetMoveRange(int range)
        {
            moveRange = range;
        }

        public bool HasMovedThisTurn()
        {
            return movedThisTurn;
        }

        public void SetMovedThisTurn(bool b)
        {
            movedThisTurn = b;
        }

        public bool HasUsedAbilityThisTurn()
        {
            return usedAbilityThisTurn;
        }

        public void SetUsedAbilityThisTurn(bool b)
        {
            usedAbilityThisTurn = b;
        }

        public bool IsAlive()
        {
            return alive;
        }

        public void SetAlive(bool b)
        {
            alive = b;
        }

        public override void Update()
        {
            healthBar.Update(position, currentHealth);

            if(currentHealth <= 0)
            {
                alive = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            healthBar.Draw(spriteBatch);
        }
    }
}
