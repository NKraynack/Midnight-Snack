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
    public class StatDisplay
    {
        private Vector2 position;
        private Texture2D backgroundTexture;
        private bool visible;
        private string unitName, unitDescription;
        private int maxHealth, currentHealth, attackStr;
        private Text name, description, health, attack;

        public StatDisplay(Vector2 pos)
        {
            position = pos;
            visible = false;
            unitName = "";
            unitDescription = "";
            maxHealth = 0;
            currentHealth = 0;
            attackStr = 0;

            name = new Text("name", pos);
            description = new Text("description", new Vector2(position.X, position.Y + 10));
            health = new Text("Health: currentHealth / maxHealth", new Vector2(position.X, position.Y + 20));
            attack = new Text("Attack Strength: ", new Vector2(position.X, position.Y + 30));
        }

        public void LoadContent(ContentManager content)
        {
            backgroundTexture = content.Load<Texture2D>("stat_display_background");
            name.LoadContent(content);
            description.LoadContent(content);
            health.LoadContent(content);
            attack.LoadContent(content);
        }

        public void Update(MobileUnit unit)
        {
            float positionX = unit.GetPosition().X;
            float positionY = unit.GetPosition().Y;
            position = new Vector2(positionX + 70, positionY);
            maxHealth = unit.GetMaxHealth();
            currentHealth = unit.GetCurrentHealth();
            attackStr = unit.GetStrength();

            if (unit.GetType() == typeof(ClericEnemy))
            {
                unitName = "Cleric";
                unitDescription = "Support spellcaster";
            }
            else if (unit.GetType() == typeof(HunterEnemy))
            {
                unitName = "Vampire Hunter";
                unitDescription = "Tracks down vampires";
            }
            else if (unit.GetType() == typeof(Player))
            {
                unitName = "You";
                unitDescription = "A powerful vampire";
            }
            else if (unit.GetType() == typeof(TownGuard))
            {
                unitName = "Town Guard";
                unitDescription = "Patroling guard";
            }
            else if (unit.GetType() == typeof(VampireEnemy))
            {
                unitName = "Rival Vampire";
                unitDescription = "Feeds on villagers";
            }

            name.SetMessage(unitName);
            name.SetPosition(new Vector2(position.X, position.Y));
            description.SetMessage(unitDescription);
            description.SetPosition(new Vector2(position.X, position.Y + 20));
            health.SetMessage("Health: " + currentHealth + "/" + maxHealth);
            health.SetPosition(new Vector2(position.X, position.Y + 40));
            attack.SetMessage("Attack Strength: " + attackStr);
            attack.SetPosition(new Vector2(position.X, position.Y + 60));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(backgroundTexture, position, Color.White);

                name.Draw(spriteBatch);
                description.Draw(spriteBatch);
                health.Draw(spriteBatch);
                attack.Draw(spriteBatch);
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

        public bool IsVisible()
        {
            return visible;
        }

        public void SetVisible(bool b)
        {
            visible = b;
        }
    }
}
