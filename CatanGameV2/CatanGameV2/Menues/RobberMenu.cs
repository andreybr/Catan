using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class RobberMenu : XNACS1Rectangle
    {
        private float fWidth = Global.PixelSize(424);
        private float fHeight = Global.PixelSize(192);
        Vector2 oDisplacement = new Vector2(50f, 35f);

        int[] nCards = { 0, 0, 0, 0, 0 }; // cards in hand
        List<XNACS1Rectangle>[] oCards = new List<XNACS1Rectangle>[5];
        List<Vector2>[] oCardDisplacements = new List<Vector2>[5];
        int nMinCardsPerStack = 3;
        float fCardWidth = Global.PixelSize(57);
        float fCardHeight = Global.PixelSize(83);
        float fCardDistance = Global.PixelSize(6);
        Color oCardLaberColor = Color.Red;

        int nDiscardsNeeded = 5;
        int nDiscardsCount = 0;
        XNACS1Rectangle oDiscardCount;
        Vector2 oDiscardDisplacement = new Vector2(-20f, -8.2f);

        XNACS1Rectangle oButton;
        Vector2 oButtonDisplacement = new Vector2(16.5f, -8.2f);
        Color oUnavavailableColor = new Color(125, 125, 125);

        public bool bFinished = false;

        public RobberMenu(int nCards)
        {
            this.Width = fWidth;
            this.Height = fHeight;
            this.Center = new Vector2(50f, 30f);
            this.Texture = "RobberMenu";

            for (int x = 0; x < oCards.Count(); x++)
            {
                oCards[x] = new List<XNACS1Rectangle>();
                oCardDisplacements[x] = new List<Vector2>();
            }

            oButton = new XNACS1Rectangle(new Vector2(), Global.PixelSize(111), Global.PixelSize(34));
            oButton.UseSpriteSheet = true;
            oButton.SetTextureSpriteSheet("TradeMenuButtons1", 3, 1, 0);
            oButton.SetTextureSpriteAnimationFrames(0, 0, 0, 0, 1, SpriteSheetAnimationMode.AnimateForward);
            oButton.LabelColor = Color.White;
            oButton.Label = "Accept";

            oDiscardCount = new XNACS1Rectangle(new Vector2(), 5f, 5f);
            oDiscardCount.Color = new Color(0, 0, 0, 0);
            oDiscardCount.Label = "" + nDiscardsNeeded;
            oDiscardCount.LabelColor = Color.White;

            nDiscardsNeeded = (nCards / 2) + (nCards % 2);
        }

        public void Update(Camera oCamera, Cursor oCursor, HUD oHUD)
        {
            //XNACS1Base.EchoToTopStatus("robbermenu:" + oDiscardCount.Center);

            nDiscardsCount = nCards[0] + nCards[1] + nCards[2] + nCards[3] + nCards[4];

            oDiscardCount.Label = nDiscardsCount + "/" + nDiscardsNeeded;

            #region Pick Up Card
            if (oHUD.oPickedUpCard != null)
            {
                if (Collided(oCursor.oHitbox))
                {
                    switch (oHUD.oPickedUpCard.Texture)
                    {
                        case "WoolCard":
                            nCards[0]++;
                            break;
                        case "LumberCard":
                            nCards[1]++;
                            break;
                        case "OreCard":
                            nCards[2]++;
                            break;
                        case "GrainCard":
                            nCards[3]++;
                            break;
                        case "BrickCard":
                            nCards[4]++;
                            break;
                    }
                    oHUD.oPickedUpCard = null;
                    ShowCards(oCamera);
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
                        nCards[x]--;

                    ShowCards(oCamera);
                }
            }
            #endregion

            #region Accept Button
            if (nDiscardsCount == nDiscardsNeeded)
            {
                oButton.TextureTintColor = new Color(256, 256, 256);

                if (oCursor.LeftClick())
                {
                    if (oCursor.oHitbox.Collided(oButton))
                    {
                        bFinished = true;
                    }
                }
            }
            else
            {
                oButton.TextureTintColor = oUnavavailableColor;
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
                    float fOffsetX = 32f;
                    float fOffsetY = 34.5f;

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

            oButton.Center = this.Center + oButtonDisplacement;

            oDiscardCount.Center = this.Center + oDiscardDisplacement;

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

            oButton.RemoveFromAutoDrawSet();

            oDiscardCount.RemoveFromAutoDrawSet();

            for (int x = 0; x < oCards.Count(); x++)
                for (int y = 0; y < oCards[x].Count(); y++)
                    oCards[x][y].RemoveFromAutoDrawSet();
        }
    }
}
