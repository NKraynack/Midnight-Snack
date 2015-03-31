using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Midnight_Snack
{
    public class Menu : GameObject
    {
        protected bool visible;   //Is the menu currently visible?
        private MenuSelector selector;  //The selector associated with this menu
        protected List<Text> menuOptions; //The options of the menu

        Player player = Player.GetInstance();
        GameManager gameManager = GameManager.GetInstance();

        public Menu(Vector2 pos, int width, int height, List<Text> options) : base(pos, width, height)
        {
            visible = true;
            selector = new MenuSelector(pos, width, 15, options.Count);

            menuOptions = options;
            int xOffset = 0;
            int yOffset = 0;
            for(int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].SetPosition(new Vector2(GetX() + xOffset, GetY() + yOffset));
                yOffset += 20;
            }

        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("action_menu_background.png");
            selector.LoadContent(content);
            for(int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].LoadContent(content);
            }
        }

        public virtual void Update(Controls controls)
        {
            //Update Selector and text
            selector.Update(controls);

            int option = selector.SelectAction(controls);
            PerformAction(GetOptionSelected(option));
        }

        public string GetOptionSelected(int option)
        {
            if(option > -1 && option <= menuOptions.Count)
            {
                return menuOptions[option].GetMessage();
            }
            return "";
        }

        public virtual void PerformAction(string action)
        {
            if(action.Equals("Start"))
            {
                //Load the level 
                gameManager.SetGameState(1);
            }
            else if(action.Equals("Tutorial"))
            {
                //Show the Tutorial's level briefing
                gameManager.SetCurrentLevel(0);
                gameManager.SetGameState(4);
            }
            else if(action.Equals("Level 1"))
            {
                //Load level 1
                /*
                gameManager.SetCurrentLevel(1);
                gameManager.SetGameState(4);
                 * */
            }
            else if(action.Equals("Try Again"))
            {
                gameManager.ResetGameState();
                //gameManager.SetGameState(1);
            }
            else if(action.Equals("Level Select"))
            {
                gameManager.ResetGameState();
                gameManager.SetGameState(0);
            }
            else if(action.Equals("Next Level"))
            {
                //Go to next level
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                //spriteBatch.Draw(texture, position, Color.White);
                selector.Draw(spriteBatch);
                for (int i = 0; i < menuOptions.Count; i++)
                {
                    menuOptions[i].Draw(spriteBatch);
                }
            }
        }

        public void SetVisible(bool b)
        {
            visible = b;
        }

        public void SetOptions(List<Text> options)
        {
            menuOptions = options;
        }
    }
}
