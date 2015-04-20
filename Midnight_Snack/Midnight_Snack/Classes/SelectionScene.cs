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
        private int numPages;
        private int currentPage;

        GameManager gameManager = GameManager.GetInstance();

        public SelectionScene(List<Text> t, Menu menu)
        {
            text = t;
            optionMenu = menu;
            briefingScreen = false;
            briefingImages = new List<Texture2D>();
            numPages = 1;
            currentPage = 1;

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
            briefingImages.Add(content.Load<Texture2D>("shapeshifting_briefing"));

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

            if (gameManager.GetGameState() == 4)
            {
                loadLevelBriefing(gameManager.GetCurrentLevel());
            }
            
            //For briefing screens only
            if (briefingScreen)
            {
                //Go to next briefing page
                if (controls.onPress(Keys.Space, Buttons.A))
                {
                    currentPage++;
                }

                //Start level if seen all the briefing pages
                if (currentPage > numPages) 
                {
                    gameManager.SetGameState(1);
                    currentPage = 1;
                }
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
                //Basic Tutorial Briefing Text
                case 0:
                    numPages = 1;
                    briefingText.SetMessage("You are one of the undead; a vampire! To sustain yourself you must feed on the blood of the living. \n Venture out into town and drink the blood of a slumbering villager. \n The guards are unlikely to look kindly on your nocturnal activities, so deal with them as you see fit. \n Just make sure to get back to your lair before sunrise, or you'll be turned to ash by the sun's harsh light!");
                    briefingImage = briefingImages[0];
                    break;
                //Enemy Tutorial Briefing Text
                case 1:
                    numPages = 1;
                    briefingText.SetMessage("Last night's feasting has put the guards on alert. You'll have to deal with any guards that try and get between you and dinner!");
                    briefingImage = briefingImages[0];
                    break;
                //Tutorial Briefing Text
                case 2:
                    numPages = 2;
                    briefingText.SetMessage("You are one of the undead; a vampire! To sustain yourself you must feed on the blood of the living. \n Venture out into town and drink the blood of a slumbering villager. \n The guards are unlikely to look kindly on your nocturnal activities, so deal with them as you see fit. \n Just make sure to get back to your lair before sunrise, or you'll be turned to ash by the sun's harsh light!");
                    briefingImage = briefingImages[0];
                    if (currentPage == 2)
                    {
                        briefingImage = briefingImages[4];
                    }
                    break;
                //Level 1 Briefing Text
                case 3:
                    numPages = 1;
                    briefingText.SetMessage("The villagers have recruited a cleric to ward off vampires. Don't let divine interference get between you and dinner!");
                    briefingImage = briefingImages[1];
                    break;
                //Level 2 Briefing Text
                case 4:
                    numPages = 1;
                    briefingText.SetMessage("Oh no! A rival vampire is in town! Feed on a villager before your gluttonous rival drains them all!");
                    briefingImage = briefingImages[2];
                    break;
                //Level 3 Briefing Text
                case 5:
                    numPages = 1;
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

        public int GetNumPages()
        {
            return numPages;
        }

        public void SetNumPages(int pages)
        {
            numPages = pages;
        }
    }
}
