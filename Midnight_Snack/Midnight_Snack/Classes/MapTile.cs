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

        public MapTile(int x, int y, int width, int height) : base(x, y, width, height)
        {
            occupant = null;
            passable = true;
            modifier = "none";
        }

        public MapTile(Vector2 pos, int width, int height) : base(pos, width, height)
        {
            occupant = null;
            passable = true;
            modifier = "none";
        }

        public override void LoadContent(ContentManager content)
        {
            if (modifier.Equals("lair"))
            {
                texture = content.Load<Texture2D>("lair_map_tile");
            }
            else if (modifier.Equals("consecrated"))
            {
                texture = content.Load<Texture2D>("consecrated_ground_map_tile");
            }
            else if (modifier.Equals("garlic"))
            {
                texture = content.Load<Texture2D>("garlic_map_tile");
            }
            else
            {
                texture = content.Load<Texture2D>("map_tile_border");
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
