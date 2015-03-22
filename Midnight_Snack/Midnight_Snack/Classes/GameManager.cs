using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Midnight_Snack
{
    public class GameManager
    {
        private bool inActionMenu;  //Is the player currently navigating the action menu
        private bool movingPlayer;  //Is the player trying to move their character
        private bool choosingAbilityTarget;    //Is the player trying to choose their interact target
        private bool playerAlive;   //Is the player still alive
        private bool playerWin; //Has the player won the level
        private int currentTurn;    //Keeps track of the number of turns
        private int turnLimit;  //The most amount of turns the player is allowed to complete objective in

        //Tracks what state the game is in (i.e. main menu, gameplay, game over, etc.)
        int gameState;
        const int levelSelect = 0, mainGame = 1, gameOver = 2, levelComplete = 3;

        private static GameManager instance = new GameManager();

        private GameManager()
        {
            inActionMenu = false;
            movingPlayer = false;
            choosingAbilityTarget = false;
            playerAlive = true;
            playerWin = false;
            currentTurn = 1;
            turnLimit = 5;
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

        //Resets the GameManager back to it's initial state
        public void ResetGameState()
        {
            inActionMenu = false;
            movingPlayer = false;
            choosingAbilityTarget = false;
            playerAlive = true;
            playerWin = false;
            currentTurn = 1;
            turnLimit = 5;
        }
    }
}
