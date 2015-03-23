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

    public class Cursor : GameObject
    {
        private int cursorRow;  //The row of the grid which the cursor is currently on
        private int cursorCol;  //The col of the grid which the cursor is currently on
        private Map map;    //The map the cursor is on
        private int maxRow; //The Bottom-most row in the grid
        private int maxCol; //The right-most column in the grid
        private char[,] map_grid; //The grid for the map to generate shortest path
        private int max_columns;
        private int max_rows;

        GameManager gameManager = GameManager.GetInstance();
        Player player = Player.GetInstance();

        public Cursor(int x, int y, int width, int height, Map map) : base(x, y, width, height)
        {
            this.map = map;
            cursorRow = map.GetLairRow();
            cursorCol = map.GetLairCol();
            maxRow = map.GetNumRows() - 1;
            maxCol = map.GetNumCols() - 1;
            this.map_grid = map.GenerateMapGrid();
            max_columns = map.GetNumCols();
            max_rows = map.GetNumRows();
        }

        public Cursor(Vector2 pos, int width, int height, Map map) : base(pos, width, height)
        {
            this.map = map;
            cursorRow = map.GetLairRow();
            cursorCol = map.GetLairCol();
            maxRow = map.GetNumRows() - 1;
            maxCol = map.GetNumCols() - 1;
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("cursor.png");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //If not moving player display cursor normally
            if (!gameManager.IsMovingPlayer())
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
            //When moving player, color cursor red when out of move range
            //or tile is invalid
            else
            {
                MapTile tile = map.GetTile(cursorRow, cursorCol);
                if (Math.Abs(cursorRow - player.GetRow()) + Math.Abs(cursorCol - player.GetCol()) > player.GetMoveRange()
                    || tile.GetOccupant() != null || !tile.IsPassable())
                {
                    spriteBatch.Draw(texture, position, Color.Red);
                }
                //If move is valid, then color normally
                else
                {
                    spriteBatch.Draw(texture, position, Color.White);
                }
            }
        }

        public void Update(Controls controls)
        {
            if (gameManager.IsChoosingAbilityTarget())
            {
                SelectAbilityTarget(controls);
            }
            //If not in the action menu, cursor should move around map
            else if (!gameManager.IsInActionMenu() && !gameManager.IsChoosingAbilityTarget())
            {
                Move(controls);
                SelectTile(controls);

                if (gameManager.IsMovingPlayer())
                {
                    MovePlayer(controls);
                }
            }
        }

        //Move the cursor around the map
        public void Move(Controls controls)
        {
            if (controls.onPress(Keys.Right, Buttons.DPadRight) && cursorCol != maxCol)
            {
                SetX(GetX() + width);
                cursorCol++;
            }
            else if (controls.onPress(Keys.Left, Buttons.DPadLeft) && cursorCol != 0)
            {
                SetX(GetX() - width);
                cursorCol--;
            }
            else if (controls.onPress(Keys.Up, Buttons.DPadUp) && cursorRow != 0)
            {
                SetY(GetY() - height);
                cursorRow--;
            }
            else if (controls.onPress(Keys.Down, Buttons.DPadDown) && cursorRow != maxRow)
            {
                SetY(GetY() + height);
                cursorRow++;
            }
        }

        //Moves the player to a valid tile
        public void MovePlayer(Controls controls)
        {
            if (controls.onPress(Keys.Space, Buttons.A) && gameManager.IsMovingPlayer())
            {
                //If player chooses a valid tile within their move range, let them move there
                if (Math.Abs(cursorRow - player.GetRow()) + Math.Abs(cursorCol - player.GetCol()) <= player.GetMoveRange())
                {
                    //Check if tile is unoccupied and passable
                    MapTile tile = map.GetTile(cursorRow, cursorCol);
                    if (tile.GetOccupant() == null && tile.IsPassable() && NoObstacles(cursorCol, cursorRow))
                    {
                        //Move player to tile
                        player.Move(position, cursorRow, cursorCol);
                        //Update that player has moved this turn
                        player.SetMovedThisTurn(true);
                        gameManager.SetMovingPlayer(false);
                        gameManager.SetInActionMenu(false);
                    }
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
                    foreach (GridPoint node in getNeighbors(max_columns, max_rows, current, map_grid))
                    {
                        if (!discovered.Contains(node))
                        {
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
            return neighbors;
        }
        public void SelectTile(Controls controls)
        {
            //Get the occupant of the selected tile
            MapTile tile = map.GetTile(cursorRow, cursorCol);
            GameObject occupant = tile.GetOccupant();

            //If selecting the tile that the player is on...
            if (controls.onPress(Keys.Space, Buttons.A) 
                && cursorRow == player.GetRow() && cursorCol == player.GetCol())
            {
                //...open up action menu
                gameManager.SetInActionMenu(true);
            }
        }

        //Can only choose to use ability on a tile directly adjacent to player
        public void SelectAbilityTarget(Controls controls)
        {
            int maxRight = player.GetCol() + 1;
            int maxLeft = player.GetCol() - 1;
            int maxUp = player.GetRow() - 1;
            int maxDown = player.GetRow() + 1;

            if (controls.onPress(Keys.Right, Buttons.DPadRight) && cursorCol != maxCol && cursorCol < maxRight)
            {
                SetX(GetX() + width);
                cursorCol++;
            }
            else if (controls.onPress(Keys.Left, Buttons.DPadLeft) && cursorCol != 0 && cursorCol > maxLeft)
            {
                SetX(GetX() - width);
                cursorCol--;
            }
            else if (controls.onPress(Keys.Up, Buttons.DPadUp) && cursorRow != 0 && cursorRow > maxUp)
            {
                SetY(GetY() - height);
                cursorRow--;
            }
            else if (controls.onPress(Keys.Down, Buttons.DPadDown) && cursorRow != maxRow && cursorRow < maxDown)
            {
                SetY(GetY() + height);
                cursorRow++;
            }

            //Get the occupant of the selected tile
            MapTile tile = map.GetTile(cursorRow, cursorCol);
            Unit occupant = tile.GetOccupant();

            //If the player chooses a tile and there is a valid ability target...
            if (controls.onPress(Keys.Space, Buttons.A) && occupant != null)
            {
                //...use ability on that target
                //Can only feed on sleeping villagers
                if(occupant.GetType() == typeof(SleepingVillager))
                {
                    SleepingVillager villager = (SleepingVillager)occupant;
                    //Villager must not already have been drained
                    if(!villager.IsDrained())
                    {
                        //Give the player blood
                        player.SetHasBlood(true);
                        //Update the villager as drained
                        villager.SetDrained(true);
                        tile.SetOccupant(villager);
                    }
                    
                }
                //Update that player has used an ability this turn
                player.SetUsedAbilityThisTurn(true);
                gameManager.SetChoosingAbilityTarget(false);
            }
            //If player cancels the ability select, exit ability select mode
            else if (controls.onPress(Keys.F, Buttons.B))
            {
                gameManager.SetChoosingAbilityTarget(false);
            }
        }

        /*
        public void SelectAction(Controls controls)
        {
            //Select an action
            //Only option right now is move
            if (controls.onPress(Keys.Space, Buttons.A))
            {
                gst.SetMovingPlayer(true);
                gst.SetInActionMenu(false);
            }
            else if (controls.onPress(Keys.F, Buttons.B))
            {
                gst.SetInActionMenu(false);
            }
        }
         * */
    }
}
