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
    public class Cursor : GameObject
    {
        private int cursorRow;  //The row of the grid which the cursor is currently on
        private int cursorCol;  //The col of the grid which the cursor is currently on
        private Map map;    //The map the cursor is on
        private int maxRow; //The Bottom-most row in the grid
        private int maxCol; //The right-most column in the grid

        GameManager gameManager = GameManager.GetInstance();
        Player player = Player.GetInstance();

        public Cursor(int x, int y, int width, int height, Map map) : base(x, y, width, height)
        {
            this.map = map;
            cursorRow = map.GetLairRow();
            cursorCol = map.GetLairCol();
            maxRow = map.GetNumRows() - 1;
            maxCol = map.GetNumCols() - 1;

        }

        public Cursor(Vector2 pos, int width, int height, Map map) : base(pos, width, height)
        {
            this.map = map;
            cursorRow = map.GetLairRow();
            cursorCol = map.GetLairCol();
            maxRow = map.GetNumRows() - 1;
            maxCol = map.GetNumCols() - 1;
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("cursor.png");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //If not moving player display cursor normally
            if (!gameManager.IsMovingPlayer())
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
            //When moving player, color cursor red when out of move range
            //or tile is invalid
            else
            {
                MapTile tile = map.GetTile(cursorRow, cursorCol);
                if (Math.Abs(cursorRow - player.GetRow()) + Math.Abs(cursorCol - player.GetCol()) > player.GetMoveRange()
                    || tile.GetOccupant() != null || !tile.IsPassable())
                {
                    spriteBatch.Draw(texture, position, Color.Red);
                }
                //If move is valid, then color normally
                else
                {
                    spriteBatch.Draw(texture, position, Color.White);
                }
            }
        }

        public void Update(Controls controls)
        {
            if (gameManager.IsChoosingInteractTarget())
            {
                SelectInteractTile(controls);
            }
            //If not in the action menu, cursor should move around map
            else if (!gameManager.IsInActionMenu() && !gameManager.IsChoosingInteractTarget())
            {
                Move(controls);
                SelectTile(controls);

                if (gameManager.IsMovingPlayer())
                {
                    MovePlayer(controls);
                }
            }
        }

        //Move the cursor around the map
        public void Move(Controls controls)
        {
            if (controls.onPress(Keys.Right, Buttons.DPadRight) && cursorCol != maxCol)
            {
                SetX(GetX() + width);
                cursorCol++;
            }
            else if (controls.onPress(Keys.Left, Buttons.DPadLeft) && cursorCol != 0)
            {
                SetX(GetX() - width);
                cursorCol--;
            }
            else if (controls.onPress(Keys.Up, Buttons.DPadUp) && cursorRow != 0)
            {
                SetY(GetY() - height);
                cursorRow--;
            }
            else if (controls.onPress(Keys.Down, Buttons.DPadDown) && cursorRow != maxRow)
            {
                SetY(GetY() + height);
                cursorRow++;
            }
        }

        //Moves the player to a valid tile
        public void MovePlayer(Controls controls)
        {
            if (controls.onPress(Keys.Space, Buttons.A) && gameManager.IsMovingPlayer())
            {
                //If player chooses a valid tile within their move range, let them move there
                if (Math.Abs(cursorRow - player.GetRow()) + Math.Abs(cursorCol - player.GetCol()) <= player.GetMoveRange())
                {
                    //Check if tile is unoccupied and passable
                    MapTile tile = map.GetTile(cursorRow, cursorCol);
                    if (tile.GetOccupant() == null && tile.IsPassable())
                    {
                        //Move player to tile
                        player.Move(position, cursorRow, cursorCol);
                        //Update that player has moved this turn
                        player.SetMovedThisTurn(true);
                        gameManager.SetMovingPlayer(false);
                        gameManager.SetInActionMenu(false);
                    }
                }
            }
        }

        public void SelectTile(Controls controls)
        {
            //Get the occupant of the selected tile
            MapTile tile = map.GetTile(cursorRow, cursorCol);
            GameObject occupant = tile.GetOccupant();

            //If selecting the tile that the player is on...
            if (controls.onPress(Keys.Space, Buttons.A) 
                && cursorRow == player.GetRow() && cursorCol == player.GetCol())
            {
                //...open up action menu
                gameManager.SetInActionMenu(true);
            }
        }

        //Can only choose to interact with a tile directly adjacent to player
        public void SelectInteractTile(Controls controls)
        {
            int maxRight = player.GetCol() + 1;
            int maxLeft = player.GetCol() - 1;
            int maxUp = player.GetRow() - 1;
            int maxDown = player.GetRow() + 1;

            if (controls.onPress(Keys.Right, Buttons.DPadRight) && cursorCol != maxCol && cursorCol < maxRight)
            {
                SetX(GetX() + width);
                cursorCol++;
            }
            else if (controls.onPress(Keys.Left, Buttons.DPadLeft) && cursorCol != 0 && cursorCol > maxLeft)
            {
                SetX(GetX() - width);
                cursorCol--;
            }
            else if (controls.onPress(Keys.Up, Buttons.DPadUp) && cursorRow != 0 && cursorRow > maxUp)
            {
                SetY(GetY() - height);
                cursorRow--;
            }
            else if (controls.onPress(Keys.Down, Buttons.DPadDown) && cursorRow != maxRow && cursorRow < maxDown)
            {
                SetY(GetY() + height);
                cursorRow++;
            }

            //Get the occupant of the selected tile
            MapTile tile = map.GetTile(cursorRow, cursorCol);
            GameObject occupant = tile.GetOccupant();

            //If the player chooses a tile and there is a valid interact target...
            if (controls.onPress(Keys.Space, Buttons.A) && occupant != null)
            {
                //...interact with that target
                player.SetHasBlood(true);
                //Update that player has interacted this turn
                player.SetUsedAbilityThisTurn(true);
                gameManager.SetChoosingInteractTarget(false);
            }
            //If player cancels the interact select, exit interact select mode
            else if (controls.onPress(Keys.F, Buttons.B))
            {
                gameManager.SetChoosingInteractTarget(false);
            }
        }

        /*
        public void SelectAction(Controls controls)
        {
            //Select an action
            //Only option right now is move
            if (controls.onPress(Keys.Space, Buttons.A))
            {
                gst.SetMovingPlayer(true);
                gst.SetInActionMenu(false);
            }
            else if (controls.onPress(Keys.F, Buttons.B))
            {
                gst.SetInActionMenu(false);
            }
        }
         * */
    }
}
