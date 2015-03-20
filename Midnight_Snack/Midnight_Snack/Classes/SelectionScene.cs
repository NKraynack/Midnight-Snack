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
    public class SelectionScene : Scene
    {
        public Menu optionMenu;

        public SelectionScene(List<Text> t, Menu menu)
        {
            text = t;
            optionMenu = menu;
        }

        public void LoadContent(ContentManager content)
        {
            //Load all text content
            for(int i = 0; i < text.Count; i++)
            {
                text[i].LoadContent(content);
            }
            //Load all menu content
            optionMenu.LoadContent(content);
        }

        public void Update(Controls controls)
        {
            optionMenu.Update(controls);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw all text content
            for (int i = 0; i < text.Count; i++)
            {
                text[i].Draw(spriteBatch);
            }
            //Draw the menu content
            optionMenu.Draw(spriteBatch);
        }


    }
}
