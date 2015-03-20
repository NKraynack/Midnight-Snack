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
        private GameObject occupant;  //The object currently occupying this tile
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

        public void LoadContent(ContentManager content)
        {
            if (passable)
            {
                if (modifier.Equals("lair"))
                {
                    texture = content.Load<Texture2D>("lair_map_tile.png");
                }
                else
                {
                    texture = content.Load<Texture2D>("map_tile_border.png");
                }
            }
            else
            {
                texture = content.Load<Texture2D>("unpassable_map_tile.png");
            }
        }

        public GameObject GetOccupant()
        {
            return occupant;
        }

        public void SetOccupant(GameObject o)
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
