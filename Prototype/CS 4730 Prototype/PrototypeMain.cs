#region Using Statements
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Tao.Sdl;
#endregion

namespace CS_4730_Prototype
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PrototypeMain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        Cursor cursor;
        Controls controls;
        Map map;
        GameStatusTracker gst;
        MiniMenu actionMenu;
        SleepingVillager villager;
        InfoText endText;
        InfoText turnText;
        InfoText goalText;

        public static int ScreenWidth;
        public static int ScreenHeight;

        public PrototypeMain()
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

            //Create map
            map = new Map(4, 6, 0, 0);
            //Create test obstacles
            MapTile obstacle1 = map.GetTile(2, 1);
            obstacle1.SetPassable(false);
            map.SetTile(2, 1, obstacle1);
            MapTile obstacle2 = map.GetTile(1, 3);
            obstacle2.SetPassable(false);
            map.SetTile(1, 3, obstacle2);

            //Set up player stuff
            cursor = new Cursor(10, 10, 100, 100, map);
            player = Player.GetInstance();
            player.SetMap(map);
            player.SetRow(map.GetLairRow());
            player.SetCol(map.GetLairCol());
            player.SetPosition(new Vector2(10, 10));
            
            //Set up villager stuff
            villager = new SleepingVillager(410, 210, 100, 100, 2, 4);
            //Mark villager tile as occupied
            MapTile villagerTile = map.GetTile(2, 4);
            villagerTile.SetOccupant(villager);
            map.SetTile(2, 4, villagerTile);

            gst = GameStatusTracker.GetInstance();

            actionMenu = new MiniMenu(player.GetPosition(), 70, 70);
            
            turnText = new InfoText("Turn: 1", new Vector2(700, 20));
            goalText = new InfoText("Goal: Get blood from villager and get back to start in 5 turns \nMove with arrow keys and select with space. Cancel out of an action with F", new Vector2(20, 420));
            endText = new InfoText("", new Vector2(700, 60));
            endText.SetVisible(false);

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
            player.LoadContent(this.Content);
            villager.LoadContent(this.Content);
            map.LoadContent(this.Content);
            cursor.LoadContent(this.Content);
            actionMenu.LoadContent(this.Content);
            goalText.LoadContent(this.Content);
            endText.LoadContent(this.Content);
            turnText.LoadContent(this.Content);
            
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

            //Update turn counter
            turnText.SetText("Turn: " + gst.GetTurn());

            if (!gst.IsInActionMenu())
            {
                actionMenu.SetVisible(false);
                cursor.Update(controls);
            }
            else
            {
                actionMenu.SetVisible(true);
                actionMenu.Update(controls);
            }

            //Update player win progression status
            if (gst.GetTurn() > gst.GetTurnLimit())
            {
                gst.SetPlayerAlive(false);
            }
            if (player.HasBlood() && player.GetRow() == map.GetLairRow() && player.GetCol() == map.GetLairCol())
            {
                gst.SetWon(true);
            }
            //Check if player has lost
            if (!gst.IsPlayerAlive())
            {
                endText.SetText("You Lose!");
                endText.SetVisible(true);
            }
            //Check if player has won
            else if (gst.HasWon())
            {
                endText.SetText("You Win!");
                endText.SetVisible(true);
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
            map.Draw(spriteBatch);
            cursor.Draw(spriteBatch);
            player.Draw(spriteBatch);
            villager.Draw(spriteBatch);
            actionMenu.Draw(spriteBatch);
            endText.Draw(spriteBatch);
            turnText.Draw(spriteBatch);
            goalText.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
