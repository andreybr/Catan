using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class Robber : XNACS1Rectangle
    {
        public float fWidth = Global.PixelSize(30); // width of image
        public float fHeight = Global.PixelSize(30); // height of image
        Vector2 oOccupiedHex;
        public bool bMoveRobber = false;
        public bool bPickedUp = false;

        XNACS1Rectangle oMessage;
        Vector2 oMessageDisplacement = new Vector2(50f, 25f);
        float fMessageWidth = 100f;
        float fMessageHeight = 5f;

        public enum State { NONE, MOVING_ROBBER, ROBBING_PLAYER };
        public State eState = State.NONE;

        public Robber(Vector2 oCenter, Vector2 oOccupiedHex)
        {
            this.Width = fWidth;
            this.Height = fHeight;
            this.Center = oCenter;
            this.Color = Color.Red;
            this.oOccupiedHex = oOccupiedHex;
        }

        public void Update(Hex[,] oHexes, Camera oCamera, Cursor oCursor, Stats oStats)
        {
            //XNACS1Base.EchoToTopStatus("HUD:" + bMoveRobber + " " + bPickedUp);

            #region Pick Up/Move Robber
            if (oCursor.LeftClick() == true)
            {
                if (bMoveRobber == true)
                {
                    if (oCursor.oHitbox.Collided(this))
                    {
                        bPickedUp = true;
                        bMoveRobber = false;
                    }
                }
            }
            if (bPickedUp == true)
            {
                Center = oCursor.oHitbox.Center;
            }
            #endregion

            #region Place Robber
            if (bPickedUp == true && oCursor.IsLeftMouseDown() == false)
            {
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        if (oHexes[x, y] != null)
                        {
                            if (oCursor.oHitbox.Collided(oHexes[x, y]))
                            {
                                oHexes[x, y].bRobber = true;
                                bPickedUp = false;
                                oHexes[(int)oOccupiedHex.X, (int)oOccupiedHex.Y].bRobber = false;
                                oOccupiedHex = new Vector2(x, y);
                                this.Center = oHexes[x,y].Center;

                                for (int z = 0; z < oHexes[x, y].oIntersections.Count(); z++)
                                {
                                    if (oHexes[x, y].oIntersections[z].nOwner == oStats.nCurrentPlayer)
                                    {
                                        continue;
                                    }

                                    oStats.oPlayers[1].bCanBeRobbed = true;
                                    oStats.oPlayers[2].bCanBeRobbed = true;
                                    oStats.oPlayers[3].bCanBeRobbed = true;
                                    //else if (oHexes[x, y].oIntersections[z].nOwner > -1 && 
                                    //         oStats.oPlayers[oHexes[x, y].oIntersections[z].nOwner].nResourceCardCount > 0)
                                    //{
                                    //    oStats.oPlayers[oHexes[x, y].oIntersections[z].nOwner].bCanBeRobbed = true;
                                    //}
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            if (oMessage != null)
            {
                if (bMoveRobber == true)
                {
                    oMessage.Label = "Move the robber";
                }
                else if (bMoveRobber == false && bPickedUp == false)
                {
                    oMessage.Label = "Rob a player";

                    if (oStats.oPlayers[0].bCanBeRobbed == false &&
                        oStats.oPlayers[1].bCanBeRobbed == false &&
                        oStats.oPlayers[2].bCanBeRobbed == false &&
                        oStats.oPlayers[3].bCanBeRobbed == false)
                    {
                        EraseMessage();
                    }
                }
            }
            
            if (oMessage != null)
                AlignWithCamera(oCamera);
        }

        public void AlignWithCamera(Camera oCamera)
        {
            oMessage.Center = oCamera.oCenter + oMessageDisplacement;
        }

        public void ShowMessage()
        {
            oMessage = new XNACS1Rectangle(new Vector2(), fMessageWidth, fMessageHeight);
            oMessage.Color = new Color(0, 0, 0, 150);
            oMessage.Label = "Move the robber";
            oMessage.LabelColor = Color.White;
        }

        public void EraseMessage()
        {
            oMessage.RemoveFromAutoDrawSet();
            oMessage = null;
        }
    }
}
