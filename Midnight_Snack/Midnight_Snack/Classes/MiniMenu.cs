using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Midnight_Snack
{
    public class MiniMenu : Menu
    {
        private MiniMenuSelector selector;  //The selector associated with this menu

        private PlayerActionText moveText;    //The text which displays the move action
        private PlayerActionText interactText;    //The text which displays the interact action
        private PlayerActionText endTurnText;    //The text which displays the end turn action

        Player player = Player.GetInstance();
        GameManager gameManager = GameManager.GetInstance();

        public MiniMenu(Vector2 playerPos, int width, int height, List<Text> options) : base(playerPos, width, height, options)
        {
            visible = false;
            selector = new MiniMenuSelector(playerPos, width, 15, options.Count);

            position = playerPos;
            position.X += 70;
            position.Y += 15;
                
            menuOptions = options;
            UpdateMenuText();
                

            moveText = new PlayerActionText("Move", position, 0, 0);
            interactText = new PlayerActionText("Interact", position, 0, 20);
            endTurnText = new PlayerActionText("End Turn", position, 0, 45);
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("action_menu_background.png");
            selector.LoadContent(content);

            for (int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].LoadContent(content);
            }
        }

        public void UpdateMenuText()
        {
            //Update text position
            int xOffset = 0;
            int yOffset = 0;
            for (int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].SetPosition(new Vector2(GetX() + xOffset, GetY() + yOffset));
                yOffset += 20;

                //Update whether option is available or not
                if(menuOptions[i].GetMessage().Equals("Move"))
                {
                    //Gray out move option if already used this turn
                    if (player.HasMovedThisTurn())
                    {
                        menuOptions[i].SetAvailable(false);
                    }
                    else
                    {
                        menuOptions[i].SetAvailable(true);
                    }
                }
                if(menuOptions[i].GetMessage().Equals("Interact"))
                {
                    //Gray out interact option if already used this turn
                    if (player.HasUsedAbilityThisTurn())
                    {
                        menuOptions[i].SetAvailable(false);
                    }
                    else
                    {
                        menuOptions[i].SetAvailable(true);
                    }
                }
            }
        }

        public override void Update(Controls controls)
        {
            //Update Menu Background Position
            position = player.GetPosition();
            position.X += 70;
            position.Y += 15;

            //Update Selector and text
            selector.Update(controls);
            UpdateMenuText();

            int option = selector.SelectAction(controls);
            PerformAction(GetOptionSelected(option));
        }

        public override void PerformAction(string action)
        {
            if(action.Equals("Move"))
            {
                //Go into move mode
                //Move if haven't already moved this turn
                if (!player.HasMovedThisTurn())
                {
                    gameManager.SetMovingPlayer(true);
                    gameManager.SetInActionMenu(false);
                    gameManager.SetChoosingInteractTarget(false);
                }
            }
            else if(action.Equals("Interact"))
            {
                //Go into interact mode
                //Interact if haven't already interacted this turn
                if(!player.HasUsedAbilityThisTurn())
                {
                    gameManager.SetChoosingInteractTarget(true);
                    gameManager.SetMovingPlayer(false);
                    gameManager.SetInActionMenu(false);
                }
            }
            else if(action.Equals("End Turn"))
            {
                //Increment turn counter
                int nextTurnNum = gameManager.GetTurn() + 1;
                gameManager.SetTurn(nextTurnNum);
                    
                //Update player status
                player.SetMovedThisTurn(false);
                player.SetUsedAbilityThisTurn(false);

                gameManager.SetChoosingInteractTarget(false);
                gameManager.SetMovingPlayer(false);
                gameManager.SetInActionMenu(false);

                //Move selector back to top of menu
                selector.SetRow(0);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(visible)
            {
                spriteBatch.Draw(texture, position, Color.White);
                selector.Draw(spriteBatch);
                
                for(int i = 0; i < menuOptions.Count; i++)
                {
                    menuOptions[i].Draw(spriteBatch);
                }
            }
        }
    }
}
