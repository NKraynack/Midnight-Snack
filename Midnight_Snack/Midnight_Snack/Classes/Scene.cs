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
    public class Scene
    {
        public Texture2D background;
        public List<Text> text;

        public Scene()
        {
            text = new List<Text>();
        }

        public virtual void LoadContent(ContentManager content)
        {
            //Load all text content
            for (int i = 0; i < text.Count; i++)
            {
                text[i].LoadContent(content);
            }
        }

        public virtual void Update(Controls controls)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //Draw all text content
            for (int i = 0; i < text.Count; i++)
            {
                text[i].Draw(spriteBatch);
            }
        }
    }
}
