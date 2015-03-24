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
    public class Unit : GameObject
    {
        //protected Map currentMap;   //The map the unit is on
        protected int mapRow;   //The row of the grid which the unit is currently on
        protected int mapCol;   //The column of the grid which the unit is currently on
        protected bool unitsTurn;   //Is it the unit's turn? (This should always be false for units that don't act)
        protected bool hasEndedTurn;    //Has the unit already ended their turn this round?

        public Unit(Vector2 pos, int width, int height, int row, int col) : base(pos, width, height)
        {
            //currentMap = map;
            mapRow = row;
            mapCol = col;
            unitsTurn = false;
            hasEndedTurn = false;
        }

        public int GetRow()
        {
            return mapRow;
        }

        public void SetRow(int row)
        {
            mapRow = row;
        }

        public int GetCol()
        {
            return mapCol;
        }

        public void SetCol(int col)
        {
            mapCol = col;
        }

        public bool IsUnitsTurn()
        {
            return unitsTurn;
        }

        public void SetUnitsTurn(bool b)
        {
            unitsTurn = b;
        }

        public bool GetHasEndedTurn()
        {
            return hasEndedTurn;
        }

        public void SetHasEndedTurn(bool b)
        {
            hasEndedTurn = b;
        }

        public override void LoadContent(ContentManager content)
        {

        }

        public virtual void Update()
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
