using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    class SmallHUD : XNACS1Rectangle
    {
        float fWidth = Global.PixelSize(217);
        float fHeight = Global.PixelSize(96);
        Vector2 oDisplacement;

        public int[] nScores = { 0, 0, 0, 0, 0 };
        XNACS1Rectangle[] oScores = new XNACS1Rectangle[5];
        Vector2[] oScoreDisplacments = new Vector2[5];
        public int nOwner;

        XNACS1Rectangle oName;
        Vector2 oNameDisplacement;

        public SmallHUD(Vector2 oDisplacement, int nOwner)
        {
            this.Width = fWidth;
            this.Height = fHeight;
            this.oDisplacement = oDisplacement;
            this.Texture = "SmallHUD2";
            this.nOwner = nOwner;

            for (int x = 0; x < nScores.Count(); x++)
            {
                oScores[x] = new XNACS1Rectangle();
                oScores[x].Color = new Color(0, 0, 0, 0);
                oScores[x].LabelColor = Color.White;
                oScores[x].LabelFont = "SmallHUD";
            }
            oScoreDisplacments[0] = new Vector2(2.1f, 0f);
            oScoreDisplacments[1] = new Vector2(6.05f, 0f);
            oScoreDisplacments[2] = new Vector2(9.7f, 0f);
            oScoreDisplacments[3] = new Vector2(2.1f, -2.9f);
            oScoreDisplacments[4] = new Vector2(6.05f, -2.9f);

            oName = new XNACS1Rectangle();
            oName.Color = new Color(0, 0, 0, 0);
            oName.LabelColor = Color.White;
            oName.Label = "BillyBobHobo";
            oNameDisplacement = new Vector2(Global.PixelSize(45), Global.PixelSize(27));
            oName.LabelFont = "SmallHUD";
        }

        public void Update(Camera oCamera, Cursor oCursor, Stats oStats, HUD oHUD)
        {
            //this.Label = "" + oStats.oPlayers[nOwner].bCanBeRobbed;

            for (int x = 0; x < nScores.Count(); x++)
            {
                oScores[x].Label = "" + nScores[x];
            }

            if (oStats.oPlayers[nOwner].bCanBeRobbed == true)
            {
                if (oCursor.oHitbox.Collided(this) && oCursor.LeftClick())
                {
                    oStats.oPlayers[0].nCards[oStats.oPlayers[nOwner].GetRobbed()]++;
                    oHUD.ShowCards(oStats.oPlayers[0].nCards, oCamera);

                    for (int x = 0; x < oStats.oPlayers.Count(); x++)
                    {
                        oStats.oPlayers[x].bCanBeRobbed = false;
                    }
                }
            }

            AlignWithCamera(oCamera);
        }

        public void AlignWithCamera(Camera oCamera)
        {
            this.Center = oDisplacement + oCamera.oCenter;

            for (int x = 0; x < oScores.Count(); x++)
            {
                oScores[x].Center = oScoreDisplacments[x] + this.Center;
            }

            oName.Center = oNameDisplacement + this.Center;
        }

        public void SetName(string sNewName)
        {
            oName.Label = sNewName;
        }
    }
}
