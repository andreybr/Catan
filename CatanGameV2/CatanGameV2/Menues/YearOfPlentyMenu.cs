using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class YearOfPlentyMenu : XNACS1Rectangle
    {
        private float fWidth = Global.PixelSize(424);
        private float fHeight = Global.PixelSize(228);
        Vector2 oDisplacement = new Vector2(50.01f, 35.01f);

        int[] nCards = { 0, 0, 0, 0, 0 }; // cards in hand
        List<XNACS1Rectangle>[] oCards = new List<XNACS1Rectangle>[5];
        List<Vector2>[] oCardDisplacements = new List<Vector2>[5];
        const int nMinCardsPerStack = 3;
        float fCardWidth = Global.PixelSize(57);
        float fCardHeight = Global.PixelSize(83);
        float fCardDistance = Global.PixelSize(6);
        Color oCardLaberColor = Color.Red;

        XNACS1Rectangle[] oButtons1 = new XNACS1Rectangle[2];
        Vector2[] oButtonDisplacements1 = new Vector2[2];
        XNACS1Rectangle[] oButtons2 = new XNACS1Rectangle[5];
        Vector2[] oButtonDisplacements2 = new Vector2[5];
        Color oClickColor = new Color(150, 150, 150);
        Color oUnavailableColor = new Color(125, 125, 125);
        Color oOriginalColor = new Color(256, 256, 256);

        int nCardsPicked = 0;
        const int nCARDS_GAINED = 2;
        public bool bFinished = false;

        public YearOfPlentyMenu()
        {
            this.Width = fWidth;
            this.Height = fHeight;
            this.Texture = "YearOfPlentyMenu";

            for (int x = 0; x < oCards.Count(); x++)
            {
                oCards[x] = new List<XNACS1Rectangle>();
                oCardDisplacements[x] = new List<Vector2>();
            }

            for (int x = 0; x < oButtons1.Count(); x++)
            {
                oButtonDisplacements1[x] = new Vector2(-7.4f + 15.01f * x, -9.011f);
                oButtons1[x] = new XNACS1Rectangle(new Vector2(), Global.PixelSize(111), Global.PixelSize(34));
                oButtons1[x].UseSpriteSheet = true;
                oButtons1[x].SetTextureSpriteSheet("TradeMenuButtons1", 3, 1, 0);
                oButtons1[x].SetTextureSpriteAnimationFrames(x, 0, x, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                oButtons1[x].LabelColor = Color.White;
            }
            oButtons1[0].Label = "Accept";
            oButtons1[1].Label = "Cancel";
            oButtons1[0].TextureTintColor = oUnavailableColor;

            for (int x = 0; x < oButtons2.Count(); x++)
            {
                oButtonDisplacements2[x] = new Vector2(-18f + Global.PixelSize(37) * x, 4.91f);
                oButtons2[x] = new XNACS1Rectangle(new Vector2(), Global.PixelSize(33), Global.PixelSize(33));
                oButtons2[x].UseSpriteSheet = true;
                oButtons2[x].SetTextureSpriteSheet("TradeMenuButtons2", 5, 1, 0);
                oButtons2[x].SetTextureSpriteAnimationFrames(x, 0, x, 0, 1, SpriteSheetAnimationMode.AnimateForward);
            }
        }

        public void Update(Cursor oCursor, Camera oCamera, HUD oHUD, Player oPlayer)
        {
            #region Resource Buttons
            for (int x = 0; x < oButtons2.Count(); x++)
            {
                if (oCursor.LeftClick() == true)
                {
                    if (nCardsPicked < nCARDS_GAINED)
                    {
                        if (oCursor.oHitbox.Collided(oButtons2[x]))
                        {
                            nCards[x]++;
                            ShowCards(oCamera);
                            nCardsPicked++;
                        }
                    }
                }
            }
            #endregion

            #region Remove Cards
            if (oCursor.RightClick() == true)
            {
                for (int x = nCards.Count() - 1; x > -1; x--)
                {
                    if (nCards[x] == 0)
                        continue;

                    if (oCursor.oHitbox.Collided(oCards[x].Last()))
                    {
                        nCards[x]--;
                        nCardsPicked--;
                        ShowCards(oCamera);
                    }
                }
            }
            #endregion

            #region Accept Button
            if (nCardsPicked == nCARDS_GAINED)
            {
                oButtons1[0].TextureTintColor = oOriginalColor;
                oButtons1[0].LabelColor = oOriginalColor;

                if (oCursor.oHitbox.Collided(oButtons1[0]))
                {
                    if (oCursor.LeftClick())
                    {
                        for (int x = 0; x < nCards.Count(); x++)
                        {
                            oPlayer.nCards[x] += nCards[x];
                        }

                        oHUD.ShowCards(oPlayer.nCards, oCamera);
                        bFinished = true;
                    }
                }
            }
            else
            {
                oButtons1[0].TextureTintColor = oUnavailableColor;
                oButtons1[0].LabelColor = oUnavailableColor;
            }
            #endregion

            #region Cancel Button
            if (oCursor.LeftClick())
            {
                if (oCursor.oHitbox.Collided(oButtons1[1]))
                {
                    bFinished = true;
                }
            }
            #endregion

            AlignWithCamera(oCamera);
        }

        public void ShowCards(Camera oCamera)
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
                    oCardDisplacements[x].RemoveAt(oCardDisplacements[x].Count - 1);
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
                    float fOffsetY = 33.0f;

                    for (int z = 0; z < x; z++)
                    {
                        if (oCards[z].Count() > 0)
                            fOffsetX += fBetweenTypes;

                        for (int c = 1; c < oCards[z].Count; c++)
                            if (oCards[z][c].Visible == true)
                                fOffsetX += fBetweenCards;
                    }

                    if (oCards[x][y].Visible == true)
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

            for (int x = 0; x < oCards.Count(); x++)
            {
                for (int y = 0; y < oCards[x].Count(); y++)
                {
                    oCards[x][y].Center = oCardDisplacements[x][y] + oCamera.oCenter;
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

            for (int x = 0; x < oCards.Count(); x++)
                for (int y = 0; y < oCards[x].Count(); y++)
                    oCards[x][y].RemoveFromAutoDrawSet();
        }
    }
}
