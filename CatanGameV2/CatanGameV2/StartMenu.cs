using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    class StartMenu
    {
        List<XNACS1Rectangle> oButtons = new List<XNACS1Rectangle>();
        float fButtonWidth = 30f;
        float fButtonHeight = 5f;
        Vector2 oButtonStartingLoc = new Vector2(50f, 25f);
        Vector2 oButtonDisplacement = new Vector2(0f, -7f);

        XNACS1Rectangle oSelectButton = new XNACS1Rectangle();
        int nCurrentSelectedButton = 0;
        int nCursorMoveTimer = 2;
        int nCurrentCursorMoveTimer = 0;

        public StartMenu()
        {
            XNACS1Base.World.SetBackgroundTexture("TitleScreen");

            for (int x = 0; x < 3; x++)
            {
                Vector2 oDisp;

                if (oButtons.Count > 0)
                    oDisp = oButtons.Last().Center + oButtonDisplacement;
                else
                    oDisp = oButtonStartingLoc;

                oButtons.Add(new XNACS1Rectangle(oDisp, fButtonWidth, fButtonHeight));
                oButtons.Last().Color = new Color(0, 0, 0, 0);
                oButtons.Last().LabelColor = Color.White;
            }
            oButtons[0].Label = "Start Game";
            oButtons[1].Label = "Settings";
            oButtons[2].Label = "Exit Game";

            oSelectButton = new XNACS1Rectangle(oButtons[0].Center, fButtonWidth, fButtonHeight);
            oSelectButton.Color = new Color(100, 100, 100, 100);
        }

        public Game1.MENU_STATE Update(Cursor oCursor, Camera oCamera)
        {
            if (nCurrentCursorMoveTimer > 1)
                nCurrentCursorMoveTimer--;
            else
                oSelectButton.ShouldTravel = false;

            for (int x = 0; x < oButtons.Count(); x++)
                if (oButtons[x].Collided(oCursor.oHitbox))
                {
                    MoveSelectButton(x);

                    if (oCursor.LeftClick())
                    {
                        if (x == 0)
                        {
                            Clear();
                            return Game1.MENU_STATE.BASIC_GAME;
                        }
                    }
                }

            return Game1.MENU_STATE.START;
        }

        public void MoveSelectButton(int nIndex)
        {
            if (nIndex != nCurrentSelectedButton)
            {
                nCurrentSelectedButton = nIndex;
                oSelectButton.ShouldTravel = true;
                oSelectButton.VelocityX = 0f;
                oSelectButton.VelocityY = (oButtons[nIndex].CenterY - oSelectButton.CenterY) / (float)nCursorMoveTimer;
                nCurrentCursorMoveTimer = nCursorMoveTimer;
                //XNACS1Base.EchoToTopStatus("" + oSelectButton.VelocityY);
            }
        }

        public void Clear()
        {
            foreach (XNACS1Rectangle button in oButtons)
                button.RemoveFromAutoDrawSet();

            oSelectButton.RemoveFromAutoDrawSet();
        }
    }
}
