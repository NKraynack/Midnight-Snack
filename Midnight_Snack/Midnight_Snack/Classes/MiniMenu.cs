using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Midnight_Snack
{
    public class MiniMenu : GameObject
    {
        private bool visible;   //Is the mini menu currently visible?
        private bool movesWithPlayer;   //Does the mini menu move around with the player character?
        private MiniMenuSelector selector;  //The selector associated with this menu
        private List<PlayerActionText> menuOptions; //The options of the menu

        private PlayerActionText moveText;    //The text which displays the move action
        private PlayerActionText interactText;    //The text which displays the interact action
        private PlayerActionText endTurnText;    //The text which displays the end turn action

        Player player = Player.GetInstance();

        public MiniMenu(Vector2 playerPos, int width, int height) : base(playerPos, width, height)
        {
            visible = true;
            selector = new MiniMenuSelector(playerPos, width, 15);

            position = playerPos;
            position.X += 70;
            position.Y += 15;

            /*
            menuOptions = options;
            int xOffset = 0;
            int yOffset = 0;
            for(int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].SetPosition(new Vector2(xOffset, yOffset));
                yOffset += 20;
            }
             * */

            moveText = new PlayerActionText("Move", position, 0, 0);
            interactText = new PlayerActionText("Interact", position, 0, 20);
            endTurnText = new PlayerActionText("End Turn", position, 0, 45);
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("action_menu_background.png");
            selector.LoadContent(content);
            /*
            for(int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].LoadContent(content);
            }
             * */

            moveText.LoadContent(content);
            interactText.LoadContent(content);
            endTurnText.LoadContent(content);

        }

        public void Update(Controls controls)
        {
            //Update Menu Background Position
            position = player.GetPosition();
            position.X += 70;
            position.Y += 15;

            //Update Selector and text
            selector.Update(controls);
            //Gray out move option if already used this turn
            if (player.HasMovedThisTurn())
            {
                moveText.SetAvailable(false);
            }
            else
            {
                moveText.SetAvailable(true);
            }
            //moveText.Update(player.GetPosition());
            //Gray out interact option if already used this turn
            if (player.HasUsedAbilityThisTurn())
            {
                interactText.SetAvailable(false);
            }
            else
            {
                interactText.SetAvailable(true);
            }

            /*
            for (int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].Update(player.GetPosition());
            }
             * */

            moveText.Update(player.GetPosition());
            interactText.Update(player.GetPosition());
            endTurnText.Update(player.GetPosition());
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, position, Color.White);
                selector.Draw(spriteBatch);
                /*
                for (int i = 0; i < menuOptions.Count; i++)
                {
                    menuOptions[i].Draw(spriteBatch);
                }
                 * */
               
                moveText.Draw(spriteBatch);
                interactText.Draw(spriteBatch);
                endTurnText.Draw(spriteBatch);
                
            }
        }

        public void SetVisible(bool b)
        {
            visible = b;
        }
    }
}
