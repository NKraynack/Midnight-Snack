using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CS_4730_Prototype
{
    public class MapTile : GameObject
    {
        private GameObject occupant;  //The object currently occupying this tile
        private bool passable;  //Is this tile passable
        private bool lair;    //Is this a lair tile

        public MapTile(int x, int y, int width, int height) : base(x, y, width, height)
        {
            occupant = null;
            passable = true;
            lair = false;
        }

        public MapTile(Vector2 pos, int width, int height) : base(pos, width, height)
        {
            occupant = null;
            passable = true;
            lair = false;
        }

        public void LoadContent(ContentManager content)
        {
            if (passable)
            {
                if (lair)
                {
                    Texture = content.Load<Texture2D>("lair_map_tile.png");
                }
                else
                {
                    Texture = content.Load<Texture2D>("map_tile_border.png");
                }
            }
            else
            {
                Texture = content.Load<Texture2D>("unpassable_map_tile.png");
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

        public bool IsLair()
        {
            return lair;
        }

        public void SetLair(bool b)
        {
            lair = b;
        }
    }
}
