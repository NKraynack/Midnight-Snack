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
    public class Player : MobileUnit
    {
        private bool hasBlood;   //Has the player collected blood
        private string form;    //The form the player is currently in
        private bool cur_drawn = false; //if drawn
        private bool prev_drawn = true; //if undrawn these two are used to track 

        private Texture2D wolfTexture;  //The texture for wolf form
        private Texture2D mistTexture;  //The texture for mist form

        Map map = Map.GetInstance();
        GameManager gameManager = GameManager.GetInstance();

        private static Player instance;

        public Player(Vector2 pos, int width, int height, int row, int col, int range, int health) 
            : base(pos, width, height, row, col, range, health)
        {
            hasBlood = false;
            form = "vampire";
        }

        public static Player GetInstance()
        {
            if (instance == null)
            {
                instance = new Player(new Vector2(0, 0), 100, 100, 0, 0, 3, 10);
            }
            return instance;
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("player_vampire");
            wolfTexture = content.Load<Texture2D>("wolf");
            mistTexture = content.Load<Texture2D>("mist");
            healthBar.LoadContent(content);
            stats.LoadContent(content);
        }

        public override void Update()
        {
            base.Update();

            if(!alive)
            {
                GameManager.GetInstance().SetPlayerAlive(false);
            }

        }

        public bool HasBlood()
        {
            return hasBlood;
        }

        public void SetHasBlood(bool b)
        {
            hasBlood = b;
        }

        public string GetForm()
        {
            return form;
        }

        public void SetForm(string f)
        {
            form = f;

            //Update move range according to form
            if (form.Equals("wolf"))
            {
                this.SetMoveRange(5);
            }
            else if (form.Equals("mist"))
            {
                this.SetMoveRange(2);
            }
            else
            {
                this.SetMoveRange(3);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            healthBar.Draw(spriteBatch);
            stats.Draw(spriteBatch);

            Color spriteColor = Color.White;
            //Tinge the player red if they have blood
            if (hasBlood)
            {
                spriteColor = Color.Red;
            }

            //Draw the player in the appropriate form
            if (form.Equals("mist"))
            {
                spriteBatch.Draw(mistTexture, position, spriteColor);
            }
            else if (form.Equals("wolf"))
            {
                spriteBatch.Draw(wolfTexture, position, spriteColor);
            }
            else
            {
                spriteBatch.Draw(texture, position, spriteColor);
            }

        }

        public void DrawMoveRange(bool undraw)
        {
            if (cur_drawn == false && undraw == false)
            {
                cur_drawn = true;
            }
            else if (cur_drawn == true && undraw == true)
            {
                cur_drawn = false;
            }
            if (cur_drawn != prev_drawn)
            {
                //pass
            }
            else if (gameManager.IsChoosingAbilityTarget())
            {
                prev_drawn = !cur_drawn;
                for (int x = this.GetCol() - 1; x <= this.GetCol() + 1; x++)
                {
                    for (int y = this.GetRow() - 1; y <= this.GetRow() + 1; y++)
                    {
                        try
                        {
                                MapTile tile = map.GetTile(y, x);
                                if (undraw)
                                {
                                    tile.SetLit(false);
                                }
                                else if (!undraw)
                                {
                                    tile.SetLit(true);
                                }
                                map.SetTile(y, x, tile);
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            //pass through out of range index because im lazy to bounds check
                        }
                    }
                }
            }
            else
            {
                prev_drawn = !cur_drawn;
                for (int x = this.GetCol() - this.GetMoveRange(); x <= this.GetCol() + this.GetMoveRange(); x++)
                {
                    for (int y = this.GetRow() - this.GetMoveRange(); y <= this.GetRow() + this.GetMoveRange(); y++)
                    {
                        try
                        {
                            //Console.WriteLine("checking: " + x + " " + y);
                            //Console.WriteLine(this.GetForm());
                            //Console.WriteLine(Math.Abs(y - this.GetRow()) + Math.Abs(x - this.GetCol()) <= this.GetMoveRange());
                            //Console.WriteLine((this.NoObstacles(x, y) || this.GetForm().Equals("mist")));
                            //Console.WriteLine((x != this.GetCol() || y != this.GetRow()));
                            if ((Math.Abs(y - this.GetRow()) + Math.Abs(x - this.GetCol()) <= this.GetMoveRange() && (this.NoObstacles(x, y) || this.GetForm().Equals("mist"))
                                && (x != this.GetCol() || y != this.GetRow()) && map.GetTile(y, x).IsPassable()))
                            {
                                MapTile tile = map.GetTile(y, x);
                                if (undraw)
                                {
                                    tile.SetLit(false);
                                }
                                else if (!undraw)
                                {
                                    tile.SetLit(true);
                                }
                                map.SetTile(y, x, tile);
                            }
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            //pass through out of range index because im lazy to bounds check
                        }
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
                    foreach (GridPoint node in getNeighbors(map.GetNumCols(), map.GetNumRows(), current, map.GenerateMapGrid()))
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



    }
}
