using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class StatisticsMenu1 : XNACS1Rectangle
    {
        KeyboardState oKeyState; 
        Vector2 oDisplacement = new Vector2(50.01f, 35.01f);
        const int nMaxRecords = 10;

        XNACS1Rectangle[] oPlayerNames = new XNACS1Rectangle[4];
        Vector2[] oPlayerNamesDisplacements = new Vector2[4];

        XNACS1Rectangle oColumnNames;
        Vector2 oColumnNamesDisplacement = new Vector2(-32.5f, 16f);

        XNACS1Rectangle oMenuName;
        Vector2 oMenuNameDisplacement = new Vector2(0f, 19f);

        int nButtonCount = 3;
        XNACS1Rectangle[] oButtons;
        Vector2[] oButtonDisplacements;
        float fButtonWidth = Global.PixelSize(24);
        float fButtonHeight = Global.PixelSize(24);

        Color oOriginalColor = new Color(256, 256, 256);
        Color oUnselectedColor = new Color(100, 100, 100);

        TurnRecordBar[] oTurnRecords = new TurnRecordBar[10];

        public StatisticsMenu1(Stats oStats, Camera oCamera)
        {
            this.Width = Global.PixelSize(824);
            this.Height = Global.PixelSize(472);
            this.Texture = "TurnStatsMenu2";
            this.Center = oDisplacement;

            oColumnNames = new XNACS1Rectangle();
            oColumnNames.Color = new Color(0, 0, 0, 0);
            oColumnNames.LabelFont = "SmallHUD";
            oColumnNames.Label = "  Turn    Roll";
            oColumnNames.LabelColor = Color.Brown;

            oMenuName = new XNACS1Rectangle();
            oMenuName.Color = new Color(0, 0, 0, 0);
            oMenuName.LabelFont = "SmallHUD";
            oMenuName.Label = "Resources Gained Per Turn";
            oMenuName.LabelColor = Color.White;

            oButtons = new XNACS1Rectangle[nButtonCount];
            oButtonDisplacements = new Vector2[nButtonCount];
            for (int x = 0; x < oButtons.Count(); x++)
            { 
                oButtons[x] = new XNACS1Rectangle(new Vector2(), fButtonWidth, fButtonHeight, "StatsMenuButton");
                oButtons[x].Label = "" + (x + 1);
                oButtons[x].LabelFont = "SmallHUD";
                oButtons[x].LabelColor = Color.White;
                oButtons[x].TextureTintColor = oUnselectedColor;
                oButtonDisplacements[x] = new Vector2(-36.35f + 3f * x, 19f); 
            }
            oButtons[0].TextureTintColor = oOriginalColor;

            for (int x = 0; x < oStats.oPlayers.Count(); x++)
            {
                oPlayerNames[x] = new XNACS1Rectangle();
                oPlayerNames[x].LabelFont = "SmallHUD";
                oPlayerNames[x].Color = new Color(0, 0, 0, 0);
                oPlayerNames[x].Label = oStats.oPlayers[x].sName;
                oPlayerNames[x].LabelColor = Color.Brown;
                oPlayerNamesDisplacements[x] = new Vector2(-19.5f + 15.9f * x, 16f);
            }

            #region Test Data
            //Random oRandom = new Random();
            //int[] oNewRecord;

            //for (int x = 0; x < nMaxRecords - 1; x++)
            //{
            //    oNewRecord = new int[22];
            //    for (int y = 0; y < oNewRecord.Count(); y++)
            //    {
            //        oNewRecord[y] = oRandom.Next(20);
            //    }
            //    AddRecord(oNewRecord);
            //}
            #endregion

            CreateRecords(oStats.nTurnResourceRecords);

            Recenter(oCamera);
        }

        public HUD.eMenuState Update(Camera oCamera, Cursor oCursor)
        {
            oKeyState = Keyboard.GetState();

            #region Close Menu (Esc)
            if (oKeyState.IsKeyDown(Keys.Escape))
                return HUD.eMenuState.NONE;
            #endregion

            #region Open Different Menu
            if (oCursor.LeftClick())
            {
                if (oButtons[1].Collided(oCursor.oHitbox))
                {
                    return HUD.eMenuState.RESOURCE_GRAPHS;
                }
                else if (oButtons[2].Collided(oCursor.oHitbox))
                {
                    return HUD.eMenuState.TABLE;
                }
            }
            #endregion

            Recenter(oCamera);

            return HUD.eMenuState.NO_CHANGE;
        }

        public void CreateRecords(List<int[]> nTurnResourceRecords)
        {
            // actual records
            for (int x = 0; x < nTurnResourceRecords.Count(); x++)
            {
                oTurnRecords[x] = new TurnRecordBar(nTurnResourceRecords[x], new Vector2(0f, 12.2f - 3.35f * x));
            }

            // fill table with empty records
            for (int x = nTurnResourceRecords.Count(); x < nMaxRecords; x++)
            {
                oTurnRecords[x] = new TurnRecordBar(null, new Vector2(0f, 12.2f - 3.35f * x));
            }
        }

        public void Recenter(Camera oCamera)
        {
            this.Center = oDisplacement + oCamera.oCenter;

            for (int x = 0; x < oTurnRecords.Count(); x++)
                oTurnRecords[x].Recenter(this.Center);

            oColumnNames.Center = this.Center + oColumnNamesDisplacement;
            oMenuName.Center = this.Center + oMenuNameDisplacement;

            for (int x = 0; x < oButtons.Count(); x++)
            {
                oButtons[x].Center = this.Center + oButtonDisplacements[x];
            }

            for (int x = 0; x < oPlayerNames.Count(); x++)
            {
                oPlayerNames[x].Center = this.Center + oPlayerNamesDisplacements[x];
            }
        }

        public void Clear()
        {
            foreach (TurnRecordBar record in oTurnRecords)
            {
                record.Clear();
                record.RemoveFromAutoDrawSet();
            }

            foreach(XNACS1Rectangle name in oPlayerNames)
                name.RemoveFromAutoDrawSet();

            foreach (XNACS1Rectangle button in oButtons)
                button.RemoveFromAutoDrawSet();

            this.RemoveFromAutoDrawSet();
            oMenuName.RemoveFromAutoDrawSet();
            oColumnNames.RemoveFromAutoDrawSet();
        }
    }
}
