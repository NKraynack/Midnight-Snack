﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Midnight_Snack
{
    public class Player : MobileUnit
    {
        private bool hasBlood;   //Has the player collected blood

        private static Player instance = new Player(new Vector2(0, 0), 100, 100, 0, 0, 3, 10);

        public Player(Vector2 pos, int width, int height, int row, int col, int range, int health) 
            : base(pos, width, height, row, col, range, health)
        {
            hasBlood = false;
        }

        public static Player GetInstance()
        {
            return instance;
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("dracula.png");
        }

        public bool HasBlood()
        {
            return hasBlood;
        }

        public void SetHasBlood(bool b)
        {
            hasBlood = b;
        }

        //Tinge the player red if they have blood
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (hasBlood)
            {
                spriteBatch.Draw(texture, position, Color.Red);
            }
            else
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
        }

        /*
        public Map GetMap()
        {
            return map;
        }

        public void SetMap(Map m)
        {
           this.map = m;
        }
         * */


    }
}