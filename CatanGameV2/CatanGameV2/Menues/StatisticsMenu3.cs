using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    class StatisticsMenu3 : XNACS1Rectangle
    {
        KeyboardState oKeyState; 
        Vector2 oDisplacement = new Vector2(50.01f, 35.01f);

        XNACS1Rectangle[] oPlayerNames = new XNACS1Rectangle[4];
        Vector2[] oPlayerNamesDisplacements = new Vector2[4];

        XNACS1Rectangle oMenuName;
        Vector2 oMenuNameDisplacement = new Vector2(0f, 19f);

        int nButtonCount = 3;
        XNACS1Rectangle[] oButtons;
        Vector2[] oButtonDisplacements;
        float fButtonWidth = Global.PixelSize(24);
        float fButtonHeight = Global.PixelSize(24);

        Color oOriginalColor = new Color(256, 256, 256);
        Color oUnselectedColor = new Color(100, 100, 100);

        public StatisticsMenu3(Stats oStats, Camera oCamera)
        {
            this.Width = Global.PixelSize(824);
            this.Height = Global.PixelSize(472);
            this.Texture = "TurnStatsMenu2";
            this.Center = oDisplacement;
            
            //this.LabelColor = Color.Red;
            //this.Label += "Total Gained:";
            //for (int x = 0; x < 4; x++)
            //    this.Label += " " + oStats.nTotalResourcesGained[x];

            //this.Label += "\nTotal Lost:";
            //for (int x = 0; x < 4; x++)
            //    this.Label += " " + oStats.nTotalResourcesLost[x];

            //this.Label += "\n Gained Through Trading:";
            //for (int x = 0; x < 4; x++)
            //    this.Label += " " + oStats.nTotalResourcesLost[x];

            //this.Label += "\n Lost Through Trading:";
            //for (int x = 0; x < 4; x++)
            //    this.Label += " " + oStats.nTotalResourcesLost[x];

            //this.Label += "\n Robbers Used:";
            //for (int x = 0; x < 4; x++)
            //    this.Label += " " + oStats.nTotalResourcesLost[x];

            //this.Label += "\n Times Robbed:";
            //for (int x = 0; x < 4; x++)
            //    this.Label += " " + oStats.nTotalResourcesLost[x];

            //this.Label += "\n Gained From Advancement Cards:";
            //for (int x = 0; x < 4; x++)
            //    this.Label += " " + oStats.nTotalResourcesLost[x];

            //this.Label += "\n Lost From Advancement Cards:";
            //for (int x = 0; x < 4; x++)
            //    this.Label += " " + oStats.nTotalResourcesLost[x];

            //this.Label += "\n Lost From Blocking";
            //for (int x = 0; x < 4; x++)
            //    this.Label += " " + oStats.nTotalResourcesLost[x];


            oMenuName = new XNACS1Rectangle();
            oMenuName.Color = new Color(0, 0, 0, 0);
            oMenuName.LabelFont = "SmallHUD";
            oMenuName.Label = "Player Statistics";
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
            oButtons[2].TextureTintColor = oOriginalColor;

            for (int x = 0; x < oStats.oPlayers.Count(); x++)
            {
                oPlayerNames[x] = new XNACS1Rectangle();
                oPlayerNames[x].LabelFont = "SmallHUD";
                oPlayerNames[x].Color = new Color(0, 0, 0, 0);
                oPlayerNames[x].Label = oStats.oPlayers[x].sName;
                oPlayerNames[x].LabelColor = Color.White;
                oPlayerNamesDisplacements[x] = new Vector2(-19.5f + 15.9f * x, 16f);
            }

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
                if(oButtons[0].Collided(oCursor.oHitbox))
                    return HUD.eMenuState.TURN_RECORDS;
                else if(oButtons[1].Collided(oCursor.oHitbox))
                    return HUD.eMenuState.RESOURCE_GRAPHS;
            }
            #endregion

            Recenter(oCamera);

            return HUD.eMenuState.NO_CHANGE;
        }

        public void Recenter(Camera oCamera)
        {
            this.Center = oDisplacement + oCamera.oCenter;

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
            foreach (XNACS1Rectangle name in oPlayerNames)
                name.RemoveFromAutoDrawSet();

            foreach (XNACS1Rectangle button in oButtons)
                button.RemoveFromAutoDrawSet();

            this.RemoveFromAutoDrawSet();
            oMenuName.RemoveFromAutoDrawSet();

        }
    }
}
