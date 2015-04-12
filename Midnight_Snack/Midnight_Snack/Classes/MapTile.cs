using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Midnight_Snack
{
    public class MapTile : GameObject
    {
        private Unit occupant;  //The object currently occupying this tile
        private bool passable;  //Is this tile passable
        private string modifier;    //The modifier on this tile (i.e. lair, consecrated, etc.)

        Texture2D lair_texture;
        Texture2D consec_texture;
        Texture2D garlic_texture;
        Texture2D valid_texture;
        Texture2D valid_lair_texture;

        public MapTile(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            occupant = null;
            passable = true;
            modifier = "none";
        }

        public MapTile(Vector2 pos, int width, int height)
            : base(pos, width, height)
        {
            occupant = null;
            passable = true;
            modifier = "none";
        }

        public override void LoadContent(ContentManager content)
        {
            lair_texture = content.Load<Texture2D>("lair_map_tile");
            consec_texture = content.Load<Texture2D>("consecrated_ground_map_tile");
            garlic_texture = content.Load<Texture2D>("garlic_map_tile");
            texture = content.Load<Texture2D>("map_tile_border");
            valid_texture = content.Load<Texture2D>("valid_move_map_tile");
            valid_lair_texture = content.Load<Texture2D>("valid_move_lair_map_tile");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (modifier.Equals("lair"))
            {
                spriteBatch.Draw(lair_texture, position, Color.White);
            }
            else if (modifier.Equals("consecrated"))
            {
                spriteBatch.Draw(consec_texture, position, Color.White);
            }
            else if (modifier.Equals("garlic"))
            {
                spriteBatch.Draw(garlic_texture, position, Color.White);
            }
            else if (modifier.Equals("move"))
            {
                spriteBatch.Draw(valid_texture, position, Color.White);
            }
            else if (modifier.Equals("move_lair"))
            {
                spriteBatch.Draw(valid_lair_texture, position, Color.White);
            }
            else
            {
                spriteBatch.Draw(texture, position, Color.White);
            }

        }

        public Unit GetOccupant()
        {
            return occupant;
        }

        public void SetOccupant(Unit o)
        {
            occupant = o;
        }

        public bool IsPassable()
        {
            return passable;
        }

        public void SetPassable(bool b)
        {
            passable = b;
        }

        public string GetModifier()
        {
            return modifier;
        }

        public void SetModifier(string mod)
        {
            modifier = mod;
        }
    }
}
