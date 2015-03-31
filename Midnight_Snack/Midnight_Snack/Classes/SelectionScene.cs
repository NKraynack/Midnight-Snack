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

        private Text briefingText;

        GameManager gameManager = GameManager.GetInstance();

        public SelectionScene(List<Text> t, Menu menu)
        {
            text = t;
            optionMenu = menu;

            briefingText = new Text("", new Vector2(GameRunner.ScreenWidth * 1 / 6, GameRunner.ScreenHeight * 1 / 6));
            text.Add(briefingText);
        }

        public override void LoadContent(ContentManager content)
        {
            //Load all text content
            for(int i = 0; i < text.Count; i++)
            {
                text[i].LoadContent(content);
            }
            //Load all menu content
            optionMenu.LoadContent(content);
        }

        public override void Update(Controls controls)
        {
            optionMenu.Update(controls);

            if(gameManager.GetGameState() == 4)
            {
                loadLevelBriefingText(gameManager.GetCurrentLevel());
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw all text content
            for (int i = 0; i < text.Count; i++)
            {
                text[i].Draw(spriteBatch);
            }
            //Draw the menu content
            optionMenu.Draw(spriteBatch);
        }

        public void loadLevelBriefingText(int level)
        {
            text.Clear();
            //Text briefingText = new Text("", new Vector2(GameRunner.ScreenWidth * 1/6, GameRunner.ScreenHeight * 1 / 6));
            switch(level)
            {
                //Tutorial Briefing Text
                case 0:
                    briefingText.SetMessage("You are one of the undead; a vampire! To sustain yourself you must feed on the blood of the living. \n Venture out into town and drink the blood of a slumbering villager. \n The guards are unlikely to look kindly on your nocturnal activities, so deal with them as you see fit. \n Just make sure to get back to your lair before sunrise, or you'll be turned to ash by the sun's harsh light!");
                break;


            }

            text.Add(briefingText);
        }
    }
}
