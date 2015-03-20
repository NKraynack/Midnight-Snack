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
    public class MobileUnit : Unit
    {
        private int moveRange;  //Number of tiles the unit can move in one turn
        private int maxHealth;  //The greatest amount of health this unit can have
        private int currentHealth;  //The current amount of health this unit has
        private bool movedThisTurn; //Has the unit moved this turn?
        private bool usedAbilityThisTurn;   //Has the unit used an ability this turn?
        private bool alive; //Is the unit alive?

        public MobileUnit(Vector2 pos, int width, int height, int row, int col, int range, int health) : base(pos, width, height, row, col)
        {
            moveRange = range;
            maxHealth = health;
            currentHealth = health;
            movedThisTurn = false;
            usedAbilityThisTurn = false;
            alive = true;
        }

        //Moves the unit to the given position
        public void Move(Vector2 pos, int row, int col)
        {
            SetPosition(pos);
            SetRow(row);
            SetCol(col);

            movedThisTurn = true;
        }

        public void UseAbility(string ability)
        {
            //Override in subclasses
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public void SetCurrentHealth(int health)
        {
            currentHealth = health;
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }

        public void SetMaxHealth(int health)
        {
            maxHealth = health;
        }

        public int GetMoveRange()
        {
            return moveRange;
        }

        public void SetMoveRange(int range)
        {
            moveRange = range;
        }

        public bool HasMovedThisTurn()
        {
            return movedThisTurn;
        }

        public void SetMovedThisTurn(bool b)
        {
            movedThisTurn = b;
        }

        public bool HasUsedAbilityThisTurn()
        {
            return usedAbilityThisTurn;
        }

        public void SetUsedAbilityThisTurn(bool b)
        {
            usedAbilityThisTurn = b;
        }

        public bool IsAlive()
        {
            return alive;
        }

        public void SetAlive(bool b)
        {
            alive = b;
        }
    }
}
