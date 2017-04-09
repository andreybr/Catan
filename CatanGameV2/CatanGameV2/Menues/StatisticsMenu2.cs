using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    class StatisticsMenu2 : XNACS1Rectangle
    {
        Vector2 oDisplacement = new Vector2(50.01f, 35.01f);

        Graph oRollsGraph;
        Graph oResourcesGraph;
        KeyboardState oKeyState = new KeyboardState();

        XNACS1Rectangle oMenuName;
        Vector2 oMenuNameDisplacement = new Vector2(0f, 19f);

        int nButtonCount = 3;
        XNACS1Rectangle[] oButtons;
        Vector2[] oButtonDisplacements;
        float fButtonWidth = Global.PixelSize(24);
        float fButtonHeight = Global.PixelSize(24);

        Color oOriginalColor = new Color(256, 256, 256);
        Color oUnselectedColor = new Color(100, 100, 100);

        public StatisticsMenu2(Stats oStats, Camera oCamera)
        {
            Create(oStats, oCamera);
        }

        public void Create(Stats oStats, Camera oCamera)
        {
            this.Width = Global.PixelSize(824);
            this.Height = Global.PixelSize(472);
            this.Texture = "TurnStatsMenu3";
            this.Center = oDisplacement;

            oMenuName = new XNACS1Rectangle();
            oMenuName.Color = new Color(0, 0, 0, 0);
            oMenuName.LabelFont = "SmallHUD";
            oMenuName.Label = "Resources Graphs";
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
            oButtons[1].TextureTintColor = oOriginalColor;
 
            //oQuitButton = new XNACS1Rectangle(this.Center + oQuitButtonOffest, Global.PixelSize(37), Global.PixelSize(37), "CloseButton");
            //oQuitButton.Color = Color.Red;
            oRollsGraph = new Graph(new Vector2(-16f, -1.6f), new Vector2(Global.PixelSize(428), Global.PixelSize(362)), 
                                    11, oStats.nRollCounts, "Rolls Made", new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" }, "GraphBox1");
            oRollsGraph.Recenter(this);
            oResourcesGraph = new Graph(new Vector2(21.6f, -1.6f), new Vector2(Global.PixelSize(314), Global.PixelSize(362)),
                                        5, oStats.nResourceCounts, "Resources Gained", new string[] { "Wood", "Lumber", "Ore", "Grain", "Brick" }, "GraphBox1");
            oResourcesGraph.Recenter(this);

            Recenter(oCamera);
        }

        public HUD.eMenuState Update(Camera oCamera, Cursor oCursor)
        {
            //XNACS1Base.SetBottomEchoColor(Color.Red);
            //XNACS1Base.EchoToBottomStatus("HUD:" + oKeyState.IsKeyDown(Keys.F2));

            oKeyState = Keyboard.GetState();

            #region Close Menu (Esc)
            if (oKeyState.IsKeyDown(Keys.Escape))
                return HUD.eMenuState.NONE;
            #endregion

            #region Open Different Menu
            if (oCursor.LeftClick())
            {
                if (oButtons[0].Collided(oCursor.oHitbox))
                    return HUD.eMenuState.TURN_RECORDS;
                else if (oButtons[2].Collided(oCursor.oHitbox))
                    return HUD.eMenuState.TABLE;
            }
            #endregion

            //if (oCursor.LeftClick() && this.Collided(oCursor.oHitbox))
            //    oMousePoint = oCursor.oHitbox.Center - this.Center;

            //if (oCursor.IsLeftMouseDown() && this.Collided(oCursor.oHitbox))
            //    bFollowMouse = true;

            //if (bFollowMouse == true && oCursor.IsLeftMouseDown() == false)
            //    bFollowMouse = false;

            //if (bFollowMouse == true)
            //{
            //    this.Center = oCursor.oHitbox.Center - oMousePoint;
            //    oQuitButton.Center = this.Center + oQuitButtonOffest;
            //}

            //if (oCursor.LeftClick() && oQuitButton.Collided(oCursor.oHitbox))
            //    return false;
            //else
            //    return true;

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

            oRollsGraph.ResizeBars();
            oRollsGraph.Recenter(this);
            oResourcesGraph.ResizeBars();
            oResourcesGraph.Recenter(this);
        }

        public void Clear()
        {
            this.RemoveFromAutoDrawSet();
            oMenuName.RemoveFromAutoDrawSet();

            oRollsGraph.Clear();
            oResourcesGraph.Clear();

            for (int x = 0; x < oButtons.Count(); x++)
                oButtons[x].RemoveFromAutoDrawSet();
        }
    }
}
