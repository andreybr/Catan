using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class TradeMenu : XNACS1Rectangle
    {
        KeyboardState oKeyState;
        MouseState oMouseState;

        private float fWidth = Global.PixelSize(424);
        private float fHeight = Global.PixelSize(335);
        Vector2 oDisplacement = new Vector2(50f, 35f);

        int[] nCards1 = { 0, 0, 0, 0, 0} ; // cards in hand
        List<XNACS1Rectangle>[] oCards1 = new List<XNACS1Rectangle>[5];
        List<Vector2>[] oCardDisplacements1 = new List<Vector2>[5];
        int[] nCards2 = { 0, 0, 0, 0, 0 }; // cards in hand
        List<XNACS1Rectangle>[] oCards2 = new List<XNACS1Rectangle>[5];
        List<Vector2>[] oCardDisplacements2 = new List<Vector2>[5];
        int nMinCardsPerStack = 3;
        float fCardWidth = Global.PixelSize(57);
        float fCardHeight = Global.PixelSize(83);
        float fCardDistance = Global.PixelSize(6);
        Color oCardLaberColor = Color.Red;

        XNACS1Rectangle[] oButtons1 = new XNACS1Rectangle[3];
        Vector2[] oButtonDisplacements1 = new Vector2[3];
        XNACS1Rectangle[] oButtons2 = new XNACS1Rectangle[5];
        Vector2[] oButtonDisplacements2 = new Vector2[5];
        Color oUnavailableColor = new Color(125, 125, 125);
        Color oOriginalColor = new Color(256, 256, 256);

        public bool bFinished = false;

        public TradeMenu()
        {
            this.Width = fWidth;
            this.Height = fHeight;
            this.Center = new Vector2(50f, 30f);
            this.Texture = "TradeMenu";

            for (int x = 0; x < oCards1.Count(); x++)
            {
                oCards1[x] = new List<XNACS1Rectangle>();
                oCardDisplacements1[x] = new List<Vector2>();
                oCards2[x] = new List<XNACS1Rectangle>();
                oCardDisplacements2[x] = new List<Vector2>();
            }

            for (int x = 0; x < oButtons1.Count(); x++)
            {
                oButtonDisplacements1[x] = new Vector2(-14f + 14f * x, -4.1f);
                oButtons1[x] = new XNACS1Rectangle(new Vector2(), Global.PixelSize(111), Global.PixelSize(34));
                oButtons1[x].UseSpriteSheet = true;
                oButtons1[x].SetTextureSpriteSheet("TradeMenuButtons1", 3, 1, 0);
                oButtons1[x].SetTextureSpriteAnimationFrames(x, 0, x, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                oButtons1[x].LabelColor = Color.White;
            }
            oButtons1[0].Label = "Trade";
            oButtons1[1].Label = "Cancel";
            oButtons1[2].Label = "Bank";

            for (int x = 0; x < oButtons2.Count(); x++)
            {
                oButtonDisplacements2[x] = new Vector2(-18f + Global.PixelSize(37) * x, 10.3f);
                oButtons2[x] = new XNACS1Rectangle(new Vector2(), Global.PixelSize(33), Global.PixelSize(33));
                oButtons2[x].UseSpriteSheet = true;
                oButtons2[x].SetTextureSpriteSheet("TradeMenuButtons2", 5, 1, 0);
                oButtons2[x].SetTextureSpriteAnimationFrames(x, 0, x, 0, 1, SpriteSheetAnimationMode.AnimateForward);
            }
        }

        public void Update(Camera oCamera, Cursor oCursor,  HUD oHUD, Stats oStats)
        {
            //XNACS1Base.EchoToTopStatus("trade:" + nCards2[0]);

            #region Pick Up Card
            if (oHUD.oPickedUpCard != null)
            {
                if (oCursor.IsLeftMouseDown() == false && Collided(oCursor.oHitbox))
                {
                    nCards1[oHUD.nPickedUpCard]++;
                    ShowCards(nCards1, oCards1, oCardDisplacements1, 1, oCamera);
                }
            }
            #endregion

            #region Remove Cards
            if (oCursor.RightClick() == true)
            {
                for (int x = nCards1.Count() - 1; x > -1; x--)
                {
                    if (nCards1[x] == 0)
                        continue;

                    if (oCursor.oHitbox.Collided(oCards1[x].Last()))
                    {
                        nCards1[x]--;
                        ShowCards(nCards1, oCards1, oCardDisplacements1, 1, oCamera);
                        oStats.oPlayers[0].nCards[x]++;
                        oHUD.ShowCards(oStats.oPlayers[0].nCards, oCamera);
                    }
                }

                for (int x = nCards2.Count() - 1; x > -1; x--)
                {
                    if (nCards2[x] == 0)
                        continue;

                    if (oCursor.oHitbox.Collided(oCards2[x].Last()))
                        nCards2[x]--;

                    ShowCards(nCards2, oCards2, oCardDisplacements2, 2, oCamera);
                }
            }
            #endregion

            #region Resource Buttons
            for (int x = 0; x < oButtons2.Count(); x++)
            {
                if (oCursor.LeftClick() == true)
                {
                    if (oCursor.oHitbox.Collided(oButtons2[x]))
                    {
                        nCards2[x]++;
                        ShowCards(nCards2, oCards2, oCardDisplacements2, 2, oCamera);
                    }
                }
            }
            #endregion

            #region Cancel Button
            if (oCursor.LeftClick())
            {
                if (oCursor.oHitbox.Collided(oButtons1[1]))
                {
                    for (int x = 0; x < nCards1.Count(); x++)
                    {
                        oStats.oPlayers[0].nCards[x] += nCards1[x];
                    }
                    oHUD.ShowCards(oStats.oPlayers[0].nCards, oCamera);

                    bFinished = true;
                }
            }
            #endregion

            #region Top Buttons Hover Effect
            for (int x = 0; x < oButtons2.Count(); x++)
            {
                if (oCursor.oHitbox.Collided(oButtons2[x]))
                {
                    oButtons2[x].TextureTintColor = new Color(256, 256, 256);
                }
                else
                    oButtons2[x].TextureTintColor = new Color(235, 235, 235);
            }
            #endregion

            #region Bottom Buttons Hover Effect
            //for (int x = 0; x < oButtons1.Count(); x++)
            //{
            //    if (oCursor.oHitbox.Collided(oButtons1[x]))
            //    {
            //        oButtons1[x].TextureTintColor = new Color(256, 256, 256);
            //    }
            //    else
            //        oButtons1[x].TextureTintColor = new Color(235, 235, 235);
            //}
            #endregion

            #region Bank Button
            int nMinTrade;
            int nTopCardsCount = nCards2[0] + nCards2[1] + nCards2[2] + nCards2[3] + nCards2[4];
            int nBottomCardsCount = nCards1[0] + nCards1[1] + nCards1[2] + nCards1[3] + nCards1[4];

            if (oStats.oPlayers[0].bPorts[5] == true)
                nMinTrade = 3;
            else
                nMinTrade = 4;

            // 3 for 1, 4 for 1
            if (nBottomCardsCount == nMinTrade && 
                (nCards1[0] == nMinTrade || nCards1[1] == nMinTrade || nCards1[2] == nMinTrade || nCards1[3] == nMinTrade || nCards1[4] == nMinTrade))
            {
                if (nTopCardsCount == 1)
                {
                    oButtons1[2].TextureTintColor = oOriginalColor;
                    oButtons1[2].LabelColor = oOriginalColor;

                    if (oCursor.LeftClick() && oButtons1[2].Collided(oCursor.oHitbox))
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            oStats.oPlayers[0].nCards[x] += nCards2[x];
                        }

                        oHUD.ShowCards(oStats.oPlayers[0].nCards, oCamera);
                        bFinished = true;
                    }
                }
                else
                {
                    oButtons1[2].TextureTintColor = oUnavailableColor;
                    oButtons1[2].LabelColor = oUnavailableColor;
                }
            }
            else
            {
                oButtons1[2].TextureTintColor = oUnavailableColor;
                oButtons1[2].LabelColor = oUnavailableColor;
            }

            // specific resource ports
            if (nBottomCardsCount == 2)
            {
                if (nTopCardsCount == 1)
                {
                    oButtons1[2].TextureTintColor = new Color(256, 256, 256);

                    if (oButtons1[2].Collided(oCursor.oHitbox))
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            if (oStats.oPlayers[0].bPorts[x] == true && nCards1[x] == 2 && oCursor.LeftClick())
                            {
                                oStats.oPlayers[0].nCards[x]++;

                                oHUD.ShowCards(oStats.oPlayers[0].nCards, oCamera);
                                bFinished = true;
                            }
                        }
                    }
                }
            }
            #endregion

            AlignWithCamera(oCamera);
        }

        public void ShowCards(int[] nCards, List<XNACS1Rectangle>[] oCards, List<Vector2>[] oCardDisplacements, int nPosition, Camera oCamera)
        {
            #region Create Cards
            for (int x = 0; x < nCards.Count(); x++)
            {
                // add cards
                while (nCards[x] > oCards[x].Count())
                {
                    switch (x)
                    {
                        case 0:
                            oCards[x].Add(new XNACS1Rectangle(new Vector2(15, 15), fCardWidth, fCardHeight, "WoolCard"));
                            break;
                        case 1:
                            oCards[x].Add(new XNACS1Rectangle(new Vector2(15, 15), fCardWidth, fCardHeight, "LumberCard"));
                            break;
                        case 2:
                            oCards[x].Add(new XNACS1Rectangle(new Vector2(15, 15), fCardWidth, fCardHeight, "OreCard"));
                            break;
                        case 3:
                            oCards[x].Add(new XNACS1Rectangle(new Vector2(15, 15), fCardWidth, fCardHeight, "GrainCard"));
                            break;
                        case 4:
                            oCards[x].Add(new XNACS1Rectangle(new Vector2(15, 15), fCardWidth, fCardHeight, "BrickCard"));
                            break;
                    }
                    oCards[x].Last().LabelColor = oCardLaberColor;
                    oCardDisplacements[x].Add(new Vector2());
                }
                // delete cards
                while (nCards[x] < oCards[x].Count())
                {
                    oCards[x].Last().RemoveFromAutoDrawSet();
                    oCards[x].RemoveAt(oCards[x].Count - 1);
                    oCardDisplacements[x].RemoveAt(oCardDisplacements[x].Count -1);
                }
            }
            #endregion

            #region Stack Cards
            for (int x = 0; x < oCards.Count(); x++)
            {
                // hide all but 1st card when stack limit reached
                if (oCards[x].Count >= nMinCardsPerStack)
                {
                    oCards[x][0].Label = "" + oCards[x].Count;

                    for (int y = 1; y < oCards[x].Count; y++)
                        oCards[x][y].Visible = false;
                }
                // show all cards when below stack limit
                else
                {
                    foreach (XNACS1Rectangle card in oCards[x])
                        card.Visible = true;

                    if (oCards[x].Count > 0)
                        oCards[x][0].Label = "";
                }
            }
            #endregion

            #region Position Cards
            float fBetweenTypes = 7f;
            float fBetweenCards = 1.25f;

            for (int x = 0; x < oCards.Count(); x++)
            {
                for (int y = 0; y < oCards[x].Count(); y++)
                {
                    float fOffsetX = 33f;
                    float fOffsetY;

                    if (nPosition == 1)
                        fOffsetY = 23.45f;
                    else
                        fOffsetY = 38.2f;

                    for(int z = 0; z < x; z++)
                    {
                        if (oCards[z].Count() > 0)
                            fOffsetX += fBetweenTypes;

                        for (int c = 1; c < oCards[z].Count; c++)
                            if(oCards[z][c].Visible == true)
                                fOffsetX += fBetweenCards;
                    }

                    if(oCards[x][y].Visible == true)
                        fOffsetX += y * fBetweenCards;

                    oCards[x][y].Center = new Vector2(fOffsetX, fOffsetY);
                    oCardDisplacements[x][y] = oCards[x][y].Center;
                    oCards[x][y].Center = new Vector2(fOffsetX, fOffsetY) + oCamera.oCenter;
                }
            }
            #endregion
        }

        public void AlignWithCamera(Camera oCamera)
        {
            this.Center = oDisplacement + oCamera.oCenter;

            for (int x = 0; x < oButtons1.Count(); x++)
                oButtons1[x].Center = this.Center + oButtonDisplacements1[x];

            for (int x = 0; x < oButtons2.Count(); x++)
                oButtons2[x].Center = this.Center + oButtonDisplacements2[x];

            for (int x = 0; x < oCards1.Count(); x++)
            {
                for (int y = 0; y < oCards1[x].Count(); y++)
                {
                    oCards1[x][y].Center = oCardDisplacements1[x][y] + oCamera.oCenter;
                }
            }

            for (int x = 0; x < oCards2.Count(); x++)
            {
                for (int y = 0; y < oCards2[x].Count(); y++)
                {
                    oCards2[x][y].Center = oCardDisplacements2[x][y] + oCamera.oCenter;
                }
            }
        }

        public void Clear()
        {
            this.RemoveFromAutoDrawSet();

            for (int x = 0; x < oButtons1.Count(); x++)
                oButtons1[x].RemoveFromAutoDrawSet();

            for (int x = 0; x < oButtons2.Count(); x++)
                oButtons2[x].RemoveFromAutoDrawSet();

            for (int x = 0; x < oCards1.Count(); x++)
                for (int y = 0; y < oCards1[x].Count(); y++)
                    oCards1[x][y].RemoveFromAutoDrawSet();

            for (int x = 0; x < oCards2.Count(); x++)
                for (int y = 0; y < oCards2[x].Count(); y++)
                    oCards2[x][y].RemoveFromAutoDrawSet();
        }
    }
}
