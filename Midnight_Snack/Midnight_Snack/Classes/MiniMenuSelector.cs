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
    public class MiniMenuSelector : MenuSelector
    {
        GameManager gameManager = GameManager.GetInstance();
        Player player = Player.GetInstance();

        public MiniMenuSelector(Vector2 playerPos, int width, int height, int rows)
            : base(playerPos, width, height, rows)
        {
            initialPos = playerPos;
            initialPos.X += 70;
            initialPos.Y += 15;

            position = playerPos;
            position.X += 70;
            position.Y += 15;

            selectorRow = 0;
            maxRow = rows - 1;
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("action_menu_selector.png");
        }
        
        public override void Update(Controls controls)
        {
            //Update selector location
            position = player.GetPosition();
            position.X += 70;
            position.Y += 15;
            initialPos = player.GetPosition();
            initialPos.X += 70;
            initialPos.Y += 15;
            SetY((int)initialPos.Y + selectorRow * 20);

            //If in the action menu, selector should navigate menu
            if (gameManager.IsInActionMenu())
            {
                Move(controls);
                SelectAction(controls);
            }
            //If player wants to cancel out of selecting an action
            if (controls.onPress(Keys.F, Buttons.B))
            {
                if (gameManager.IsInActionMenu())
                {
                    if (gameManager.IsInAbilitiesMenu())
                    {
                        gameManager.SetInAbilitiesMenu(false);
                    }
                    else
                    {
                        gameManager.SetInActionMenu(false);
                    }
                }
            }
        }
        
        /**
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
         * */

        /*
        public override int SelectAction(Controls controls)
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
         * */
    }
}
