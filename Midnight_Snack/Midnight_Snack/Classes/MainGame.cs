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
    public class MainGame : Scene
    {
        private Map map;
        private List<Unit> units;
        private Cursor cursor;
        private List<Menu> menus;
        private MiniMenu actionMenu;
 
        private Text endText;
        private Text turnText;
        private Text goalText;

        GameManager gameManager = GameManager.GetInstance();
        Player player = Player.GetInstance();

        public MainGame(Map map, List<Unit> units, Cursor cursor, List<Menu> menus) 
        {
            this.map = map;
            this.units = units;
            this.cursor = cursor;
            this.menus = menus;
            this.text = new List<Text>();

            Text moveText = new Text("Move", player.GetPosition());
            Text interactText = new Text("Interact", player.GetPosition());
            Text endTurnText = new Text("End Turn", player.GetPosition());
            List<Text> actionMenuOptions = new List<Text>();
            actionMenuOptions.Add(moveText);
            actionMenuOptions.Add(interactText);
            actionMenuOptions.Add(endTurnText);
            actionMenu = new MiniMenu(player.GetPosition(), 70, 70, actionMenuOptions);
            menus.Add(actionMenu);

            turnText = new Text("Turn: 1", new Vector2(700, 20));
            goalText = new Text("Goal: Get blood from villager and get back to start in 5 turns \nMove with arrow keys and select with space. Cancel out of an action with F", new Vector2(20, 420));
            endText = new Text("", new Vector2(700, 60));
            endText.SetVisible(false);
            text.Add(turnText);
            text.Add(goalText);
            text.Add(endText);
            
        }

        public override void LoadContent(ContentManager content)
        {
            map.LoadContent(content);
            cursor.LoadContent(content);

            //Load unit content
            for(int i = 0; i < units.Count; i++)
            {
                units[i].LoadContent(content);
            }
            //Load menu content
            for (int i = 0; i < menus.Count; i++)
            {
                menus[i].LoadContent(content);
            }
            //Load all text content
            for (int i = 0; i < text.Count; i++)
            {
                text[i].LoadContent(content);
            }
        }

        public void Update(Controls controls)
        {
            //Update Turn Counter
            turnText.SetMessage("Turn: " + gameManager.GetTurn());

            //Only display the action menu when using it
            if (gameManager.IsInActionMenu())
            {
                actionMenu.SetVisible(true);
                actionMenu.Update(controls);
            }
            else
            {
                actionMenu.SetVisible(false);
                cursor.Update(controls);
            }

            //Update player win progression status
            if (gameManager.GetTurn() > gameManager.GetTurnLimit())
            {
                gameManager.SetPlayerAlive(false);
            }
            if (player.HasBlood() && player.GetRow() == map.GetLairRow() && player.GetCol() == map.GetLairCol())
            {
                gameManager.SetWon(true);
            }
            //Check if player has lost
            if (!gameManager.IsPlayerAlive())
            {
                endText.SetMessage("You Lose!");
                endText.SetVisible(true);
            }
            //Check if player has won
            else if (gameManager.HasWon())
            {
                endText.SetMessage("You Win!");
                endText.SetVisible(true);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw map
            map.Draw(spriteBatch);
            //Draw cursor
            cursor.Draw(spriteBatch);
            //Draw units
            for (int i = 0; i < units.Count; i++)
            {
                if(units[i] != null)
                {
                    units[i].Draw(spriteBatch);
                }
            }
            //Draw menus
            actionMenu.Draw(spriteBatch);
            for (int i = 0; i < menus.Count; i++)
            {
                if(menus[i] != null)
                {
                    menus[i].Draw(spriteBatch);
                }
            }
            //Draw all text content
            for (int i = 0; i < text.Count; i++)
            {
                if(text[i] != null)
                { 
                    text[i].Draw(spriteBatch);
                }
            }
        }

        public List<Unit> GetUnits()
        {
            return units;
        }

        public void SetUnits(List<Unit> units)
        {
            this.units = units;
        }

        public bool RemoveUnit(Unit unit)
        {
            if(units.Contains(unit))
            {
                units.Remove(unit);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddUnit(Unit unit)
        {
            units.Add(unit);
        }

        public Map GetMap()
        {
            return map;
        }

        public void SetMap(Map map)
        {
            this.map = map;
        }
    }
}
