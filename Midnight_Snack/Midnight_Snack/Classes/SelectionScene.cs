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

        private bool briefingScreen;
        private Text briefingText;
        private Texture2D briefingImage;
        private List<Texture2D> briefingImages;

        GameManager gameManager = GameManager.GetInstance();

        public SelectionScene(List<Text> t, Menu menu)
        {
            text = t;
            optionMenu = menu;
            briefingScreen = false;
            briefingImages = new List<Texture2D>();

            briefingText = new Text("", new Vector2(GameRunner.ScreenWidth * 1 / 6, GameRunner.ScreenHeight * 1 / 6));
            text.Add(briefingText);
        }

        public override void LoadContent(ContentManager content)
        {
            //Load background content
            //background = content.Load<Texture2D>("goomba");

            //Load all briefing images
            briefingImages.Add(content.Load<Texture2D>("tutorial_briefing"));
            briefingImages.Add(content.Load<Texture2D>("level1_briefing"));
            briefingImages.Add(content.Load<Texture2D>("level2_briefing"));
            briefingImages.Add(content.Load<Texture2D>("level3_briefing"));

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
                loadLevelBriefing(gameManager.GetCurrentLevel());
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw the background
            //spriteBatch.Draw(background, new Rectangle(0, 0, GameRunner.ScreenWidth, GameRunner.ScreenHeight), Color.White);

            if (briefingScreen)
            {
                if (briefingImage != null)
                {
                    //Calculate remaining screen space
                    int remainingSpace = GameRunner.ScreenWidth - briefingImage.Width;
                    //Draw the briefing image
                    spriteBatch.Draw(briefingImage, new Rectangle(remainingSpace / 2, 250, briefingImage.Width, briefingImage.Height), Color.White);
                }
            }

            //Draw all text content
            for (int i = 0; i < text.Count; i++)
            {
                text[i].Draw(spriteBatch);
            }
            //Draw the menu content
            optionMenu.Draw(spriteBatch);
        }

        public void loadLevelBriefing(int level)
        {
            text.Clear();
            //Text briefingText = new Text("", new Vector2(GameRunner.ScreenWidth * 1/6, GameRunner.ScreenHeight * 1 / 6));
            switch(level)
            {
                //Tutorial Briefing Text
                case 0:
                    briefingText.SetMessage("You are one of the undead; a vampire! To sustain yourself you must feed on the blood of the living. \n Venture out into town and drink the blood of a slumbering villager. \n The guards are unlikely to look kindly on your nocturnal activities, so deal with them as you see fit. \n Just make sure to get back to your lair before sunrise, or you'll be turned to ash by the sun's harsh light!");
                    briefingImage = briefingImages[0];
                    break;
                case 1:
                    briefingText.SetMessage("The villagers have recruited a cleric to ward off vampires. Don't let divine interference get between you and dinner!");
                    briefingImage = briefingImages[1];
                    break;
                case 2:
                    briefingText.SetMessage("Oh no! A rival vampire is in town! Feed on a villager before your gluttonous rival drains them all!");
                    briefingImage = briefingImages[2];
                    break;
                case 3:
                    briefingText.SetMessage("All this vampiric activity has attracted vampire hunters. A smart vampire picks his fights wisely.");
                    briefingImage = briefingImages[3];
                    break;
                default:
                    break;


            }

            text.Add(briefingText);
        }

        public bool IsBriefingScreen()
        {
            return briefingScreen;
        }

        public void SetBriefingScreen(bool b)
        {
            briefingScreen = b;
        }
    }
}
