using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Midnight_Snack
{
    public class GameManager
    {
        private bool inActionMenu;  //Is the player currently navigating the action menu
        private bool inAbilitiesMenu;   //Is the player currently navigating the ability menu
        private bool movingPlayer;  //Is the player trying to move their character
        private bool choosingAbilityTarget;    //Is the player trying to choose their ability target
        private string playerAbility; //What ability is the player trying to use right now (empty string if none)
        private bool playerAlive;   //Is the player still alive
        private bool playerWin; //Has the player won the level
        private int currentTurn;    //Keeps track of the number of turns
        private int turnLimit;  //The most amount of turns the player is allowed to complete objective in
        private int currentLevel;   //What level is the player currently on? (defaults to 0; the tutorial)

        //Tracks what state the game is in (i.e. main menu, gameplay, game over, etc.)
        int gameState;
        const int levelSelect = 0, mainGame = 1, gameOver = 2, levelComplete = 3, levelBriefing = 4, loadMap = 5, tryAgain = 6;

        private static GameManager instance = new GameManager();

        private GameManager()
        {
            inActionMenu = false;
            inAbilitiesMenu = false;
            movingPlayer = false;
            choosingAbilityTarget = false;
            playerAbility = "";
            playerAlive = true;
            playerWin = false;
            currentTurn = 1;
            turnLimit = 5;
            currentLevel = 0;
        }

        public static GameManager GetInstance()
        {
            return instance;
        }

        public bool IsInActionMenu()
        {
            return inActionMenu;
        }

        public void SetInActionMenu(bool b)
        {
            inActionMenu = b;
        }

        public bool IsInAbilitiesMenu()
        {
            return inAbilitiesMenu;
        }

        public void SetInAbilitiesMenu(bool b)
        {
            inAbilitiesMenu = b;
        }

        public bool IsMovingPlayer()
        {
            return movingPlayer;
        }

        public void SetMovingPlayer(bool b)
        {
            movingPlayer = b;
        }

        public bool IsPlayerAlive()
        {
            return playerAlive;
        }

        public void SetPlayerAlive(bool b)
        {
            playerAlive = b;
        }

        public bool IsChoosingAbilityTarget()
        {
            return choosingAbilityTarget;
        }

        public void SetChoosingAbilityTarget(bool b)
        {
            choosingAbilityTarget = b;
        }

        public string GetPlayerAbility()
        {
            return playerAbility;
        }

        public void SetPlayerAbility(string ability)
        {
            playerAbility = ability;
        }

        public bool HasWon()
        {
            return playerWin;
        }

        public void SetWon(bool b)
        {
            playerWin = b;
        }

        public int GetTurn()
        {
            return currentTurn;
        }

        public void SetTurn(int num)
        {
            currentTurn = num;
        }

        public int GetTurnLimit()
        {
            return turnLimit;
        }

        public void SetTurnLimit(int num)
        {
            turnLimit = num;
        }

        public int GetGameState()
        {
            return gameState;
        }

        public void SetGameState(int state)
        {
            gameState = state;
        }

        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        public void SetCurrentLevel(int level)
        {
            currentLevel = level;
        }

        //Resets the GameManager back to it's initial state
        public void ResetGameState()
        {
            inActionMenu = false;
            inAbilitiesMenu = false;
            movingPlayer = false;
            choosingAbilityTarget = false;
            playerAbility = "";
            playerAlive = true;
            playerWin = false;
            currentTurn = 1;
            turnLimit = 5;
        }
    }
}
