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
        private bool lit;           //Is this tile lit up (by player move range)

        Texture2D lair_texture;
        Texture2D consec_texture;
        Texture2D garlic_texture;
        Texture2D move_texture;
        Texture2D move_lair_texture;
        Texture2D move_garlic_texture;
        Texture2D enemy_move_lair_texture;

        public MapTile(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            occupant = null;
            passable = true;
            modifier = "none";
            lit = false;
        }

        public MapTile(Vector2 pos, int width, int height)
            : base(pos, width, height)
        {
            occupant = null;
            passable = true;
            modifier = "none";
            lit = false;
        }

        public override void LoadContent(ContentManager content)
        {
            lair_texture = content.Load<Texture2D>("lair_map_tile");
            consec_texture = content.Load<Texture2D>("consecrated_ground_map_tile");
            garlic_texture = content.Load<Texture2D>("garlic_map_tile");
            texture = content.Load<Texture2D>("map_tile_border");
            move_texture = content.Load<Texture2D>("valid_move_map_tile");
            move_lair_texture = content.Load<Texture2D>("valid_move_lair_map_tile");
            move_garlic_texture = content.Load<Texture2D>("valid_move_garlic_map_tile");
            enemy_move_lair_texture = content.Load<Texture2D>("enemy_valid_move_map_tile");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (modifier.Equals("lair"))
            {
                if (lit)
                {
                    spriteBatch.Draw(move_lair_texture, position, Color.White);
                }
                else
                {
                    spriteBatch.Draw(lair_texture, position, Color.White);
                }
            }
            else if (modifier.Equals("consecrated"))
            {
                if (lit)
                {
                    spriteBatch.Draw(move_texture, position, Color.White);
                }
                else
                {
                    spriteBatch.Draw(consec_texture, position, Color.White);
                }
            }
            else if (modifier.Equals("garlic"))
            {
                if (lit)
                {
                    spriteBatch.Draw(move_garlic_texture, position, Color.White);
                }
                else
                {
                    spriteBatch.Draw(garlic_texture, position, Color.White);
                }
            }
            else if (modifier.Equals("enemy_move"))
            {
                if (lit)
                {
                    spriteBatch.Draw(enemy_move_lair_texture, position, Color.White);
                }
                else
                {
                    spriteBatch.Draw(garlic_texture, position, Color.White);
                }
            }
            else
            {
                if (lit)
                {
                    spriteBatch.Draw(move_texture, position, Color.White);
                }
                else
                {
                    spriteBatch.Draw(texture, position, Color.White);
                }
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

        public bool IsLit()
        {
            return lit;
        }

        public void SetLit(bool b)
        {
            lit = b;
        }
    }
}
