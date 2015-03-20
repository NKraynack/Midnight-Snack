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
    public class SleepingVillager : GameObject
    {
        private int villagerRow;  //The row of the grid which the player is currently on
        private int villagerCol;  //The col of the grid which the player is currently on

        public SleepingVillager(int x, int y, int width, int height, int row, int col) : base(x, y, width, height)
        {
            villagerRow = row;
            villagerCol = col;
        }

        public void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("mario.png");
        }

        //Returns the row of the grid the villager is in
        public int GetRow()
        {
            return villagerRow;
        }

        //Returns the column of the grid the villager is in
        public int GetCol()
        {
            return villagerCol;
        }
    }
}
