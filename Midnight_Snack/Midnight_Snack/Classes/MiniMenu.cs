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

        Player player = Player.GetInstance();
        GameManager gameManager = GameManager.GetInstance();
        Map map = Map.GetInstance();

        public MiniMenu(Vector2 playerPos, int width, int height, List<Text> options) : base(playerPos, width, height, options)
        {
            visible = false;
            selector = new MiniMenuSelector(playerPos, width, 15, options.Count);

            position = playerPos;
            position.X += 70;
            position.Y += 15;
                
            menuOptions = options;
            UpdateMenuText();
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("minimenu_background");
            selector.LoadContent(content);

            for (int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].LoadContent(content);
            }
        }

        public void UpdateMenuText()
        {
            //Update text position
            int xOffset = 10;
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
                if(menuOptions[i].GetMessage().Equals("Abilities"))
                {
                    //Gray out abilities option if already used this turn
                    //or if player is on a garlic tile
                    if (player.HasUsedAbilityThisTurn() || player.GetCurrentMapTile().GetModifier().Equals("garlic"))
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
            //Tell the gameManager what ability the player is using
            gameManager.SetPlayerAbility(action);


            if(action.Equals("Move"))
            {
                //Go into move mode
                //Move if haven't already moved this turn
                if (!player.HasMovedThisTurn())
                {
                    gameManager.SetMovingPlayer(true);
                    gameManager.SetInActionMenu(false);
                    gameManager.SetChoosingAbilityTarget(false);
                    player.DrawTile();
                    
                }
            }
            //Can only use abilities if not on a garlic tile
            else if(action.Equals("Abilities") && !player.GetCurrentMapTile().GetModifier().Equals("garlic"))
            {
                //Go into use ability mode
                //Use ability if haven't already done so this turn
                if(!player.HasUsedAbilityThisTurn())
                {
                    gameManager.SetInAbilitiesMenu(true);

                    /*
                    gameManager.SetChoosingAbilityTarget(true);
                    gameManager.SetMovingPlayer(false);
                    gameManager.SetInActionMenu(false);
                     * */
                }
            }
            else if (action.Equals("Feed"))
            {
                gameManager.SetChoosingAbilityTarget(true);
                gameManager.SetMovingPlayer(false);
                gameManager.SetInActionMenu(false);
                gameManager.SetInAbilitiesMenu(false);
            }
            else if (action.Equals("Attack"))
            {
                gameManager.SetChoosingAbilityTarget(true);
                gameManager.SetMovingPlayer(false);
                gameManager.SetInActionMenu(false);
                gameManager.SetInAbilitiesMenu(false);
            }
            else if (action.Equals("Wolf Form"))
            {
                player.SetForm("wolf");
                player.SetUsedAbilityThisTurn(true);
                gameManager.SetMovingPlayer(false);
                gameManager.SetInActionMenu(false);
                gameManager.SetInAbilitiesMenu(false);
            }
            else if (action.Equals("Mist Form"))
            {
                player.SetForm("mist");
                player.SetUsedAbilityThisTurn(true);
                gameManager.SetMovingPlayer(false);
                gameManager.SetInActionMenu(false);
                gameManager.SetInAbilitiesMenu(false);
            }
            else if(action.Equals("End Turn"))
            {
                //Increment turn counter
                int nextTurnNum = gameManager.GetTurn() + 1;
                gameManager.SetTurn(nextTurnNum);
                    
                //Update player status
                player.SetHasEndedTurn(true);
                player.SetMovedThisTurn(false);
                player.SetUsedAbilityThisTurn(false);
                player.SetForm("vampire");

                gameManager.SetChoosingAbilityTarget(false);
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
                //Draw menu background of appropriate size for options
                Vector2 backgroundPos = position;
                for (int i = 0; i < menuOptions.Count; i++)
                {
                    spriteBatch.Draw(texture, backgroundPos, Color.White);
                    backgroundPos.Y += 20;
                }

                selector.Draw(spriteBatch);
                
                for(int i = 0; i < menuOptions.Count; i++)
                {
                    menuOptions[i].Draw(spriteBatch);
                }
            }
        }
    }
}
