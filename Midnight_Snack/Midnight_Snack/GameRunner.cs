#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Tao.Sdl;
using System.Xml;
using System.IO;
using System.Text;
#endregion

namespace Midnight_Snack
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameRunner : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        Cursor cursor;
        Controls controls;
        GameManager gameManager;
        Enemy[] enemies;

        SelectionScene levelSelectScene;
        SelectionScene gameOverScene;
        SelectionScene levelCompleteScene;
        SelectionScene gameCompleteScene;
        SelectionScene levelBriefingScene;
        MainGame mainGame;
        Map map; //map for the current level instance
        List<Unit> units; //units for the current level instance
        List<Menu> menus; //menu for the current level instance

        public static int ScreenWidth;
        public static int ScreenHeight;

        public GameRunner()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            // TODO: Add your initialization logic here

            //Full Screen
            ScreenWidth = GraphicsDevice.DisplayMode.Width;
            ScreenHeight = GraphicsDevice.DisplayMode.Height;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            IsMouseVisible = false;
            this.Window.Title = "Midnight Snack";

            gameManager = GameManager.GetInstance();


            /**** Initialize Level Select Screen ****/
            Text titleText = new Text("Midnight Snack", new Vector2((ScreenWidth - 100) / 2, ScreenHeight / 4));
            Text startText = new Text("Select a Level", new Vector2((ScreenWidth - 100) / 2, ScreenHeight * 2 / 5));
            List<Text> levelSelectText = new List<Text>();
            levelSelectText.Add(titleText);
            levelSelectText.Add(startText);
            List<Text> levelSelectOptions = new List<Text>();
            Text option1 = new Text("Basic Tutorial", new Vector2(0, 0));
            Text option2 = new Text("Enemy Tutorial", new Vector2(0, 0));
            Text option3 = new Text("Tutorial", new Vector2(0, 0));
            Text option4 = new Text("Level 1", new Vector2(0, 0));
            Text option5 = new Text("Level 2", new Vector2(0, 0));
            Text option6 = new Text("Level 3", new Vector2(0, 0));
            levelSelectOptions.Add(option1);
            levelSelectOptions.Add(option2);
            levelSelectOptions.Add(option3);
            levelSelectOptions.Add(option4);
            levelSelectOptions.Add(option5);
            levelSelectOptions.Add(option6);
            Menu levelSelectMenu = new Menu(new Vector2((ScreenWidth - 100) / 2, ScreenHeight / 2), 100, 100, levelSelectOptions);
            levelSelectScene = new SelectionScene(levelSelectText, levelSelectMenu);

            /***** Initialize Level Briefing Screen ****/
            List<Text> levelBriefingText = new List<Text>();
            List<Text> levelBriefingOptions = new List<Text>();
            Text levelBriefingOption1 = new Text("Next", new Vector2(0, 0));
            levelBriefingOptions.Add(levelBriefingOption1);
            Menu levelBriefingMenu = new Menu(new Vector2((ScreenWidth - 100)/ 2, ScreenHeight - ScreenHeight / 6), 100, 100, levelBriefingOptions);
            levelBriefingScene = new SelectionScene(levelBriefingText, levelBriefingMenu);
            levelBriefingScene.SetBriefingScreen(true);

            /**** Initialize Main Game Screen ****/
            LoadXmlMap();

            /**** Initialize Game Over Scene ****/
            Text gameOverText = new Text("Game Over", new Vector2((ScreenWidth - 100) / 2, ScreenHeight / 3));
            List<Text> gameOverSceneText = new List<Text>();
            gameOverSceneText.Add(gameOverText);
            List<Text> gameOverOptions = new List<Text>();
            Text gameOverOption1 = new Text("Try Again", new Vector2(0, 0));
            Text gameOverOption2 = new Text("Level Select", new Vector2(0, 0));
            gameOverOptions.Add(gameOverOption1);
            gameOverOptions.Add(gameOverOption2);
            Menu gameOverMenu = new Menu(new Vector2((ScreenWidth - 100) / 2, ScreenHeight / 2), 100, 100, gameOverOptions);
            gameOverScene = new SelectionScene(gameOverSceneText, gameOverMenu);

            /**** Initialize Level Complete Scene ****/
            Text levelCompleteText = new Text("Level Complete!", new Vector2((ScreenWidth - 100) / 2, ScreenHeight / 3));
            List<Text> levelCompleteSceneText = new List<Text>();
            levelCompleteSceneText.Add(levelCompleteText);
            List<Text> levelCompleteOptions = new List<Text>();
            Text levelCompleteOption1 = new Text("Next Level", new Vector2(0, 0));
            levelCompleteOption1.SetAvailable(true);
            Text levelCompleteOption2 = new Text("Level Select", new Vector2(0, 0));
            levelCompleteOptions.Add(levelCompleteOption1);
            levelCompleteOptions.Add(levelCompleteOption2);
            Menu levelCompleteMenu = new Menu(new Vector2((ScreenWidth - 100) / 2, ScreenHeight / 2), 100, 100, levelCompleteOptions);
            levelCompleteScene = new SelectionScene(levelCompleteSceneText, levelCompleteMenu);

            /**** Initialize Game Complete Scene ****/
            Text gameCompleteText = new Text("Congratulations! \nYou are an amazing vampire! \nThanks for playing!", new Vector2((ScreenWidth - 100) / 2, ScreenHeight / 3));
            List<Text> gameCompleteSceneText = new List<Text>();
            gameCompleteSceneText.Add(gameCompleteText);
            List<Text> gameCompleteOptions = new List<Text>();
            Text gameCompleteOption1 = new Text("Level Select", new Vector2(0, 0));
            gameCompleteOptions.Add(gameCompleteOption1);
            Menu gameCompleteMenu = new Menu(new Vector2((ScreenWidth - 100) / 2, ScreenHeight / 2), 100, 100, gameCompleteOptions);
            gameCompleteScene = new SelectionScene(gameCompleteSceneText, gameCompleteMenu);

            //Start the game on the level select screen
            //gameState = levelSelect;
            gameManager.SetGameState(0);

            base.Initialize();

            Joystick.Init();
            Console.WriteLine("Number of joysticks: " + Sdl.SDL_NumJoysticks());
            controls = new Controls();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            levelSelectScene.LoadContent(this.Content);
            mainGame.LoadContent(this.Content);
            gameOverScene.LoadContent(this.Content);
            levelCompleteScene.LoadContent(this.Content);
            levelBriefingScene.LoadContent(this.Content);
            gameCompleteScene.LoadContent(this.Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name=gameTime>Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            controls.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //ScreenWidth = GraphicsDevice.Viewport.Width;
            //ScreenHeight = GraphicsDevice.Viewport.Height;

            switch (gameManager.GetGameState())
            {
                //Level Select Screen
                case 0:
                    levelSelectScene.Update(controls);
                    break;

                //Main Game Screen
                case 1:
                    mainGame.Update(controls);
                    break;

                //Game Over Screen
                case 2:
                    gameOverScene.Update(controls);
                    break;

                //Level Complete Screen
                case 3:
                    //Check if completed last level
                    if(gameManager.GetCurrentLevel() == 5)
                    {
                        gameManager.SetGameCompleted(true);
                    }

                    if (gameManager.IsGameCompleted())
                    {
                        gameCompleteScene.Update(controls);
                    }
                    else
                    {
                        levelCompleteScene.Update(controls);
                    }
                    break;

                //Level Briefing Screen
                case 4:
                    levelBriefingScene.Update(controls);
                    break;
                case 5:
                    LoadXmlMap();
                    mainGame.LoadContent(this.Content);
                    gameManager.SetGameState(4);
                    break;
                case 6:
                    LoadXmlMap();
                    mainGame.LoadContent(this.Content);
                    gameManager.SetGameState(1);
                    break;

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            switch (gameManager.GetGameState())
            {
                //Level Select Screen
                case 0:
                    levelSelectScene.Draw(spriteBatch);
                    break;

                //Main Game Screen
                case 1:
                    mainGame.Draw(spriteBatch);
                    break;

                //Game Over Screen
                case 2:
                    gameOverScene.Draw(spriteBatch);
                    break;

                //Level Complete Screen
                case 3:
                    if (gameManager.IsGameCompleted())
                    {
                        gameCompleteScene.Draw(spriteBatch);
                    }
                    else
                    {
                        levelCompleteScene.Draw(spriteBatch);
                    }
                    break;

                //Level Briefing Screen
                case 4:
                    levelBriefingScene.Draw(spriteBatch);
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }


        public void LoadXmlMap()
        {
            string levelFile;
            switch (gameManager.GetCurrentLevel())
            {
                case 0:
                    levelFile = "basic_tutorial";
                    break;
                case 1:
                    levelFile = "enemy_tutorial";
                    break;
                case 2:
                    levelFile = "tutorial";
                    break;
                case 3:
                    levelFile = "level1";
                    break;
                case 4:
                    levelFile = "level2";
                    break;
                case 5:
                    levelFile = "level3";
                    break;
                default:
                    levelFile = null;
                    break;
            }
            XmlDocument doc = new XmlDocument();
            String contentDir = Directory.GetCurrentDirectory() + "\\Content\\" + levelFile + ".xml";
            doc.Load(contentDir);
            StringBuilder output = new StringBuilder();
            string xmlcontents = doc.InnerXml;

            //Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlcontents)))
            {
                //pull map info
                reader.ReadToFollowing("map");
                int rows = Convert.ToInt32(reader.GetAttribute("rows"));
                int cols = Convert.ToInt32(reader.GetAttribute("cols"));
                int startRow = Convert.ToInt32(reader.GetAttribute("startRow"));
                int startCol = Convert.ToInt32(reader.GetAttribute("startCol"));

                
                reader.ReadToDescendant("turnLim"); //get turn limit
                int turnLim = Convert.ToInt32(reader.GetAttribute("limit"));

                //get cursor info
                reader.ReadToNextSibling("cursor"); 
                int cw = Convert.ToInt32(reader.GetAttribute("width"));
                int ch = Convert.ToInt32(reader.GetAttribute("height"));
                output.AppendLine("The cursor dimensions: ");
                output.AppendLine("\t width: " + cw);
                output.AppendLine("\t height: " + ch);

                //set turn limit and create the map
                gameManager.SetTurnLimit(turnLim);
                
                map = Map.GetInstance();
                map.SetRows(rows);
                map.SetCols(cols);
                map.FillGrid(rows, cols);
                map.SetLair(startRow, startCol);
                units = new List<Unit>();
                

                //Reset player fields between levels
                player = Player.GetInstance();
                player.SetRow(map.GetLairRow());
                player.SetCol(map.GetLairCol());
                player.SetPosition(map.GetLairPos());
                player.SetMap(map);
                player.SetCurrentHealth(player.GetMaxHealth());
                player.SetHasBlood(false);
                player.SetAlive(true);
                player.SetForm("vampire");
                player.SetMovedThisTurn(false);
                player.SetHasEndedTurn(false);
                player.SetUsedAbilityThisTurn(false);
                units.Add(player);

                //get villager info
                reader.ReadToNextSibling("villagers");
                int num_villagers = Convert.ToInt32(reader.GetAttribute("numVillagers"));
                reader.ReadStartElement();
                for (int i = 0; i < num_villagers; i++)
                {
                    int vw = Convert.ToInt32(reader.GetAttribute("width"));
                    int vh = Convert.ToInt32(reader.GetAttribute("height"));
                    int vrow = Convert.ToInt32(reader.GetAttribute("row"));
                    int vcol = Convert.ToInt32(reader.GetAttribute("col"));
                    //output.AppendLine("The villager dimensions: ");
                    //output.AppendLine("\t rows: " + vrow);
                    //output.AppendLine("\t cols: " + vcol);
                    //output.AppendLine("\t width: " + vw);
                    //output.AppendLine("\t height: " + vh);

                    //Set up villager stuff
                    SleepingVillager villager = new SleepingVillager(new Vector2(0, 0), vw, vh, vrow, vcol);
                    //Mark villager tile as occupied
                    MapTile villagerTile = map.GetTile(villager.GetRow(), villager.GetCol());
                    villagerTile.SetOccupant(villager);
                    villagerTile.SetPassable(false);
                    map.SetTile(villager.GetRow(), villager.GetCol(), villagerTile);
                    villager.SetPosition(villagerTile.GetPosition());

                    units.Add(villager);

                    reader.ReadToNextSibling("villager");
                }


                //Get obstacles information and then create all of them accordingly
                reader.ReadToNextSibling("obstacles");
                int num_obs = Convert.ToInt32(reader.GetAttribute("numObs"));
                reader.ReadStartElement();
                //Console.WriteLine("loar: " + map.GetLairCol() + " " + map.GetLairRow());
                for (int i = 0; i < num_obs; i++)
                {
                    int orows = Convert.ToInt32(reader.GetAttribute("row"));
                    int ocols = Convert.ToInt32(reader.GetAttribute("col"));
                    string otype = reader.GetAttribute("type");
                    //output.AppendLine("Obstacle at: " + ocols + " " + orows);

                    MapTile obstacleTile = map.GetTile(orows, ocols);
                    obstacleTile.SetPassable(false);
                    map.SetTile(orows, ocols, obstacleTile);

                    Obstacle obstacle = new Obstacle(obstacleTile.GetPosition(), 100, 100, orows, ocols, otype);
                    units.Add(obstacle);

                    reader.ReadToNextSibling("obstacle");
                }


                //get enemies info, then create every enemy type accordingly
                reader.ReadToNextSibling("enemies");
                int num_enemies = Convert.ToInt32(reader.GetAttribute("numEnemies"));
                reader.ReadStartElement();
                enemies = new Enemy[num_enemies];
                int[] enemyX = new int[num_enemies];
                int[] enemyY = new int[num_enemies];
                int[] enemyRange = new int[num_enemies];
                MapTile[] enemyTiles = new MapTile[num_enemies];


                for (int i = 0; i < num_enemies; i++ )
                {
                    string etype = reader.GetAttribute("type");
                    int ew = Convert.ToInt32(reader.GetAttribute("width"));
                    int eh = Convert.ToInt32(reader.GetAttribute("height"));
                    int erow = Convert.ToInt32(reader.GetAttribute("row"));
                    int ecol = Convert.ToInt32(reader.GetAttribute("col"));
                    int erange = Convert.ToInt32(reader.GetAttribute("range"));
                    int ehealth = Convert.ToInt32(reader.GetAttribute("health"));
                    
                    //Console.WriteLine("pos: enemy: " + ecol + ":" + erow);

                    enemyX[i] = erow;
                    enemyY[i] = ecol;
                    enemyRange[i] = erange;

                    if (etype.Equals("guard"))
                    {
                        int[] tgdests = new int[4];
                        string steps2 = reader.GetAttribute("dest");
                        string[] destsinString = steps2.Split(new char[] { ';' });
                        
                        
                        for (int j = 0; j < tgdests.Length; j++)
                        {
                            Debug.WriteLine("Row: " + destsinString[j]);
                            tgdests[j] = Convert.ToInt32(destsinString[j]);
                        }
                        enemies[i] = new TownGuard(new Vector2(0, 0), ew, eh, enemyX[i],
                        enemyY[i], enemyRange[i], ehealth, tgdests);
                    }
                    else if (etype.Equals("vampire"))
                    {
                        enemies[i] = new VampireEnemy(new Vector2(0, 0), ew, eh, enemyX[i],
                        enemyY[i], enemyRange[i], ehealth);
                    }
                    else if (etype.Equals("cleric"))
                    {
                        enemies[i] = new ClericEnemy(new Vector2(0, 0), ew, eh, enemyX[i],
                        enemyY[i], enemyRange[i], ehealth);
                    }
                    else if (etype.Equals("hunter"))
                    {
                        enemies[i] = new HunterEnemy(new Vector2(0, 0), ew, eh, enemyX[i],
                        enemyY[i], enemyRange[i], ehealth);
                    }
                    else
                    {
                        enemies[i] = new Enemy(new Vector2(0, 0), ew, eh, enemyX[i],
                            enemyY[i], enemyRange[i], ehealth);
                    }

                    enemyTiles[i] = map.GetTile(enemies[i].GetRow(), enemies[i].GetCol());
                    enemyTiles[i].SetOccupant(enemies[i]);
                    map.SetTile(enemies[i].GetRow(), enemies[i].GetCol(), enemyTiles[i]);
                    enemies[i].SetPosition(enemyTiles[i].GetPosition());

                    //Create a list of all the units on the map
                    units.Add(enemies[i]);
                    reader.ReadToNextSibling("enemy");
                }

                //Get hint information and then create all of them accordingly
                List<Hint> hints = new List<Hint>();
                reader.ReadToNextSibling("hints");
                int num_hints = Convert.ToInt32(reader.GetAttribute("numHints"));
                reader.ReadStartElement();
                for (int i = 0; i < num_hints; i++)
                {
                    bool beforeBlood = true;
                    string b4Blood = reader.GetAttribute("beforeBlood");
                    if(b4Blood.Equals("false"))
                    {
                        beforeBlood = false;
                    }
                    List<int> hintRows = new List<int>();
                    List<int> hintCols = new List<int>();
                    string hRows = reader.GetAttribute("rows");
                    string hCols = reader.GetAttribute("cols");
                    int numHintSpots = Convert.ToInt32(reader.GetAttribute("numSpots"));
                    if (numHintSpots == 1)
                    {
                        hintRows.Add(Convert.ToInt32(hRows));
                        hintCols.Add(Convert.ToInt32(hCols));
                    }
                    else
                    {
                        string[] hintRowCoords = hRows.Split(new char[] { ';' });
                        string[] hintColCoords = hCols.Split(new char[] { ';' });

                        for (int j = 0; j < numHintSpots; j++)
                        {
                            hintRows.Add(Convert.ToInt32(hintRowCoords[j]));
                            hintCols.Add(Convert.ToInt32(hintColCoords[j]));
                        }
                    }

                    string hintText = reader.GetAttribute("text");
                    //output.AppendLine("Hint at: " + hintRow + " " + hintCol);

                    Hint hint = new Hint(new Vector2(0, ScreenHeight - 200), 200, 200, beforeBlood, hintRows, hintCols, hintText);
                    hints.Add(hint);

                    reader.ReadToNextSibling("hint");
                }

                output.AppendLine("The map dimensions: ");
                output.AppendLine("\t rows: " + rows);
                output.AppendLine("\t cols: " + cols);
                output.AppendLine("\t startRow: " + startRow);
                output.AppendLine("\t startCol: " + startCol);
                Console.WriteLine(output);

                //Get tile types and create all of them accordingly
                reader.ReadToNextSibling("tiles");
                int num_tiles = Convert.ToInt32(reader.GetAttribute("numTiles"));
                reader.ReadStartElement();
                //Console.WriteLine("loar: " + map.GetLairCol() + " " + map.GetLairRow());
                for (int i = 0; i < num_tiles; i++)
                {
                    int trows = Convert.ToInt32(reader.GetAttribute("row"));
                    int tcols = Convert.ToInt32(reader.GetAttribute("col"));
                    string modifier = reader.GetAttribute("mod");
                    //output.AppendLine("Obstacle at: " + ocols + " " + orows);

                    MapTile obstacle = map.GetTile(trows, tcols);
                    obstacle.SetModifier(modifier);
                    map.SetTile(trows, tcols, obstacle);
                    reader.ReadToNextSibling("tile");
                }

                cursor = new Cursor(map.GetLairPos(), cw, ch);

                //Set up menus needs to be done here sonce it's dependent on the player 
                Text moveText = new Text("Move", player.GetPosition());
                Text abilitiesText = new Text("Abilities", player.GetPosition());
                Text endTurnText = new Text("End Turn", player.GetPosition());
                List<Text> actionMenuOptions = new List<Text>();
                actionMenuOptions.Add(moveText);
                actionMenuOptions.Add(abilitiesText);
                actionMenuOptions.Add(endTurnText);
                menus = new List<Menu>();
                MiniMenu actionMenu = new MiniMenu(player.GetPosition(), 70, 70, actionMenuOptions);
                menus.Add(actionMenu);

                //Create the main game screen
                mainGame = new MainGame(units, cursor, menus, hints);
            }
        }

    }
}
