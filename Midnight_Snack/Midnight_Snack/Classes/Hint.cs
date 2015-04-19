using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Midnight_Snack
{
    public class Hint : GameObject
    {
        private int row, col;
        private Text hintText;
        private bool visible;

        Player player = Player.GetInstance();

        public Hint(Vector2 pos, int width, int height, int row, int col, string text) : base(pos, width, height)
        {
            this.row = row;
            this.col = col;

            int xOffset = 5;
            int yOffset = 5;
            hintText = new Text(text, new Vector2(position.X + xOffset, position.Y + yOffset));
            visible = false;
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("hint_box");
            hintText.LoadContent(content);
        }

        public void Update()
        {
            //If player is standing on the proper tile, display hint
            if (player.GetRow() == row && player.GetCol() == col)
            {
                visible = true;
            }
            else
            {
                visible = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, position, Color.White);
                hintText.Draw(spriteBatch);
            }
        }

    }
}
