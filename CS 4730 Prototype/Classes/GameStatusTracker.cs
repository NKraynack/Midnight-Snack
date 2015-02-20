using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS_4730_Prototype
{
    public class GameStatusTracker
    {
        private bool inActionMenu;  //Is the player currently navigating the action menu
        private bool movingPlayer;  //Is the player trying to move their character
        private bool choosingInteractTarget;    //Is the player trying to choose their interact target
        private bool playerAlive;   //Is the player still alive
        private bool playerWin; //Has the player won the level
        private int currentTurn;    //Keeps track of the number of turns
        private int turnLimit;  //The most amount of turns the player is allowed to complete objective in

        private static GameStatusTracker instance = new GameStatusTracker();

        private GameStatusTracker()
        {
            inActionMenu = false;
            movingPlayer = false;
            choosingInteractTarget = false;
            playerAlive = true;
            playerWin = false;
            currentTurn = 1;
            turnLimit = 5;
        }

        public static GameStatusTracker GetInstance()
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

        public bool IsChoosingInteractTarget()
        {
            return choosingInteractTarget;
        }

        public void SetChoosingInteractTarget(bool b)
        {
            choosingInteractTarget = b;
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
    }
}
