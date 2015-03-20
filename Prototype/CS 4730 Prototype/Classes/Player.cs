using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CS_4730_Prototype
{
    public class Player : GameObject
    {
        private Map map;    //The map the player is on
        private int playerRow;  //The row of the grid which the player is currently on
        private int playerCol;  //The col of the grid which the player is currently on
        private int moveRange;  //Number of tiles the player can move in one turn
        private bool hasBlood;   //Has the player collected blood
        private bool movedThisTurn;  //Has the player moved this turn
        private bool interactedThisTurn; //Has the player interacted this turn

        private static Player instance = new Player(0, 0, 100, 100);

        public Player(int x, int y, int width, int height) : base(x, y, width, height)
        {
            playerRow = 0;
            playerCol = 0;
            moveRange = 3;
            hasBlood = false;
            movedThisTurn = false;
            interactedThisTurn = false;
        }

        public Player(int x, int y, int width, int height, Map map) : base(x, y, width, height)
        {
            this.map = map;
            playerRow = map.GetLairRow();
            playerCol = map.GetLairCol();
            moveRange = 3;
            hasBlood = false;
            movedThisTurn = false;
            interactedThisTurn = false;
        }

        public Player(Vector2 pos, int width, int height, Map map) : base(pos, width, height)
        {
            this.map = map;
            playerRow = map.GetLairRow();
            playerCol = map.GetLairCol();
            moveRange = 3;
            hasBlood = false;
            movedThisTurn = false;
            interactedThisTurn = false;
        }

        public static Player GetInstance()
        {
            return instance;
        }

        public void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("dracula.png");
        }

        //Moves the player to the given position
        public void Move(int x, int y, int row, int col)
        {
            SetX(x);
            SetY(y);
            SetRow(row);
            SetCol(col);
        }

        //Feed on an adjacent sleeping villager and gain blood
        public void Feed()
        {
            hasBlood = true;
        }

        //Tinge the player red if they have blood
        public void Draw(SpriteBatch spriteBatch)
        {
            if (hasBlood)
            {
                spriteBatch.Draw(Texture, Position, Color.Red);
            }
            else
            {
                spriteBatch.Draw(Texture, Position, Color.White);
            }
        }

        public Map GetMap()
        {
            return map;
        }

        public void SetMap(Map m)
        {
           this.map = m;
        }

        //Returns the row of the grid the player is in
        public int GetRow()
        {
            return playerRow;
        }

        //Returns the column of the grid the player is in
        public int GetCol()
        {
            return playerCol;
        }

        //Sets the row the player is in
        public void SetRow(int row)
        {
            playerRow = row;
        }

        //Sets the column the player is in
        public void SetCol(int col)
        {
            playerCol = col;
        }

        //Returns the player's move range
        public int GetMoveRange()
        {
            return moveRange;
        }

        public bool HasBlood()
        {
            return hasBlood;
        }

        public bool HasMovedThisTurn()
        {
            return movedThisTurn;
        }

        public void SetMovedThisTurn(bool b)
        {
            movedThisTurn = b;
        }

        public bool HasInteractedThisTurn()
        {
            return interactedThisTurn;
        }

        public void SetInteractedThisTurn(bool b)
        {
            interactedThisTurn = b;
        }
    }
}
