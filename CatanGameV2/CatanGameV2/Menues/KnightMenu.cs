using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class KnightMenu : XNACS1Rectangle
    {
        private float fWidth = Global.PixelSize(424);
        private float fHeight = Global.PixelSize(87);
        Vector2 oDisplacement = new Vector2(50f, 35f);

        enum STATE { NONE };

        XNACS1Rectangle[] oButtons = new XNACS1Rectangle[2];
        Vector2[] oButtonDisplacements = new Vector2[2];
        Color oClickColor = new Color(150, 150, 150);
        Color oUnavailableColor = new Color(125, 125, 125);
        Color oOriginalColor = new Color(256, 256, 256);

        XNACS1Rectangle oMessage;
        Vector2 oMessageDisplacement = new Vector2(50f, 25f);
        float fMessageWidth = 100f;
        float fMessageHeight = 5f;

        int nRoadsLeft = 0;
        public bool bFinished = false;
        public bool bCanBuildRoads = true;

        public enum State { MENU, MOVING_ROBBER, ROBBING_PLAYER };
        public State eState = State.MENU;

        public KnightMenu()
        {
            this.Width = fWidth;
            this.Height = fHeight;
            this.Center = new Vector2(50f, 30f);
            this.Texture = "KnightMenu";

            for (int x = 0; x < oButtons.Count(); x++)
            {
                oButtonDisplacements[x] = new Vector2(-7.4f + 15f * x, -2.11f);
                oButtons[x] = new XNACS1Rectangle(new Vector2(), Global.PixelSize(111), Global.PixelSize(34));
                oButtons[x].UseSpriteSheet = true;
                oButtons[x].SetTextureSpriteSheet("TradeMenuButtons1", 3, 1, 0);
                oButtons[x].SetTextureSpriteAnimationFrames(x, 0, x, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                oButtons[x].LabelColor = Color.White;
            }
            oButtons[0].Label = "Accept";
            oButtons[1].Label = "Cancel";
        }

        public void Update(Cursor oCursor, Camera oCamera, HUD oHUD, Player[] oPlayers, Robber oRobber)
        {
            if (eState == State.MENU)
            {
                #region Accept Button
                if (oCursor.LeftClick())
                {
                    if (oButtons[0].Collided(oCursor.oHitbox))
                    {
                        //bFinished = true;
                        Hide();
                        oRobber.bMoveRobber = true;
                        oPlayers[0].eCurrentlyBuilding = Player.CurrentlyBuilding.ROAD_BUILDING;
                        eState = State.MOVING_ROBBER;
                        oMessage = new XNACS1Rectangle(new Vector2(), fMessageWidth, fMessageHeight);
                        oMessage.Color = new Color(0, 0, 0, 150);
                        oMessage.Label = "Move the robber";
                        oMessage.LabelColor = Color.White;
                    }
                }
                #endregion

                #region Cancel Button
                if (oCursor.LeftClick())
                {
                    if (oCursor.oHitbox.Collided(oButtons[1]))
                    {
                        bFinished = true;
                    }
                }
                #endregion
            }
            else if (eState == State.MOVING_ROBBER)
            {
                if (oRobber.bMoveRobber == false && oRobber.bPickedUp == false)
                {
                    oMessage.Label = "Rob a player";
                    eState = State.ROBBING_PLAYER;
                }
            }
            else if (eState == State.ROBBING_PLAYER)
            {
                if (oPlayers[0].bCanBeRobbed == false &&
                    oPlayers[1].bCanBeRobbed == false &&
                    oPlayers[2].bCanBeRobbed == false &&
                    oPlayers[3].bCanBeRobbed == false)
                {
                    bFinished = true;
                }
            }
            AlignWithCamera(oCamera);
        }

        public void AlignWithCamera(Camera oCamera)
        {
            this.Center = oDisplacement + oCamera.oCenter;

            for (int x = 0; x < oButtons.Count(); x++)
                oButtons[x].Center = this.Center + oButtonDisplacements[x];

            if (oMessage != null)
                oMessage.Center = oMessageDisplacement + oCamera.oCenter;

        }

        public void Hide()
        {
            this.Visible = false;

            for (int x = 0; x < oButtons.Count(); x++)
                oButtons[x].Visible = false;
        }

        public void Clear()
        {
            this.RemoveFromAutoDrawSet();

            for (int x = 0; x < oButtons.Count(); x++)
                oButtons[x].RemoveFromAutoDrawSet();

            if (oMessage != null)
                oMessage.RemoveFromAutoDrawSet();
        }
    }
}
