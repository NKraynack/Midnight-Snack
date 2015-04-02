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
        private char[,] map_grid; //The grid for the map to generate shortest path
        public Enemy(Vector2 pos, int width, int height, int row, int col, int range, int health, Map map) 
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
                    //For testing purposes enemy just moves one tile to the left each turn
                    Debug.WriteLine("Enemy Turn");
                    if (this.AdjacentToPlayer())
                    {
                        Debug.WriteLine("Attacking player!");
                        //insert attack method
                        Attack(player);
                    }

                    if (this.GetCol() - 2 > 0) 
                    {
                        MapTile dest = map.GetTile(this.GetRow(), this.GetCol() - 2);
                        if (this.NoObstacles(this.GetCol() - 2, this.GetRow()) && dest.IsPassable())
                        {
                            Move(dest.GetPosition(), this.GetCol() - 2, this.GetRow());
                        }
                    }
                    
                    //End enemy's turn
                    hasEndedTurn = true;
                }
            }
 
        }

        private bool NoObstacles(int mov_x, int mov_y) //Do dijkstra and then return if movable
        {
            Queue<GridPoint> q = new Queue<GridPoint>();
            List<GridPoint> solution = new List<GridPoint>();
            HashSet<GridPoint> discovered = new HashSet<GridPoint>();
            Dictionary<GridPoint, GridPoint> prev = new Dictionary<GridPoint, GridPoint>();
            GridPoint player_pos = new GridPoint(player.GetCol(), player.GetRow());
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
            if (!current.Equals(cursor_pos))
            {
                return false;
            }
            for (GridPoint node = cursor_pos; node != player_pos; prev.TryGetValue(node, out node))
            {
                solution.Add(node);
            }
            return solution.Count <= player.GetMoveRange();
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
                neighbors.Add(new GridPoint(enemy_x- 1, enemy_y - 1));
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
            if (map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant() != null)
            {
                Debug.WriteLine("Enemy below something");
                if (map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant().GetType() == typeof(Player))
                {
                    Debug.WriteLine("Enemy below player");
                    return true;
                }
            }
            else if (map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant() != null)
            {
                Debug.WriteLine("Enemy above something");
                if (map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant().GetType() == typeof(Player))
                {
                    Debug.WriteLine("Enemy above player");
                    return true;
                }
            }
            else if (map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant() != null)
            {
                Debug.WriteLine("Enemy right of something");
                if (map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant().GetType() == typeof(Player))
                {
                    Debug.WriteLine("Enemy right of player");
                    return true;
                }
            }
            else if (map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant() != null)
            {
                Debug.WriteLine("Enemy left of something");
                if (map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant().GetType() == typeof(Player))
                {
                    Debug.WriteLine("Enemy left of player");
                    return true;
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
                int targetHealth = target.GetCurrentHealth() - 3;
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
