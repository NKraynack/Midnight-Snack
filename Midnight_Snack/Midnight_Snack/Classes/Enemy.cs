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
    public class Enemy : MobileUnit 
    {

        Player player = Player.GetInstance();

        public Enemy(Vector2 pos, int width, int height, int row, int col, int range, int health, Map map) 
            : base(pos, width, height, row, col, range, health, map)
        {
            
        }

        public override void LoadContent(ContentManager content)
        {
            //temp until I either draw one or find one
            texture = content.Load<Texture2D>("goomba.png");
            healthBar.LoadContent(content);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (alive)
            {
                base.Draw(spriteBatch);

                spriteBatch.Draw(texture, position, Color.White);
            }
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
                    //For testing purposes enemy just moves one tile to the left each turn
                    Debug.WriteLine("Enemy Turn");
                    if (this.AdjacentToPlayer())
                    {
                        Debug.WriteLine("Attacking player!");
                        //insert attack method
                        Attack(player);
                    }
                    //End enemy's turn
                    hasEndedTurn = true;
                }
            }
 
        }

        public bool AdjacentToPlayer()
        {
            if (map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant() != null)
            {
                Debug.WriteLine("Enemy below something");
                if (map.GetTile(this.GetRow() - 1, this.GetCol()).GetOccupant().GetType() == typeof(Player))
                {
                    Debug.WriteLine("Enemy below player");
                    return true;
                }
            }
            else if (map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant() != null)
            {
                Debug.WriteLine("Enemy above something");
                if (map.GetTile(this.GetRow() + 1, this.GetCol()).GetOccupant().GetType() == typeof(Player))
                {
                    Debug.WriteLine("Enemy above player");
                    return true;
                }
            }
            else if (map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant() != null)
            {
                Debug.WriteLine("Enemy right of something");
                if (map.GetTile(this.GetRow(), this.GetCol() - 1).GetOccupant().GetType() == typeof(Player))
                {
                    Debug.WriteLine("Enemy right of player");
                    return true;
                }
            }
            else if (map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant() != null)
            {
                Debug.WriteLine("Enemy left of something");
                if (map.GetTile(this.GetRow(), this.GetCol() + 1).GetOccupant().GetType() == typeof(Player))
                {
                    Debug.WriteLine("Enemy left of player");
                    return true;
                }
            }

            return false;
        }

        public override void Attack(MobileUnit target)
        {
            //Target must still be alive
            if (target.IsAlive())
            {
                //Update the target's health
                int targetHealth = target.GetCurrentHealth() - 3;
                target.SetCurrentHealth(targetHealth);
                //Updated map tile of target
                MapTile tile = map.GetTile(target.GetRow(), target.GetCol());
                tile.SetOccupant(target);
                //Update that unit has used an ability this turn
                this.SetUsedAbilityThisTurn(true);
            }
        }
    }
}
