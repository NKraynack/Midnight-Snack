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
    public class MiniMenuSelector : GameObject
    {
        private int selectorRow;    //Which row of the MiniMenu is the selector in
        private int maxRow; //Bottom-most row of the MiniMenu

        GameManager gst = GameManager.GetInstance();
        Player player = Player.GetInstance();

        public MiniMenuSelector(Vector2 playerPos, int width, int height) : base(playerPos, width, height)
        {
            position = playerPos;
            position.X += 70;
            position.Y += 15;

            selectorRow = 0;
            maxRow = 2;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("action_menu_selector.png");
        }

        public void Update(Controls controls)
        {
            //Update selector location
            if (selectorRow == 0)
            {
                SetX(player.GetX() + 70);
                SetY(player.GetY() + 15);
            }
            else if (selectorRow == 1)
            {
                SetX(player.GetX() + 70);
                SetY(player.GetY() + 40);
            }
            else if (selectorRow == 2)
            {
                SetX(player.GetX() + 70);
                SetY(player.GetY() + 65);
            }

            //If in the action menu, selector should navigate menu
            if (gst.IsInActionMenu())
            {
                Move(controls);
                SelectAction(controls);
            }
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

        public void SelectAction(Controls controls)
        {
            //Select an action
            if (controls.onPress(Keys.Space, Buttons.A))
            {
                //Chose move if haven't already moved this turn
                if(selectorRow == 0 && !player.HasMovedThisTurn())
                {
                    gst.SetMovingPlayer(true);
                    gst.SetInActionMenu(false);
                    gst.SetChoosingInteractTarget(false);
                }
                //Chose interact if haven't already interacted this turn
                else if(selectorRow == 1 && !player.HasUsedAbilityThisTurn())
                {
                    gst.SetChoosingInteractTarget(true);
                    gst.SetMovingPlayer(false);
                    gst.SetInActionMenu(false);
                }
                //Chose to end turn
                else if(selectorRow == 2)
                {
                    //Increment turn counter
                    int nextTurnNum = gst.GetTurn() + 1;
                    gst.SetTurn(nextTurnNum);
                    
                    //Update player status
                    player.SetMovedThisTurn(false);
                    player.SetUsedAbilityThisTurn(false);

                    gst.SetChoosingInteractTarget(false);
                    gst.SetMovingPlayer(false);
                    gst.SetInActionMenu(false);

                    //Move selector back to top of menu
                    selectorRow = 0;
                }
            }
            //Cancel out of select an action
            else if (controls.onPress(Keys.F, Buttons.B))
            {
                gst.SetInActionMenu(false);
            }
        }
    }
}
