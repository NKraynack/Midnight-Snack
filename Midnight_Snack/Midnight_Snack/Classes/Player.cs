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
    public class Player : MobileUnit
    {
        private bool hasBlood;   //Has the player collected blood
        private string form;    //The form the player is currently in

        private Texture2D wolfTexture;  //The texture for wolf form
        private Texture2D mistTexture;  //The texture for mist form

        private static Player instance = new Player(new Vector2(0, 0), 100, 100, 0, 0, 3, 10, new Map(1, 1, 0, 0));

        public Player(Vector2 pos, int width, int height, int row, int col, int range, int health, Map map) 
            : base(pos, width, height, row, col, range, health, map)
        {
            hasBlood = false;
            form = "vampire";
        }

        public static Player GetInstance()
        {
            return instance;
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("dracula");
            wolfTexture = content.Load<Texture2D>("wolf");
            mistTexture = content.Load<Texture2D>("mist");
            healthBar.LoadContent(content);
        }

        public override void Update()
        {
            base.Update();

            if(!alive)
            {
                GameManager.GetInstance().SetPlayerAlive(false);
            }

        }

        public bool HasBlood()
        {
            return hasBlood;
        }

        public void SetHasBlood(bool b)
        {
            hasBlood = b;
        }

        public string GetForm()
        {
            return form;
        }

        public void SetForm(string f)
        {
            form = f;

            //Update move range according to form
            if (form.Equals("wolf"))
            {
                this.SetMoveRange(5);
            }
            else
            {
                this.SetMoveRange(3);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            healthBar.Draw(spriteBatch);

            Color spriteColor = Color.White;
            //Tinge the player red if they have blood
            if (hasBlood)
            {
                spriteColor = Color.Red;
            }

            //Draw the player in the appropriate form
            if (form.Equals("mist"))
            {
                spriteBatch.Draw(mistTexture, position, spriteColor);
            }
            else if (form.Equals("wolf"))
            {
                spriteBatch.Draw(wolfTexture, position, spriteColor);
            }
            else
            {
                spriteBatch.Draw(texture, position, spriteColor);
            }

        }

    }
}
