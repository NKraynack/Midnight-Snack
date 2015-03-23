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
    public class HealthBar
    {
        private Vector2 position;
        private bool visible;
        private int currentHealth;
        private int maxHealth;
        private List<HealthPiece> health;

        public HealthBar(Vector2 pos, int maximumHealth)
        {
            position = pos;
            visible = true;
            maxHealth = maximumHealth;
            currentHealth = maxHealth;

            health = new List<HealthPiece>();
            int xOffset = 0;
            int yOffset = 0;
            for(int i = 0; i < maxHealth; i++)
            {
                HealthPiece pip = new HealthPiece(new Vector2(position.X + xOffset, position.Y + yOffset), 5, 10);
                health.Add(pip);
                xOffset += 5;
            }
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void SetPosition(Vector2 pos)
        {
            position = pos;
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

        public bool IsVisible()
        {
            return visible;
        }

        public void SetVisible(bool b)
        {
            visible = b;
        }

        public void LoadContent(ContentManager content)
        {
            for (int i = 0; i < health.Count; i++)
            {
                health[i].LoadContent(content);
            }
        }

        public void Update(Vector2 pos, int currHealth)
        {
            position = pos;
            currentHealth = currHealth;
            int xOffset = 0;
            int yOffset = -10;
            for (int i = 0; i < health.Count; i++)
            {
                //Update location of healthbar
                health[i].SetPosition(new Vector2(position.X + xOffset, position.Y + yOffset));
                xOffset += 5;

                //Display proper amount of health
                if(currentHealth > i)
                {
                    health[i].SetEmpty(false);
                }
                else
                {
                    health[i].SetEmpty(true);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                for (int i = 0; i < health.Count; i++)
                {
                    health[i].Draw(spriteBatch);
                }
            }
        }

    }
}
