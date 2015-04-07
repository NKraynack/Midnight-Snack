using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Midnight_Snack
{
    public class VampireEnemy : Enemy
    {
        Player player = Player.GetInstance();

        public VampireEnemy(Vector2 pos, int width, int height, int row, int col, int range, int health, Map map)
            : base(pos, width, height, row, col, range, health, map)
        {
            this.map_grid = map.GenerateMapGrid();
        }

        public override void LoadContent(ContentManager content)
        {
            //temp until I either draw one or find one
            texture = content.Load<Texture2D>("dracula");
            healthBar.LoadContent(content);
        }

        public override void Update()
        {
            base.Update();

            if (!alive)
            {
                //Remove enemy from play
                map.GetTile(this.GetRow(), this.GetCol()).SetOccupant(null);
                //End enemy's turn
                hasEndedTurn = true;
            }
            else
            {
                //If it's this enemy's turn, have it move and use an ability (if possible)
                if (unitsTurn)
                {
                    Debug.WriteLine("Vampire Enemy Turn");

                    //Use any relevant abilities
                    this.UseAbilities();

                    //Handle enemy movement
                    int[] destCoords = GetDestination();
                    MapTile dest = map.GetTile(destCoords[0], destCoords[1]);
                    EnemyMove(destCoords[0], destCoords[1], dest);

                    //If did not use any abilities before moving, try now
                    this.UseAbilities();

                    //End enemy's turn
                    hasEndedTurn = true;
                }
            }

        }

        public void Feed(SleepingVillager target)
        {
            //Villager must not already have been drained
            if (!target.IsDrained())
            {
                //Update the villager as drained
                target.SetDrained(true);
                //Update that vampire has used an ability this turn
                this.SetUsedAbilityThisTurn(true);
            }
        }

        public override void UseAbilities()
        {
            //Feed on an adjacent villager if possible
            if (this.GetAdjacentVillager() != null && !this.HasUsedAbilityThisTurn())
            {
                Debug.WriteLine("Vampire feeding on villager");
                Feed(this.GetAdjacentVillager());
            }
            //Attack the player if adjacent
            else if (this.AdjacentToPlayer() && !this.HasUsedAbilityThisTurn())
            {
                Debug.WriteLine("Vampire attacking player!");
                //insert attack method
                Attack(player);
            }
        }

        //Returns a villager adjacent to the unit if there is one
        //If there is not an adjacent villager, returns null
        public SleepingVillager GetAdjacentVillager()
        {
            if (this.GetRow() - 1 > -1)
            {
                if (map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy below something");
                    if (map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Enemy below villager");
                        return (SleepingVillager) map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant();
                    }
                }
            }
            if (this.GetRow() + 1 >= map.GetNumRows())
            {
                if (map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy above something");
                    if (map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Enemy above villager");
                        return (SleepingVillager) map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant();
                    }
                }
            }

            if (this.GetCol() - 1 > -1)
            {
                if (map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy right of something");
                    if (map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Enemy right of villager");
                        return (SleepingVillager) map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant();
                    }
                }
            }

            if (this.GetCol() + 1 >= map.GetNumRows())
            {
                if (map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant() != null)
                {
                    Debug.WriteLine("Enemy left of something");
                    if (map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant().GetType() == typeof(SleepingVillager))
                    {
                        Debug.WriteLine("Enemy left of villager");
                        return (SleepingVillager) map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant();
                    }
                }
            }
            return null;
        }

    }
}
