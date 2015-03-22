#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Tao.Sdl;
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

        SelectionScene levelSelectScene;
        SelectionScene gameOverScene;
        SelectionScene levelCompleteScene;
        MainGame mainGame;

        /*
        //Tracks what state the game is in (i.e. main menu, gameplay, game over, etc.)
        int gameState;
        const int levelSelect = 0, mainGame = 1;
         * */

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
            ScreenWidth = GraphicsDevice.Viewport.Width;
            ScreenHeight = GraphicsDevice.Viewport.Height;

            IsMouseVisible = true;
            this.Window.Title = "Midnight Snack";

            gameManager = GameManager.GetInstance();

            /**** Initialize Level Select Screen ****/
            Text titleText = new Text("Midnight Snack", new Vector2(300, 100));
            Text startText = new Text("Select a Level", new Vector2(300, 250));
            List<Text> levelSelectText = new List<Text>();
            levelSelectText.Add(titleText);
            levelSelectText.Add(startText);
            List<Text> levelSelectOptions = new List<Text>();
            Text option1 = new Text("Tutorial", new Vector2(0, 0));
            Text option2 = new Text("Level 1", new Vector2(0, 0));
            //Level 1 not available right now so display it as unavailable
            option2.SetAvailable(false);
            levelSelectOptions.Add(option1);
            levelSelectOptions.Add(option2);
            Menu levelSelectMenu = new Menu(new Vector2(300, 300), 100, 100, levelSelectOptions);
            levelSelectScene = new SelectionScene(levelSelectText, levelSelectMenu);

            /**** Initialize Main Game Screen ****/
            //Create map
            Map map = new Map(4, 6, 0, 0);
            //Create test obstacles
            MapTile obstacle1 = map.GetTile(2, 1);
            obstacle1.SetPassable(false);
            map.SetTile(2, 1, obstacle1);
            MapTile obstacle2 = map.GetTile(1, 3);
            obstacle2.SetPassable(false);
            map.SetTile(1, 3, obstacle2);

            //Set up player stuff
            cursor = new Cursor(10, 30, 100, 100, map);
            player = Player.GetInstance();
            //player.SetMap(map);
            player.SetRow(map.GetLairRow());
            player.SetCol(map.GetLairCol());
            player.SetPosition(new Vector2(10, 30));

            //Set up villager stuff
            SleepingVillager villager = new SleepingVillager(new Vector2(0, 0), 100, 100, 2, 4);
            //Mark villager tile as occupied
            MapTile villagerTile = map.GetTile(2, 4);
            villagerTile.SetOccupant(villager);
            map.SetTile(2, 4, villagerTile);
            villager.SetPosition(villagerTile.GetPosition());

            //Create a list of all the units on the map
            List<Unit> units = new List<Unit>();
            units.Add(player);
            units.Add(villager);
            //Set up menus
            Text moveText = new Text("Move", player.GetPosition());
            Text abilitiesText = new Text("Abilities", player.GetPosition());
            Text endTurnText = new Text("End Turn", player.GetPosition());
            List<Text> actionMenuOptions = new List<Text>();
            actionMenuOptions.Add(moveText);
            actionMenuOptions.Add(abilitiesText);
            actionMenuOptions.Add(endTurnText);
            List<Menu> menus = new List<Menu>();
            MiniMenu actionMenu = new MiniMenu(player.GetPosition(), 70, 70, actionMenuOptions);
            menus.Add(actionMenu);
            //Create the main game screen
            mainGame = new MainGame(map, units, cursor, menus);

            /**** Initialize Game Over Scene ****/
            Text gameOverText = new Text("Game Over", new Vector2(300, 100));
            List<Text> gameOverSceneText = new List<Text>();
            gameOverSceneText.Add(gameOverText);
            List<Text> gameOverOptions = new List<Text>();
            Text gameOverOption1 = new Text("Try Again", new Vector2(0, 0));
            //Can't currently "Try Again" yet so gray out option
            gameOverOption1.SetAvailable(false);
            Text gameOverOption2 = new Text("Level Select", new Vector2(0, 0));
            gameOverOptions.Add(gameOverOption1);
            gameOverOptions.Add(gameOverOption2);
            Menu gameOverMenu = new Menu(new Vector2(300, 300), 100, 100, gameOverOptions);
            gameOverScene = new SelectionScene(gameOverSceneText, gameOverMenu);

            /**** Initialize Level Complete Scene ****/
            Text levelCompleteText = new Text("Level Complete!", new Vector2(300, 100));
            List<Text> levelCompleteSceneText = new List<Text>();
            levelCompleteSceneText.Add(levelCompleteText);
            List<Text> levelCompleteOptions = new List<Text>();
            Text levelCompleteOption1 = new Text("Next Level", new Vector2(0, 0));
            //Can't currently go to next level yet, so gray out option
            levelCompleteOption1.SetAvailable(false);
            Text levelCompleteOption2 = new Text("Level Select", new Vector2(0, 0));
            levelCompleteOptions.Add(levelCompleteOption1);
            levelCompleteOptions.Add(levelCompleteOption2);
            Menu levelCompleteMenu = new Menu(new Vector2(300, 300), 100, 100, levelCompleteOptions);
            levelCompleteScene = new SelectionScene(levelCompleteSceneText, levelCompleteMenu);

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
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            controls.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            ScreenWidth = GraphicsDevice.Viewport.Width;
            ScreenHeight = GraphicsDevice.Viewport.Height;

            switch(gameManager.GetGameState())
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
                    levelCompleteScene.Update(controls);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

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
                    levelCompleteScene.Draw(spriteBatch);
                break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
