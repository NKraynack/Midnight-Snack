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
    public class MenuSelector : GameObject
    {
        protected int selectorRow;    //Which row of the Menu is the selector in
        protected int maxRow; //Bottom-most row of the Menu
        protected Vector2 initialPos;

        GameManager gst = GameManager.GetInstance();
        Player player = Player.GetInstance();

        public MenuSelector(Vector2 pos, int width, int height, int rows) : base(pos, width, height)
        {
            initialPos = pos;
            selectorRow = 0;
            maxRow = rows - 1;
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("selector");
        }

        public virtual void Update(Controls controls)
        {
            SetY((int)initialPos.Y + selectorRow * 20);

            //selector should navigate menu
            Move(controls);
            SelectAction(controls);
        }

        public void Move(Controls controls)
        {
            if (controls.onPress(Keys.Up, Buttons.DPadUp) && selectorRow != 0)
            {
                selectorRow--;
            }
            else if (controls.onPress(Keys.Down, Buttons.DPadDown) && selectorRow != maxRow)
            {
                selectorRow++;
            }
        }

        public int SelectAction(Controls controls)
        {
            //Select an action
            if(controls.onPress(Keys.Space, Buttons.A))
            {
                return selectorRow;
            }
            return -1;
        }

        public bool SetRow(int row)
        {
            if(row <= maxRow)
            {
                selectorRow = row;
            }
            return false;
        }
    }
}
