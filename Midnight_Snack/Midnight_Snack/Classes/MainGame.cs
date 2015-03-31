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
        private MiniMenu abilitiesMenu;
 
        private Text endText;
        private Text turnText;
        private Text goalText;

        private int activeUnit;

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
            Text abilitiesText = new Text("Abilities", player.GetPosition());
            Text endTurnText = new Text("End Turn", player.GetPosition());
            List<Text> actionMenuOptions = new List<Text>();
            actionMenuOptions.Add(moveText);
            actionMenuOptions.Add(abilitiesText);
            actionMenuOptions.Add(endTurnText);
            actionMenu = new MiniMenu(player.GetPosition(), 70, 70, actionMenuOptions);
            menus.Add(actionMenu);

            Text feedText = new Text("Feed", player.GetPosition());
            Text attackText = new Text("Attack", player.GetPosition());
            Text endAbilityTurnText = new Text("End Ability", player.GetPosition());
            List<Text> abilitiesMenuOptions = new List<Text>();
            abilitiesMenuOptions.Add(attackText);
            abilitiesMenuOptions.Add(feedText);
            abilitiesMenuOptions.Add(endAbilityTurnText);
            abilitiesMenu = new MiniMenu(player.GetPosition(), 70, 70, abilitiesMenuOptions);
            menus.Add(abilitiesMenu);

            turnText = new Text("Turn: 1", new Vector2(10, 5));
            goalText = new Text("Goal: Get blood from villager and get back to start in 8 turns \nMove with arrow keys and select with space. Cancel out of an action with F", new Vector2(20, GameRunner.ScreenHeight * 5/6));
            endText = new Text("", new Vector2(700, 60));
            endText.SetVisible(false);
            text.Add(turnText);
            text.Add(goalText);
            text.Add(endText);

            activeUnit = 0;
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
            turnText.SetMessage("Turns Until Sunrise: " + (gameManager.GetTurnLimit() - gameManager.GetTurn()));

            //Temporary HealthBar tester
            if(controls.onPress(Keys.J, Buttons.X))
            {
                player.SetCurrentHealth(player.GetCurrentHealth() - 1);
            }
            if (controls.onPress(Keys.K, Buttons.Y))
            {
                player.SetCurrentHealth(player.GetCurrentHealth() + 1);
            }

            //Update units
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i] != null)
                {
                    //System.Diagnostics.Debug.WriteLine("activeUnitIndex = " + activeUnit + "; activeUnit is " + units[activeUnit].GetType());

                    //If it is the player's turn
                    if (i == activeUnit && units[i].Equals(player))
                    {
                        //If player has not already ended their turn
                        if(!units[i].GetHasEndedTurn())
                        {
                            //Set that it's their turn
                            units[i].SetUnitsTurn(true);
                        }
                        //If the player has already ended their turn
                        else
                        {
                            //If player ends their turn on consecrated tile, take damage
                            if(map.GetTile(player.GetRow(), player.GetCol()).GetModifier().Equals("consecrated"))
                            {
                                //Take 1 damage
                                player.SetCurrentHealth(player.GetCurrentHealth() - 1);
                            }

                            //Set that it's not player's turn
                            units[i].SetUnitsTurn(false);
                            //Go to the next unit's turn
                            NextActiveUnit();
                        }
                    }
                    else
                    {
                        //If it's this unit's turn, set it
                        if (i == activeUnit)
                        {
                            //If unit does not act, just skip it
                            if (!(units[i].GetType()).IsSubclassOf(typeof(MobileUnit)))
                            {
                                NextActiveUnit();
                            }
                            else
                            {
                                //If unit has not already ended it's turn
                                if (!units[i].GetHasEndedTurn())
                                {
                                    //Set that it's their turn
                                    units[i].SetUnitsTurn(true);
                                }
                                //If unit has already ended it's turn
                                else
                                {
                                    //Set that it's not it's turn
                                    units[i].SetUnitsTurn(false);
                                    //Go to the next unit's turn
                                    NextActiveUnit();
                                }
                            }
                        }
                    }
                    units[i].Update();
                }
            }

            //Only display the action menu when using it
            if (gameManager.IsInActionMenu())
            {
                actionMenu.SetVisible(true);
                if (gameManager.IsInAbilitiesMenu())
                {
                    abilitiesMenu.SetVisible(true);
                    abilitiesMenu.Update(controls);
                }
                else
                {
                    abilitiesMenu.SetVisible(false);
                    actionMenu.Update(controls);
                }
            }
            else
            {
                actionMenu.SetVisible(false);
                abilitiesMenu.SetVisible(false);
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
                gameManager.SetGameState(2);
                endText.SetMessage("You Lose!");
                endText.SetVisible(true);
            }
            //Check if player has won
            else if (gameManager.HasWon())
            {
                gameManager.SetGameState(3);
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
            abilitiesMenu.Draw(spriteBatch);
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

        //Gets the index of the next unit whose turn it should be
        public void NextActiveUnit()
        {
            //If all units have had a turn,
            //go back to start of unit list
            if (activeUnit >= units.Count - 1)
            {
                //Reset index
                activeUnit = 0;
                //Reset all units' ended turns
                for (int i = 0; i < units.Count; i++)
                {
                    units[i].SetHasEndedTurn(false);
                }
            }
            //Otherwise go to next unit in the list
            else
            {
                activeUnit++;
            }
        }
    }
}
