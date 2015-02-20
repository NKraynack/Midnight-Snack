using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CS_4730_Prototype
{
    public class MiniMenu : GameObject
    {
        private bool visible;   //Is the mini menu currently visible?
        private MenuSelector selector;  //The selector associated with this menu
        private PlayerActionText moveText;    //The text which displays the move action
        private PlayerActionText interactText;    //The text which displays the interact action
        private PlayerActionText endTurnText;    //The text which displays the end turn action

        Player player = Player.GetInstance();

        public MiniMenu(Vector2 playerPos, int width, int height) : base(playerPos, width, height)
        {
            visible = true;
            selector = new MenuSelector(playerPos, width, 15);

            Position = playerPos;
            Position.X += 70;
            Position.Y += 15;

            moveText = new PlayerActionText("Move", Position, 0, 0);
            interactText = new PlayerActionText("Interact", Position, 0, 20);
            endTurnText = new PlayerActionText("End Turn", Position, 0, 45);
        }

        public void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("action_menu_background.png");
            selector.LoadContent(content);
            moveText.LoadContent(content);
            interactText.LoadContent(content);
            endTurnText.LoadContent(content);
        }

        public void Update(Controls controls)
        {
            //Update Menu Background Position
            Position = player.GetPosition();
            Position.X += 70;
            Position.Y += 15;

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
            moveText.Update(player.GetPosition());
            //Gray out interact option if already used this turn
            if (player.HasInteractedThisTurn())
            {
                interactText.SetAvailable(false);
            }
            else
            {
                interactText.SetAvailable(true);
            }
            interactText.Update(player.GetPosition());
            endTurnText.Update(player.GetPosition());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(Texture, Position, Color.White);
                selector.Draw(spriteBatch);
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
