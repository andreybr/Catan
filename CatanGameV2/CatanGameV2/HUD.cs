using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2 
{
    public class HUD : XNACS1Rectangle
    {
        KeyboardState oKeyState;

        private float fWidth = Global.PixelSize(Global.nWindowPixelWidth);
        private float fHeight = Global.PixelSize(Global.nWindowPixelHeight);
        private Vector2 oHUDDisplacement = new Vector2(XNACS1Base.World.WorldDimension.X / 2 + 0.01f,
                                                       XNACS1Base.World.WorldDimension.Y / 2 + 0.01f);
        bool bRobberEnabled = false;

        public TradeMenu oTradeMenu;
        public RobberMenu oRobberMenu;
        public YearOfPlentyMenu oYearOfPlentymenu;
        public MonopolyMenu oMonopolyMenu;
        public RoadBuildingMenu oRoadBuildingMenu;
        public KnightMenu oKnightMenu;

        List<XNACS1Rectangle>[] oCards = new List<XNACS1Rectangle>[10];
        List<Vector2>[] oCardDisplacements = new List<Vector2>[10];
        private int nMinCardsPerStack = 4;
        float fCardWidth = Global.PixelSize(57);
        float fCardHeight = Global.PixelSize(83);
        float fCardDistance = Global.PixelSize(6);
        Color oCardLaberColor = Color.Red;
        Vector2 oCardPosition = new Vector2(1f, 5f);

        public XNACS1Rectangle oPickedUpCard;
        public int nPickedUpCard;

        XNACS1Rectangle[] oButtons = new XNACS1Rectangle[7];
        Vector2[] oButtonDisplacement = new Vector2[7];
        Color oOriginalColor = new Color(256, 256, 256);
        Color oUnselectedColor = new Color(100, 100, 100);

        XNACS1Rectangle[] oDice = new XNACS1Rectangle[2];
        Vector2[] oDiceDisplacement = new Vector2[2];
        float fDiceWidth = Global.PixelSize(64);
        float fDiceHeight = Global.PixelSize(64);
        int nCurrentRoll;
        bool bRollMade = false;

        SmallHUD[] oSmallHUDs = new SmallHUD[3];

        // top right menu
        XNACS1Rectangle oTurnNumber;
        Vector2 oTurnNumberDisplacement = new Vector2(82.41f, 68.31f);
        XNACS1Rectangle oCurrentPlayer = new XNACS1Rectangle();
        Vector2 oCurrentPlayerDisplacement = new Vector2(92.51f, 68.31f);

        public enum eMenuState { NONE, NO_CHANGE, TURN_RECORDS, RESOURCE_GRAPHS, TABLE };
        eMenuState MenuState = eMenuState.NONE;
        StatisticsMenu1 oStatisticsMenu1; 
        StatisticsMenu2 oStatisticsMenu2;
        StatisticsMenu3 oStatisticsMenu3;

        public HUD(Stats oStats)
        {
            this.Width = fWidth;
            this.Height = fHeight;
            this.Color = Color.Purple;
            this.Texture = "MainHUD";
            //this.Visible = false;

            oTurnNumber = new XNACS1Rectangle();
            oTurnNumber.Label = oStats.nCurrentTurn + "";
            oTurnNumber.Center = oTurnNumberDisplacement;
            oTurnNumber.Color = new Color(0, 0, 0, 0); // make box invisible
            oTurnNumber.LabelFont = "SmallHUD";
            oTurnNumber.LabelColor = Color.White;

            oCurrentPlayer = new XNACS1Rectangle();
            oCurrentPlayer.Label = oStats.oPlayers[oStats.nCurrentPlayer].sName;
            oCurrentPlayer.Center = oCurrentPlayerDisplacement;
            oCurrentPlayer.LabelFont = "SmallHUD";
            oCurrentPlayer.LabelColor = Color.White;
            oCurrentPlayer.Color = new Color(0, 0, 0, 0); // make box invisible

            oDiceDisplacement[0] = new Vector2(87.31f, 62.91f);
            oDiceDisplacement[1] = new Vector2(94.31f, 62.91f);
            oDice[0] = new XNACS1Rectangle(oDiceDisplacement[0], fDiceWidth, fDiceHeight);
            oDice[0].SetTextureSpriteSheet("dice", 7, 2, 0);
            oDice[0].UseSpriteSheet = true;
            oDice[0].SetTextureSpriteAnimationFrames(0, 0, 0, 0, 1, SpriteSheetAnimationMode.AnimateForward);
            oDice[1] = new XNACS1Rectangle(oDiceDisplacement[1], fDiceWidth, fDiceHeight);
            oDice[1].SetTextureSpriteSheet("dice", 7, 2, 0);
            oDice[1].UseSpriteSheet = true;
            oDice[1].SetTextureSpriteAnimationFrames(0, 1, 0, 1, 1, SpriteSheetAnimationMode.AnimateForward);

            for (int x = 0; x < oSmallHUDs.Count(); x++)
            {
                oSmallHUDs[x] = new SmallHUD(new Vector2(12.51f + 23.01f * x, 64f), x + 1);
                oSmallHUDs[x].SetName(oStats.oPlayers[x].sName);
            }

            for (int x = 0; x < oCards.Count(); x++)
            {
                oCards[x] = new List<XNACS1Rectangle>();
                oCardDisplacements[x] = new List<Vector2>();
            }

            oButtonDisplacement[0] = new Vector2(55.5f, 14.01f);
            oButtonDisplacement[1] = new Vector2(61.5f, 14.01f);
            oButtonDisplacement[2] = new Vector2(67.5f, 14.01f);
            oButtonDisplacement[3] = new Vector2(73.5f, 14.01f);
            oButtonDisplacement[4] = new Vector2(84.01f, 14.01f);
            oButtonDisplacement[5] = new Vector2(90.01f, 14.01f);
            oButtonDisplacement[6] = new Vector2(96.01f, 14.01f);
            oButtons[0] = new XNACS1Rectangle(oButtonDisplacement[2], Global.PixelSize(55), Global.PixelSize(55), "RoadButton");
            oButtons[1] = new XNACS1Rectangle(oButtonDisplacement[0], Global.PixelSize(55), Global.PixelSize(55), "SettlementButton");
            oButtons[2] = new XNACS1Rectangle(oButtonDisplacement[1], Global.PixelSize(55), Global.PixelSize(55), "CityButton");
            oButtons[3] = new XNACS1Rectangle(oButtonDisplacement[3], Global.PixelSize(55), Global.PixelSize(55), "DevelopmentCardButton");
            oButtons[4] = new XNACS1Rectangle(oButtonDisplacement[4], Global.PixelSize(55), Global.PixelSize(55), "DiceButton");
            oButtons[5] = new XNACS1Rectangle(oButtonDisplacement[5], Global.PixelSize(55), Global.PixelSize(55), "TradeButton");
            oButtons[6] = new XNACS1Rectangle(oButtonDisplacement[6], Global.PixelSize(55), Global.PixelSize(55), "EndTurnButton");

            //test 
            //for (int x = 0; x < 4; x++)
            //{
            //    oButtons[x].Visible = false;
            //}
        }

        public void Update(Cursor oCursor, Camera oCamera, Stats oStats, Hex[,] oHexes, Robber oRobber)
        {
            Random oRandom = new Random();
            oKeyState = Keyboard.GetState();

            //XNACS1Base.EchoToTopStatus("HUD:" + oPickedUpCard);

            #region Year of Plenty Menu (test) (F3)
            if (oYearOfPlentymenu == null && oKeyState.IsKeyDown(Keys.F3))
            {
                oYearOfPlentymenu = new YearOfPlentyMenu();
                RedrawCards(oStats, oCamera);
            }
            if (oYearOfPlentymenu != null)
            {
                oYearOfPlentymenu.Update(oCursor, oCamera, this, oStats.oPlayers[0]);

                if(oYearOfPlentymenu.bFinished == true)
                {
                    oYearOfPlentymenu.Clear();
                    oYearOfPlentymenu = null;
                }
            }
            #endregion

            #region Monopoly Menu (test) (F4)
            if (oMonopolyMenu == null && oKeyState.IsKeyDown(Keys.F4))
            {
                oMonopolyMenu = new MonopolyMenu();
                RedrawCards(oStats, oCamera);
            }
            if (oMonopolyMenu != null)
            {
                oMonopolyMenu.Update(oCursor, oCamera, this, oStats.oPlayers);

                if (oMonopolyMenu.bFinished == true)
                {
                    oMonopolyMenu.Clear();
                    oMonopolyMenu = null;
                }
            }
            #endregion

            #region Road Building Menu (test) (F5)
            if (oRoadBuildingMenu == null && oKeyState.IsKeyDown(Keys.F5))
            {
                oRoadBuildingMenu = new RoadBuildingMenu();
                RedrawCards(oStats, oCamera);
            }
            if (oRoadBuildingMenu != null)
            {
                oRoadBuildingMenu.Update(oCursor, oCamera, this, oStats.oPlayers);

                if (oRoadBuildingMenu.bFinished == true)
                {
                    oRoadBuildingMenu.Clear();
                    oRoadBuildingMenu = null;
                }
            }
            #endregion

            #region Knight Menu (test) (F6)
            if (oKnightMenu == null && oKeyState.IsKeyDown(Keys.F6))
            {
                oKnightMenu = new KnightMenu();
                RedrawCards(oStats, oCamera);
            }
            if (oKnightMenu != null)
            {
                oKnightMenu.Update(oCursor, oCamera, this, oStats.oPlayers, oRobber);

                if (oKnightMenu.bFinished == true)
                {
                    oKnightMenu.Clear();
                    oKnightMenu = null;
                }
            }
            #endregion

            #region Statistics Menues (F7-F9)
            // button press
            if (MenuState == eMenuState.NONE)
            {
                if(oKeyState.IsKeyDown(Keys.F7))
                    MenuState = eMenuState.TURN_RECORDS;
                else if(oKeyState.IsKeyDown(Keys.F8))
                    MenuState = eMenuState.RESOURCE_GRAPHS;
                else if (oKeyState.IsKeyDown(Keys.F9))
                    MenuState = eMenuState.TABLE;
            }

            // update
            if (oStatisticsMenu1 != null)
            {
                eMenuState oCurrent = oStatisticsMenu1.Update(oCamera, oCursor);

                if (oCurrent != eMenuState.NO_CHANGE)
                {
                    oStatisticsMenu1.Clear();
                    oStatisticsMenu1 = null;
                    MenuState = oCurrent;
                }
            }
            else if (oStatisticsMenu2 != null)
            {
                eMenuState oCurrent = oStatisticsMenu2.Update(oCamera, oCursor);

                if (oCurrent != eMenuState.NO_CHANGE)
                {
                    oStatisticsMenu2.Clear();
                    oStatisticsMenu2 = null;
                    MenuState = oCurrent;
                }
            }
            else if (oStatisticsMenu3 != null)
            {
                eMenuState oCurrent = oStatisticsMenu3.Update(oCamera, oCursor);

                if (oCurrent != eMenuState.NO_CHANGE)
                {
                    oStatisticsMenu3.Clear();
                    oStatisticsMenu3 = null;
                    MenuState = oCurrent;
                }
            }

            // create
            if (MenuState == eMenuState.TURN_RECORDS && oStatisticsMenu1 == null)
                oStatisticsMenu1 = new StatisticsMenu1(oStats, oCamera);
            else if (MenuState == eMenuState.RESOURCE_GRAPHS && oStatisticsMenu2 == null)
                oStatisticsMenu2 = new StatisticsMenu2(oStats, oCamera);
            else if (MenuState == eMenuState.TABLE && oStatisticsMenu3 == null)
                oStatisticsMenu3 = new StatisticsMenu3(oStats, oCamera);
            #endregion

            #region HUD Follow Camera
            // hud
            this.Center = oCamera.oCenter + oHUDDisplacement;

            // cards
            for (int x = 0; x < oCards.Count(); x++)
            {
                for (int y = 0; y < oCards[x].Count(); y++)
                {
                    oCards[x][y].Center = oCardDisplacements[x][y] + oCamera.oCenter;
                }
            }

            // buttons
            for (int x = 0; x < oButtons.Count(); x++)
            {
                oButtons[x].Center = oButtonDisplacement[x] + oCamera.oCenter;
            }
            #endregion

            #region Update Small HUDs
            for(int x = 0; x < oSmallHUDs.Count(); x++)
            {
                oSmallHUDs[x].Update(oCamera, oCursor, oStats, this);
                oSmallHUDs[x].nScores[0] = oStats.oPlayers[x + 1].nVictoryPoints;
                oSmallHUDs[x].nScores[1] = oStats.oPlayers[x + 1].nArmyCount;
                oSmallHUDs[x].nScores[2] = oStats.oPlayers[x + 1].nLongestRoad;
                oSmallHUDs[x].nScores[3] = oStats.oPlayers[x + 1].nDevelopmentCardCount;
                oSmallHUDs[x].nScores[4] = oStats.oPlayers[x + 1].nResourceCardCount;
            }
            #endregion

            #region Road Button (Left Click), Cancel Building (Right Click)
            if (oCursor.LeftClick() == true && oButtons[0].Collided(oCursor.oHitbox))
            {
                XNACS1Base.PlayACue("ButtonClick");

                if (oStats.oPlayers[oStats.nCurrentPlayer].CanBuildRoad() || oStats.oPlayers[oStats.nCurrentPlayer].nFreeRoads > 0)
                {
                    oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding = Player.CurrentlyBuilding.ROAD;
                    ChangeSelectedButton(oStats);
                }
            }
            else if (oCursor.RightClick())
            {
                oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding = Player.CurrentlyBuilding.NONE;
                ChangeSelectedButton(oStats);
            }
            #endregion

            #region Settlement Button Click
            if (oCursor.LeftClick() == true && oButtons[1].Collided(oCursor.oHitbox))
            {
                XNACS1Base.PlayACue("ButtonClick");

                if (oStats.oPlayers[oStats.nCurrentPlayer].CanBuildSettlement())
                {
                    oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding = Player.CurrentlyBuilding.SETTLEMENT;
                    ChangeSelectedButton(oStats);
                }
            }
            #endregion

            #region City Button Click
            if (oCursor.LeftClick() == true && oButtons[2].Collided(oCursor.oHitbox))
            {
                XNACS1Base.PlayACue("ButtonClick");

                if (oStats.oPlayers[oStats.nCurrentPlayer].CanBuildCity())
                {
                    oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding = Player.CurrentlyBuilding.CITY;
                    ChangeSelectedButton(oStats);
                }
            }
            #endregion

            #region Development Card Button Click
            if (oCursor.LeftClick() == true && oButtons[3].Collided(oCursor.oHitbox))
            {
                XNACS1Base.PlayACue("ButtonClick");

                if (oStats.oPlayers[oStats.nCurrentPlayer].CanGetDevelopment(oStats.nResourceCardsLeft))
                {
                    oStats.oPlayers[oStats.nCurrentPlayer].GetDevelopment(oStats.nResourceCardsLeft, this);
                    oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding = Player.CurrentlyBuilding.NONE;
                    ChangeSelectedButton(oStats);
                    ShowCards(oStats.oPlayers[oStats.nCurrentPlayer].nCards, oCamera);
                }
            }
            #endregion

            #region Dice Button Click
            if (oCursor.LeftClick() == true && oButtons[4].Collided(oCursor.oHitbox) && bRollMade == false)
            {
                XNACS1Base.PlayACue("ButtonClick");
                oButtons[4].TextureTintColor = oUnselectedColor;
                //bRollMade = true;
                int nRoll1 = oRandom.Next() % 6 + 1;
                int nRoll2 = oRandom.Next() % 6 + 1;
                nCurrentRoll = nRoll1 + nRoll2;
                oDice[0].SetTextureSpriteAnimationFrames(nRoll1, 0, nRoll1, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                oDice[1].SetTextureSpriteAnimationFrames(nRoll2, 1, nRoll2, 1, 1, SpriteSheetAnimationMode.AnimateForward);
                oStats.AddRollRecord(nCurrentRoll - 2);

                if (nCurrentRoll == 7 && bRobberEnabled == true)
                {
                    oRobber.ShowMessage();
                    oRobber.bMoveRobber = true;
                    oStats.oPlayers[0].UpdateCardCount();

                    if (oStats.oPlayers[0].nResourceCardCount > 7)
                    {
                        oRobberMenu = new RobberMenu(oStats.oPlayers[0].nResourceCardCount);
                        RedrawCards(oStats, oCamera);
                    }
                }

                int[] nResourceRecord = new int[22];
                nResourceRecord[20] = nCurrentRoll;
                nResourceRecord[21] = oStats.nCurrentTurn;

                for (int x = 0; x < 5; x++) // give resources to players
                {
                    for (int y = 0; y < 5; y++)
                    {
                        if(oHexes[x,y] != null)
                            oHexes[x, y].GiveResources(nCurrentRoll, oStats, nResourceRecord);
                    }
                }

                oStats.AddTurnRecord(nResourceRecord);
                ShowCards(oStats.oPlayers[0].nCards, oCamera);
            }
            #endregion

            #region Trade Button Click
            if (oCursor.LeftClick() == true && oButtons[5].Collided(oCursor.oHitbox))
            {
                XNACS1Base.PlayACue("ButtonClick");
                oTradeMenu = new TradeMenu();
                RedrawCards(oStats, oCamera);
            }
            #endregion

            #region End Turn Button Click
            if (CanEndTurn(oRobber))
            {
                oButtons[6].TextureTintColor = oOriginalColor;
                if (oCursor.LeftClick() == true && oButtons[6].Collided(oCursor.oHitbox) && bRollMade == true)
                {
                    XNACS1Base.PlayACue("ButtonClick");
                    oButtons[4].TextureTintColor = oOriginalColor;
                    bRollMade = false;
                    oStats.PassTurn();
                    oTurnNumber.Label = "" + oStats.nCurrentTurn;
                    oCurrentPlayer.Label = "" + oStats.oPlayers[oStats.nCurrentPlayer].sName;
                }
            }
            else
            {
                oButtons[6].TextureTintColor = oUnselectedColor;
            }
            #endregion

            #region Trade Menu
            if (oTradeMenu != null)
            {
                oTradeMenu.Update(oCamera, oCursor, this, oStats);

                if (oTradeMenu.bFinished == true)
                {
                    oTradeMenu.Clear();
                    oTradeMenu = null;
                }
            }
            #endregion

            #region Robber Menu
            if (oRobberMenu != null)
            {
                oRobberMenu.Update(oCamera, oCursor, this);

                if (oRobberMenu.bFinished == true)
                {
                    oRobberMenu.Clear();
                    oRobberMenu = null;
                }
            }
            #endregion

            #region Pick Up Card/Card Follow Cursor
            // move picked up card
            if (oPickedUpCard != null)
            {
                oPickedUpCard.Center = oCursor.oHitbox.Center;

                if (oCursor.IsLeftMouseDown() == false)
                {
                    if (oTradeMenu != null && oCursor.oHitbox.Collided(oTradeMenu) ||
                        oRobberMenu != null && oCursor.oHitbox.Collided(oRobberMenu))
                    {
                        oStats.oPlayers[0].nCards[nPickedUpCard]--;
                    }

                    oPickedUpCard = null;
                    nPickedUpCard = -1;
                    ShowCards(oStats.oPlayers[0].nCards, oCamera);
                }
            }

            // pick up the card
            if (oCursor.LeftClick() == true && oPickedUpCard == null)
            {
                for (int x = 0; x < oCards.Count(); x++)
                {
                    if (oCards[x].Count > 0 && oCursor.oHitbox.Collided(oCards[x][oCards[x].Count - 1]) == true)
                    {
                        oPickedUpCard = oCards[x][oCards[x].Count - 1];
                        nPickedUpCard = x;

                        if (oPickedUpCard.Visible == false)
                        {
                            oPickedUpCard.Visible = true;
                            oCards[x][0].Label = "" + (oCards[x].Count - 1);
                        }
                    }
                }
            }
            #endregion

            for (int x = 0; x < oDice.Count(); x++)
            {
                oDice[x].Center = oCamera.oCenter + oDiceDisplacement[x];
            }
            oTurnNumber.Center = oCamera.oCenter + oTurnNumberDisplacement;
            oCurrentPlayer.Center = oCamera.oCenter + oCurrentPlayerDisplacement;
        }

        public void ShowCards(int[] nResourceCards, Camera oCamera)
        {
            #region Create Cards
            for (int x = 0; x < nResourceCards.Count(); x++)
            {
                // add cards
                while (nResourceCards[x] > oCards[x].Count())
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
                        case 5:
                            oCards[x].Add(new XNACS1Rectangle(new Vector2(15, 15), fCardWidth, fCardHeight, "KnightCard"));
                            break;
                        case 6:
                            oCards[x].Add(new XNACS1Rectangle(new Vector2(15, 15), fCardWidth, fCardHeight, "VictoryPointCard"));
                            break;
                        case 7:
                            oCards[x].Add(new XNACS1Rectangle(new Vector2(15, 15), fCardWidth, fCardHeight, "RoadBuildingCard"));
                            break;
                        case 8:
                            oCards[x].Add(new XNACS1Rectangle(new Vector2(15, 15), fCardWidth, fCardHeight, "MonopolyCard"));
                            break;
                        case 9:
                            oCards[x].Add(new XNACS1Rectangle(new Vector2(15, 15), fCardWidth, fCardHeight, "YearOfPlentyCard"));
                            break;
                    }
                    oCards[x].Last().LabelColor = oCardLaberColor;
                    oCardDisplacements[x].Add(new Vector2());
                }
                // delete cards
                while (nResourceCards[x] < oCards[x].Count())
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
            float fBetweenTypes = 6.2f;
            float fBetweenCards = 1.05f;
            float fStartingOffset = 27f;

            for (int x = 0; x < oCards.Count(); x++)
            {
                for (int y = 0; y < oCards[x].Count(); y++)
                {
                    float fTotalOffset = fStartingOffset;

                    for(int z = 0; z < x; z++)
                    {
                        if (oCards[z].Count() > 0)
                            fTotalOffset += fBetweenTypes;

                        for (int c = 1; c < oCards[z].Count; c++)
                            if(oCards[z][c].Visible == true)
                                fTotalOffset += fBetweenCards;
                    }

                    if(oCards[x][y].Visible == true)
                        fTotalOffset += y * fBetweenCards;

                    oCards[x][y].Center = new Vector2(fTotalOffset, 5f);
                    oCardDisplacements[x][y] = oCards[x][y].Center;
                    oCards[x][y].Center = new Vector2(fTotalOffset, 5f) + oCamera.oCenter;
                }
            }
            #endregion
        }

        public void RedrawCards(Stats oStats, Camera oCamera)
        {
            for (int x = 0; x < oCards.Count(); x++)
            {
                for (int y = 0; y < oCards[x].Count(); y++)
                {
                    oCards[x][y].RemoveFromAutoDrawSet();
                }

                oCards[x] = new List<XNACS1Rectangle>();
            }

            ShowCards(oStats.oPlayers[0].nCards, oCamera);
        }

        public void ChangeSelectedButton(Stats oStats)
        {
            if(oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding == Player.CurrentlyBuilding.ROAD)
                oButtons[0].TextureTintColor = new Color(100, 100, 100);
            else
                oButtons[0].TextureTintColor = new Color(256, 256, 256);

            if (oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding == Player.CurrentlyBuilding.SETTLEMENT)
                oButtons[1].TextureTintColor = new Color(100, 100, 100);
            else
                oButtons[1].TextureTintColor = new Color(256, 256, 256);

            if (oStats.oPlayers[oStats.nCurrentPlayer].eCurrentlyBuilding == Player.CurrentlyBuilding.CITY)
                oButtons[2].TextureTintColor = new Color(100, 100, 100);
            else
                oButtons[2].TextureTintColor = new Color(256, 256, 256);
        }

        public bool CanEndTurn(Robber oRobber)
        {
            if (oRobberMenu == null &&
                oYearOfPlentymenu == null &&
                oMonopolyMenu == null &&
                oRoadBuildingMenu == null &&
                oKnightMenu == null &&
                bRollMade == true &&
                oRobber.bMoveRobber == false &&
                oRobber.bPickedUp == false)
            {
                return true;
            }

            return false;
        }
    }
}
