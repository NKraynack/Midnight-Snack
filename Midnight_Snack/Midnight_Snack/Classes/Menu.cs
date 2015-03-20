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
        private bool visible;   //Is the mini menu currently visible?
        private MenuSelector selector;  //The selector associated with this menu
        private List<Text> menuOptions; //The options of the menu

        Player player = Player.GetInstance();
        GameManager gst = GameManager.GetInstance();

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

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("action_menu_background.png");
            selector.LoadContent(content);
            for(int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].LoadContent(content);
            }
        }

        public void Update(Controls controls)
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

        public void PerformAction(string action)
        {
            if(action.Equals("Tutorial"))
            {
                gst.SetGameState(1);
            }
            if(action.Equals("Level 1"))
            {
                //Load level 1
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, position, Color.White);
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
