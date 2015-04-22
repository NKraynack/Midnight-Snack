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

        private bool levelSelectScreen;
        private bool briefingScreen;
        private Text briefingText;
        private Texture2D briefingImage;
        private List<Texture2D> images;
        private int numPages;
        private int currentPage;

        GameManager gameManager = GameManager.GetInstance();

        public SelectionScene(List<Text> t, Menu menu)
        {
            text = t;
            optionMenu = menu;
            briefingScreen = false;
            levelSelectScreen = false;
            images = new List<Texture2D>();
            numPages = 1;
            currentPage = 1;

            briefingText = new Text("", new Vector2(GameRunner.ScreenWidth * 1 / 6, GameRunner.ScreenHeight * 1 / 6));
            text.Add(briefingText);
        }

        public override void LoadContent(ContentManager content)
        {
            //Load background content
            background = content.Load<Texture2D>("level_select_background");

            //Load all briefing images
            images.Add(content.Load<Texture2D>("tutorial_briefing"));
            images.Add(content.Load<Texture2D>("shapeshifting_briefing"));
            images.Add(content.Load<Texture2D>("level1_briefing"));
            images.Add(content.Load<Texture2D>("level2_briefing"));
            images.Add(content.Load<Texture2D>("level3_briefing"));
            images.Add(content.Load<Texture2D>("level4_briefing"));
            images.Add(content.Load<Texture2D>("title"));
            images.Add(content.Load<Texture2D>("guard_briefing"));

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
            if (levelSelectScreen)
            {
                //Draw the background
                //spriteBatch.Draw(background, new Rectangle(0, 0, GameRunner.ScreenWidth, 300), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
                spriteBatch.Draw(background, Vector2.Zero, new Rectangle(0, 0, GameRunner.ScreenWidth, 300), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.End();
                spriteBatch.Begin();
                //Draw the title
                Texture2D title = images[6];
                int remainingSpace = GameRunner.ScreenWidth - title.Width;
                spriteBatch.Draw(title, new Rectangle(remainingSpace / 2, 0, title.Width, title.Height), Color.White);
            }

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
                    briefingText.SetMessage("You are one of the undead; a vampire! To sustain yourself you must feed on the blood of the living. \n Venture out into town and drink the blood of a slumbering villager. \n Just make sure to get back to your lair before sunrise, or you'll be turned to ash by the sun's harsh light!");
                    briefingImage = images[0];
                    break;
                //Enemy Tutorial Briefing Text
                case 1:
                    numPages = 1;
                    briefingText.SetMessage("Last night's feasting has put the guards on alert. You'll have to deal with any guards that try and get between you and dinner!");
                    briefingImage = images[7];
                    break;
                //Forms Tutorial Briefing Text
                case 2:
                    numPages = 1;
                    briefingText.SetMessage("Vampires have the ability to shapeshift into different forms with unique attributes and abilities. \n You'll need to make use of these shapeshifting powers to feed on a villager tonight!");
                    briefingImage = images[1];
                    break;
                //Level 1 Briefing Text
                case 3:
                    numPages = 1;
                    briefingText.SetMessage("The villagers have recruited a cleric to ward off vampires. Don't let divine interference get between you and dinner!");
                    briefingImage = images[2];
                    break;
                //Level 2 Briefing Text
                case 4:
                    numPages = 1;
                    briefingText.SetMessage("Oh no! A rival vampire is in town! Feed on a villager before your gluttonous rival drains them all! \nRemember: You only need to feed on one villager to get the blood you need!");
                    briefingImage = images[3];
                    break;
                //Level 3 Briefing Text
                case 5:
                    numPages = 1;
                    briefingText.SetMessage("All this vampiric activity has attracted vampire hunters. A smart vampire picks his fights wisely.");
                    briefingImage = images[4];
                    break;
                //Level 4 Briefing Text
                case 6:
                    numPages = 1;
                    briefingText.SetMessage("The clerics and vampire hunters have teamed up under the leadership of a master vampire hunter! \nIt will take all your cunning to outwit this deadly alliance!");
                    briefingImage = images[5];
                    break;
                default:
                    break;


            }

            text.Add(briefingText);
        }

        public bool IsLevelSelectScreen()
        {
            return levelSelectScreen;
        }

        public void SetLevelSelectScreen(bool b)
        {
            levelSelectScreen = b;
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
