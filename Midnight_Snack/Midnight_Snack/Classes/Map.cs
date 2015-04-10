using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Midnight_Snack
{
    public class Map
    {
        private int rows;   //Number of rows in the grid
        private int cols;   //Number of columns in the grid
        private MapTile[,] grid;    //2D array representing the map
        private int lairRow;    //The row in which the lair is located
        private int lairCol;    //The column in which the lair is located

        private static Map instance;

        //Creates a map of the given number of rows and columns
        //with a starting position at [startRow, startCol]
        public Map(int numRows, int numCols, int startRow, int startCol)
        {
            rows = numRows;
            cols = numCols;
            this.grid = new MapTile[numCols, numRows];
            this.FillGrid(numRows, numCols);
            SetLair(startRow, startCol);
        }

        //public void Update()
        //{
        //    grid = this.FillGrid(rows, cols);
        //}

        public static Map GetInstance()
        {
            if (instance == null)
            {
                instance = new Map(1, 1, 0, 0);
            }
            return instance;
        }

        public void SetRows(int numRows)
        {
            this.rows = numRows;
        }

        public void SetCols(int numCols)
        {
            this.cols = numCols;
        }

        //Returns a grid with the given number of rows and cols
        //with MapTiles in each square of the grid
        public void FillGrid(int numRows, int numCols)
        {
            this.grid = new MapTile[numRows, numCols];

            //Get width of map in pixels
            int mapWidth = this.GetNumCols() * 100;
            //Calculate remaining screen space
            int remainingSpace = GameRunner.ScreenWidth - mapWidth;
            //Calculate where map should start to be centered on screen

            int xOffset = remainingSpace / 2;
            int yOffset = 30;
            int tilesize = 100;

            for (int r = 0; r < numRows; r++)
            {
                for (int c = 0; c < numCols; c++)
                {
                    int x = xOffset + (c * tilesize);
                    int y = yOffset + (r * tilesize);
                    MapTile tile = new MapTile(x, y, tilesize, tilesize);
                    this.grid[r, c] = tile;
                }
            }
        }

        //Generate map for movement
        public char[,] GenerateMapGrid()
        {
            char[,] grid = new char[cols, rows];
            for(int i = 0; i < cols; i++) {
                for (int j = 0; j < rows; j++) {
                    grid[i,j] = GetTile(j,i).IsPassable() ? 'o' : 'x';
                }
            }
            return grid;
        }

        //Loads the content for all the MapTiles of the Map
        public void LoadContent(ContentManager content)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    grid[r, c].LoadContent(content);
                }
            }
        }

        //Draws all the MapTiles which comprise the Map
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    grid[r, c].Draw(spriteBatch);
                }
            }
        }

        //Returns the number of rows in the grid
        public int GetNumRows()
        {
            return rows;
        }

        //Returns the number of columns in the grid
        public int GetNumCols()
        {
            return cols;
        }

        //Returns the tile in the given row and column
        public MapTile GetTile(int row, int col)
        {
            return grid[row, col];
        }

        //Sets the tile in the given row and column to given tile
        public void SetTile(int row, int col, MapTile tile)
        {
            grid[row, col] = tile;
        }

        //Returns the tile at which the player starts
        public MapTile GetLair()
        {
            return grid[lairRow, lairCol];
        }

        //Returns the (x,y) position of the lair
        public Vector2 GetLairPos()
        {
            return GetLair().GetPosition();
        }

        //Returns the row of the grid where the lair is
        public int GetLairRow()
        {
            return lairRow;
        }

        //Returns the column of the grid where the lair is
        public int GetLairCol()
        {
            return lairCol;
        }

        //Sets the lair to the MapTile at [row, col]
        public void SetLair(int row, int col)
        {
            lairRow = row;
            lairCol = col;

            MapTile lairTile = GetLair();
            lairTile.SetModifier("lair");
            SetTile(row, col, lairTile);
        }

        //Returns the grid
        public MapTile[,] GetGrid()
        {
            return this.grid;
        }

        //Sets the grid to the given 2D array
        public void SetGrid(MapTile[,] g)
        {
            this.grid = g;
        }
    }
}
