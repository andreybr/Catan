using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class Intersection : XNACS1Rectangle
    {
        public Vector2 oHexCenter;
        //public Building oBuilding; // old
        //public Building[] oNearbyBuildings = new Building[3]; // old
        public Intersection[] oNearbyBuildings = new Intersection[3];
        public Road[] oNearbyRoads = new Road[3];
        public Vector2[] oBuildingDisplacements = new Vector2[6]; // building position in relation to hex 
        public Vector2[] oRoadDisplacements = new Vector2[6]; // road position in relation to hex 
        public float[] oAngles = new float[6];
        //public bool[] bPorts = { false, false, false, false, false, false }; // ports connected to this intersection
        public int nPortType = -1;
        //public bool bHasPorts = false;

        private float fSettlementWidth = (float)Global.PixelSize(18);  // width of image
        private float fSettlementHeight = (float)Global.PixelSize(29); // height of image
        private float fCityWidth = (float)Global.PixelSize(28);  // width of image
        private float fCityHeight = (float)Global.PixelSize(30); // height of image
        public int nOwner = -1; // player who owns this road, -1 = not owned
        public enum STATE { NONE, SETTLEMENT, CITY }
        public STATE eState = STATE.NONE;
        public Color oUnselectedColor = new Color(0, 0, 0, 160);
        public Color oHoverColor = new Color(0, 0, 0, 160);
        public int nCount;
        public int nUsed;
        public string str;
        public int nID;

        public Intersection(int nID)
        {
            this.nID = nID;
        }

        public void Create(Vector2 oHexCenter, string str)
        {
            this.oHexCenter = oHexCenter;
            oBuildingDisplacements[0] = new Vector2(2.9f, 5.11f);
            oBuildingDisplacements[1] = new Vector2(-3.2f, 5.11f);
            oBuildingDisplacements[2] = new Vector2(-6.2f, 0.5f);
            oBuildingDisplacements[3] = new Vector2(-3.2f, -4.15f);
            oBuildingDisplacements[4] = new Vector2(2.9f, -4.15f);
            oBuildingDisplacements[5] = new Vector2(6.2f, 0f);
            oRoadDisplacements[0] = new Vector2(0f, 4.85f);
            oRoadDisplacements[1] = new Vector2(-4.85f, 2.7f);
            oRoadDisplacements[2] = new Vector2(-4.75f, -2.15f);
            oRoadDisplacements[3] = new Vector2(0f, -4.4f);
            oRoadDisplacements[4] = new Vector2(4.75f, -2.15f);
            oRoadDisplacements[5] = new Vector2(4.85f, 2.7f);
            oAngles[0] = 90f;
            oAngles[1] = 144f;
            oAngles[2] = 216f;
            oAngles[3] = 270f;
            oAngles[4] = 330f;
            oAngles[5] = 34f;

            this.Width = fSettlementWidth;
            this.Height = fSettlementHeight;
            this.LabelColor = Color.White;
            this.LabelFont = "Small";
            //this.Color = oUnselectedColor;
            this.Visible = false;
            this.Texture = "Settlement3";

            // test
            //this.TextureTintColor = Color.Red;
        }

        public void Update(Cursor oCursor, Camera oCamera, int nPosition, Intersection[] oIntersections, Stats oStats, HUD oHUD, XNACS1Rectangle oPort)
        {
            #region Update Roads
            for (int x = 0; x < oNearbyRoads.Count(); x++)
               if(oNearbyRoads[x] != null)
                   oNearbyRoads[x].Update();
            #endregion

            #region Build Settlement/City
            if (nOwner == -1) // not owned by a player
            {
                if(oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding == Player.CurrentlyBuilding.SETTLEMENT) // build settlement button was clicked
                {
                    if(oCursor.oHitbox.Collided(this)) // cursor on this building
                    {
                        if ((oNearbyBuildings[0] == null || oNearbyBuildings[0].nOwner == -1) && // no nearby owned buildings
                            (oNearbyBuildings[1] == null || oNearbyBuildings[1].nOwner == -1) &&
                            (oNearbyBuildings[2] == null || oNearbyBuildings[2].nOwner == -1))
                        {
                            if ((oNearbyRoads[0] != null && oNearbyRoads[0].nOwner == oStats.nCurrentPlayer) || // player owns nearby road
                                (oNearbyRoads[1] != null && oNearbyRoads[1].nOwner == oStats.nCurrentPlayer) ||
                                (oNearbyRoads[2] != null && oNearbyRoads[2].nOwner == oStats.nCurrentPlayer))
                            {
                                Visible = true;
                                this.TextureTintColor = oHoverColor;
                                if(oCursor.LeftClick())
                                {
                                    if(nPortType > -1)
                                        oPort.TextureTintColor = oStats.oPlayers[oStats.nCurrentPlayer].oPlayerColor;

                                    this.TextureTintColor = oStats.oPlayers[oStats.nCurrentPlayer].oPlayerColor;
                                    this.nOwner = oStats.nCurrentPlayer;
                                    oStats.oPlayers[oStats.nCurrentPlayer].nSettlements++;
                                    oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding = Player.CurrentlyBuilding.NONE;
                                    oStats.oPlayers[oStats.nCurrentPlayer].BuildSettlement();
                                    oHUD.ShowCards(oStats.oPlayers[oStats.nCurrentPlayer].nCards, oCamera);
                                    oHUD.ChangeSelectedButton(oStats);
                                    eState = STATE.SETTLEMENT;
                                }
                            }
                        }
                    }
                    else
                        Visible = false;
                }
            }

            if(nOwner == oStats.nCurrentPlayer) // owned
            {
                if(oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding == Player.CurrentlyBuilding.CITY) 
                {
                    if (oCursor.oHitbox.Collided(this)) // cursor on this building
                    {
                        if (eState == STATE.SETTLEMENT) // this structure is a settlement
                        {
                            if (oCursor.LeftClick())
                            {
                                Width = fCityWidth;
                                Height = fCityHeight;
                                this.Texture = "City";

                                oStats.oPlayers[oStats.nCurrentPlayer].nSettlements--;
                                oStats.oPlayers[oStats.nCurrentPlayer].nCities++;
                                oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding = Player.CurrentlyBuilding.NONE;
                                oStats.oPlayers[oStats.nCurrentPlayer].BuildCity();
                                oHUD.ShowCards(oStats.oPlayers[oStats.nCurrentPlayer].nCards, oCamera);
                                oHUD.ChangeSelectedButton(oStats);
                                eState = STATE.CITY;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Build Road
            int nIndex;
            int nOffIndex;

            if (nPosition == 0)
                nIndex = 2;
            else if (nPosition == 1)
                nIndex = 2;
            else if (nPosition == 2)
                nIndex = 1;
            else if (nPosition == 3)
                nIndex = 1;
            else if (nPosition == 4)
                nIndex = 0;
            else
                nIndex = 0;

            if (nPosition == 0)
                nOffIndex = 0;
            else if (nPosition == 1)
                nOffIndex = 0;
            else if (nPosition == 2)
                nOffIndex = 2;
            else if (nPosition == 3)
                nOffIndex = 2;
            else if (nPosition == 4)
                nOffIndex = 1;
            else
                nOffIndex = 1;

            if (oNearbyRoads[nIndex].nOwner > -1)
            {
                oNearbyRoads[nIndex].Visible = true;
            }
            else if (oNearbyRoads[nIndex] != null)
            {
                if (oCursor.oHitbox.Collided(oNearbyRoads[nIndex])) // click
                {
                    if (oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding == Player.CurrentlyBuilding.ROAD) // road button is selected
                    {
                        oNearbyRoads[nIndex].Visible = true;
                        oNearbyRoads[nIndex].TextureTintColor = oHoverColor;

                        if (oCursor.LeftClick())
                        {
                            oNearbyRoads[nIndex].nOwner = oStats.nCurrentPlayer;
                            oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding = Player.CurrentlyBuilding.NONE;

                            if (oStats.oPlayers[oStats.nCurrentPlayer].nFreeRoads == 0)
                            {
                                oStats.oPlayers[oStats.nCurrentPlayer].BuildRoad();
                                oHUD.ShowCards(oStats.oPlayers[oStats.nCurrentPlayer].nCards, oCamera);
                                oNearbyRoads[nIndex].TextureTintColor = oStats.oPlayers[oStats.nCurrentPlayer].oPlayerColor;
                            }
                            else
                            {
                                oStats.oPlayers[oStats.nCurrentPlayer].nFreeRoads--;
                            }

                            oHUD.ChangeSelectedButton(oStats);
                        }
                    }
                }
                else
                {
                    oNearbyRoads[nIndex].Visible = false;
                }
            }
            #endregion

            //test stuff
            //this.Label = "" + nOwner;
            //this.Visible = true;
        }

        private bool CanBuildRoad(Intersection[] oIntersections, int nPosition)
        {
            return true; // test

            int nIndex;

            if (nPosition == 0)
                nIndex = 2;
            else if (nPosition == 1)
                nIndex = 2;
            else if (nPosition == 2)
                nIndex = 1;
            else if (nPosition == 3)
                nIndex = 1;
            else if (nPosition == 4)
                nIndex = 0;
            else
                nIndex = 0;

            for(int x = 0; x < oNearbyRoads.Count(); x++)
            {
                if (x == nIndex || oNearbyRoads[x] == null)
                    continue;

                if (oNearbyRoads[x].nOwner == 1)
                    return true;
            }

            nPosition++;
            if(nPosition > oIntersections.Count() - 1)
                nPosition = 0;

            if (nPosition == 0)
                nIndex = 2;
            else if (nPosition == 1)
                nIndex = 2;
            else if (nPosition == 2)
                nIndex = 1;
            else if (nPosition == 3)
                nIndex = 1;
            else if (nPosition == 4)
                nIndex = 0;
            else
                nIndex = 0;

            nIndex++;
            if (nIndex > oNearbyRoads.Count() - 1)
                nIndex = 0;

            for (int x = 0; x < oNearbyRoads.Count(); x++)
            {
                if (x == nIndex || oNearbyRoads[x] == null)
                    continue;

                if (oIntersections[nPosition].oNearbyRoads[x].nOwner == 1)
                    return true;
            }

            return false;
        }

        public void PositionStructures(int nPosition)
        {
            //oBuilding.Center = oBuildingDisplacements[nPosition] + oHexCenter; // old
            this.Center = oBuildingDisplacements[nPosition] + oHexCenter;

            if (nPosition == 0)
            {
                oNearbyRoads[2].Center = oHexCenter + oRoadDisplacements[nPosition];
                oNearbyRoads[2].UseSpriteSheet = true;
                oNearbyRoads[2].SetTextureSpriteSheet("Roads2", 3, 1, 0);
                oNearbyRoads[2].SetTextureSpriteAnimationFrames(nPosition % 3, 0, nPosition % 3, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                //oNearbyRoads[2].Center = oHexCenter + oRoadDisplacements[0];
                //oNearbyRoads[2].RotateAngle = oAngles[0];
            }
            else if (nPosition == 1)
            {
                oNearbyRoads[2].Center = oHexCenter + oRoadDisplacements[nPosition];
                oNearbyRoads[2].UseSpriteSheet = true;
                oNearbyRoads[2].SetTextureSpriteSheet("Roads2", 3, 1, 0);
                oNearbyRoads[2].SetTextureSpriteAnimationFrames(nPosition % 3, 0, nPosition % 3, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                //oNearbyRoads[2].Center = oHexCenter + oRoadDisplacements[1];
                //oNearbyRoads[2].RotateAngle = oAngles[1];
            }
            else if (nPosition == 2)
            {
                oNearbyRoads[1].Center = oHexCenter + oRoadDisplacements[nPosition];
                oNearbyRoads[1].UseSpriteSheet = true;
                oNearbyRoads[1].SetTextureSpriteSheet("Roads2", 3, 1, 0);
                oNearbyRoads[1].SetTextureSpriteAnimationFrames(nPosition % 3, 0, nPosition % 3, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                //oNearbyRoads[1].Center = oHexCenter + oRoadDisplacements[2];
                //oNearbyRoads[1].RotateAngle = oAngles[2];
            }
            else if (nPosition == 3)
            {
                oNearbyRoads[1].Center = oHexCenter + oRoadDisplacements[nPosition];
                oNearbyRoads[1].UseSpriteSheet = true;
                oNearbyRoads[1].SetTextureSpriteSheet("Roads2", 3, 1, 0);
                oNearbyRoads[1].SetTextureSpriteAnimationFrames(nPosition % 3, 0, nPosition % 3, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                //oNearbyRoads[1].Center = oHexCenter + oRoadDisplacements[3];
                //oNearbyRoads[1].RotateAngle = oAngles[3];
            }
            else if (nPosition == 4)
            {
                oNearbyRoads[0].Center = oHexCenter + oRoadDisplacements[nPosition];
                oNearbyRoads[0].UseSpriteSheet = true;
                oNearbyRoads[0].SetTextureSpriteSheet("Roads2", 3, 1, 0);
                oNearbyRoads[0].SetTextureSpriteAnimationFrames(nPosition % 3, 0, nPosition % 3, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                //oNearbyRoads[0].Center = oHexCenter + oRoadDisplacements[4];
                //oNearbyRoads[0].RotateAngle = oAngles[4];
            }
            else if (nPosition == 5)
            {
                oNearbyRoads[0].Center = oHexCenter + oRoadDisplacements[nPosition];
                oNearbyRoads[0].UseSpriteSheet = true;
                oNearbyRoads[0].SetTextureSpriteSheet("Roads2", 3, 1, 0);
                oNearbyRoads[0].SetTextureSpriteAnimationFrames(nPosition % 3, 0, nPosition % 3, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                //oNearbyRoads[0].Center = oHexCenter + oRoadDisplacements[5];
                //oNearbyRoads[0].RotateAngle = oAngles[5];
            }
        }
    }
}
