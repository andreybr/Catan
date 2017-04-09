using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    class BasicGame
    {
        Hex[,] oHexes;
        private int[] nResourceCounts = new int[6] { 4, 4, 3, 4, 3, 1 };
        private int[] nNumberCounts = new int[13] { 0, 0, 1, 2, 2, 2, 2, 0, 2, 2, 2, 2, 1 };
        Intersection[] oTempIntersections = new Intersection[54];

        XNACS1Rectangle oWater = new XNACS1Rectangle(new Vector2(51f, 35f), Global.PixelSize(730), Global.PixelSize(609), "WaterBG");

        Robber oRobber;
        //XNACS1Base.World.SetBackgroundColor(Color.Black);

        KeyboardState oKeyState;

        int nButtonDelay = 5;
        int nCurrentButtonDelay = 0;
        bool bReset = true;

        Stats oStats;
        HUD oHUD;
        ChatMenu oChatMenu;

        public BasicGame(Camera oCamera)
        {
            //XNACS1Base.World.SetBackgroundTexture(null);
            //XNACS1Base.World.SetBackgroundColor(new Color(88, 177, 223));
            XNACS1Base.World.SetBackgroundTexture("Background");
            oCamera.bAllowMovement = true;
            oStats = new Stats();

            Random rand = new Random();
            oHexes = new Hex[5, 5];
            int nCount = 0;

            Road[] oTempRoads = new Road[75];

            Vector2 oRobberStartingLocation = new Vector2();
            Vector2 oRobberStartingHex = new Vector2();

            #region Create and Place Hexes
            for (int y = 0; y < 5; y++)
            {
                for (int x = 4; x > -1; x--)
                {
                    //Vector2 oVect;
                    
                    //version 1
                    //float fYPos = Global.PixelSize(80);
                    //float fXPos = Global.PixelSize(88);

                    //version 2
                    float fYPos = 9.01f;
                    float fXPos = 9.72f;

                    if ((y == 0 || y == 4) && x >= 3)
                        continue;
                    else if ((y == 1 || y == 3) && x >= 4)
                        continue;

                    Vector2 oVect = new Vector2(32f + x * fXPos, 35f - (y * fYPos) + x * (fYPos / 2));

                    if (y > 2)
                        oVect += new Vector2(fXPos, (fYPos / 2));
                    if (y > 3)
                        oVect += new Vector2(fXPos, (fYPos / 2));

                    #region Resource Type
                    int nRand;

                    do
                    {
                        nRand = rand.Next() % 6;
                    }
                    while (nResourceCounts[nRand] == 0);
                    nResourceCounts[nRand]--;

                    string sType;

                    if (nRand == 0)
                        sType = "wool";
                    else if (nRand == 1)
                        sType = "lumber";
                    else if (nRand == 2)
                        sType = "ore";
                    else if (nRand == 3)
                        sType = "grain";
                    else if (nRand == 4)
                        sType = "brick";
                    else
                        sType = "empty";
                    #endregion

                    #region Roll Number
                    int nNumber = 0;
                    if (sType != "empty")
                    {
                        do
                        {
                            nNumber = rand.Next() % 11 + 2;
                        }
                        while (nNumber == 7 || nNumber == 0 || nNumberCounts[nNumber] == 0);

                        nNumberCounts[nNumber]--;
                    }
                    #endregion

                    oHexes[x, y] = new Hex(oVect, nNumber, sType, x + "," + y);

                    if (sType == "empty")
                    {
                        oHexes[x, y].bRobber = true;
                        oRobberStartingLocation = oHexes[x, y].Center;
                        oRobberStartingHex = new Vector2(x, y);
                    }
                }
            }
            #endregion

            for (int x = 0; x < oTempRoads.Count(); x++)
                oTempRoads[x] = new Road(x, new Vector2(x * 1.5f, 0));

            for (int x = 0; x < oTempIntersections.Count(); x++)
                oTempIntersections[x] = new Intersection(x);

            #region Connect Buildings 
            oHexes[0, 0].oIntersections[0] = oTempIntersections[2];
            oHexes[0, 0].oIntersections[1] = oTempIntersections[1];
            oHexes[0, 0].oIntersections[2] = oTempIntersections[0];
            oHexes[0, 0].oIntersections[3] = oTempIntersections[8];
            oHexes[0, 0].oIntersections[4] = oTempIntersections[9];
            oHexes[0, 0].oIntersections[5] = oTempIntersections[10];
            oHexes[1, 0].oIntersections[0] = oTempIntersections[4];
            oHexes[1, 0].oIntersections[1] = oTempIntersections[3];
            oHexes[1, 0].oIntersections[2] = oTempIntersections[2];
            oHexes[1, 0].oIntersections[3] = oTempIntersections[10];
            oHexes[1, 0].oIntersections[4] = oTempIntersections[11];
            oHexes[1, 0].oIntersections[5] = oTempIntersections[12];
            oHexes[2, 0].oIntersections[0] = oTempIntersections[6];
            oHexes[2, 0].oIntersections[1] = oTempIntersections[5];
            oHexes[2, 0].oIntersections[2] = oTempIntersections[4];
            oHexes[2, 0].oIntersections[3] = oTempIntersections[12];
            oHexes[2, 0].oIntersections[4] = oTempIntersections[13];
            oHexes[2, 0].oIntersections[5] = oTempIntersections[14];

            oHexes[0, 1].oIntersections[0] = oTempIntersections[9];
            oHexes[0, 1].oIntersections[1] = oTempIntersections[8];
            oHexes[0, 1].oIntersections[2] = oTempIntersections[7];
            oHexes[0, 1].oIntersections[3] = oTempIntersections[17];
            oHexes[0, 1].oIntersections[4] = oTempIntersections[18];
            oHexes[0, 1].oIntersections[5] = oTempIntersections[19];
            oHexes[1, 1].oIntersections[0] = oTempIntersections[11];
            oHexes[1, 1].oIntersections[1] = oTempIntersections[10];
            oHexes[1, 1].oIntersections[2] = oTempIntersections[9];
            oHexes[1, 1].oIntersections[3] = oTempIntersections[19];
            oHexes[1, 1].oIntersections[4] = oTempIntersections[20];
            oHexes[1, 1].oIntersections[5] = oTempIntersections[21];
            oHexes[2, 1].oIntersections[0] = oTempIntersections[13];
            oHexes[2, 1].oIntersections[1] = oTempIntersections[12];
            oHexes[2, 1].oIntersections[2] = oTempIntersections[11];
            oHexes[2, 1].oIntersections[3] = oTempIntersections[21];
            oHexes[2, 1].oIntersections[4] = oTempIntersections[22];
            oHexes[2, 1].oIntersections[5] = oTempIntersections[23];
            oHexes[3, 1].oIntersections[0] = oTempIntersections[15];
            oHexes[3, 1].oIntersections[1] = oTempIntersections[14];
            oHexes[3, 1].oIntersections[2] = oTempIntersections[13];
            oHexes[3, 1].oIntersections[3] = oTempIntersections[23];
            oHexes[3, 1].oIntersections[4] = oTempIntersections[24];
            oHexes[3, 1].oIntersections[5] = oTempIntersections[25];

            oHexes[0, 2].oIntersections[0] = oTempIntersections[18];
            oHexes[0, 2].oIntersections[1] = oTempIntersections[17];
            oHexes[0, 2].oIntersections[2] = oTempIntersections[16];
            oHexes[0, 2].oIntersections[3] = oTempIntersections[27];
            oHexes[0, 2].oIntersections[4] = oTempIntersections[28];
            oHexes[0, 2].oIntersections[5] = oTempIntersections[29];
            oHexes[1, 2].oIntersections[0] = oTempIntersections[20];
            oHexes[1, 2].oIntersections[1] = oTempIntersections[19];
            oHexes[1, 2].oIntersections[2] = oTempIntersections[18];
            oHexes[1, 2].oIntersections[3] = oTempIntersections[29];
            oHexes[1, 2].oIntersections[4] = oTempIntersections[30];
            oHexes[1, 2].oIntersections[5] = oTempIntersections[31];
            oHexes[2, 2].oIntersections[0] = oTempIntersections[22];
            oHexes[2, 2].oIntersections[1] = oTempIntersections[21];
            oHexes[2, 2].oIntersections[2] = oTempIntersections[20];
            oHexes[2, 2].oIntersections[3] = oTempIntersections[31];
            oHexes[2, 2].oIntersections[4] = oTempIntersections[32];
            oHexes[2, 2].oIntersections[5] = oTempIntersections[33];
            oHexes[3, 2].oIntersections[0] = oTempIntersections[24];
            oHexes[3, 2].oIntersections[1] = oTempIntersections[23];
            oHexes[3, 2].oIntersections[2] = oTempIntersections[22];
            oHexes[3, 2].oIntersections[3] = oTempIntersections[33];
            oHexes[3, 2].oIntersections[4] = oTempIntersections[34];
            oHexes[3, 2].oIntersections[5] = oTempIntersections[35];
            oHexes[4, 2].oIntersections[0] = oTempIntersections[26];
            oHexes[4, 2].oIntersections[1] = oTempIntersections[25];
            oHexes[4, 2].oIntersections[2] = oTempIntersections[24];
            oHexes[4, 2].oIntersections[3] = oTempIntersections[35];
            oHexes[4, 2].oIntersections[4] = oTempIntersections[36];
            oHexes[4, 2].oIntersections[5] = oTempIntersections[37];

            oHexes[0, 3].oIntersections[0] = oTempIntersections[30];
            oHexes[0, 3].oIntersections[1] = oTempIntersections[29];
            oHexes[0, 3].oIntersections[2] = oTempIntersections[28];
            oHexes[0, 3].oIntersections[3] = oTempIntersections[38];
            oHexes[0, 3].oIntersections[4] = oTempIntersections[39];
            oHexes[0, 3].oIntersections[5] = oTempIntersections[40];
            oHexes[1, 3].oIntersections[0] = oTempIntersections[32];
            oHexes[1, 3].oIntersections[1] = oTempIntersections[31];
            oHexes[1, 3].oIntersections[2] = oTempIntersections[30];
            oHexes[1, 3].oIntersections[3] = oTempIntersections[40];
            oHexes[1, 3].oIntersections[4] = oTempIntersections[41];
            oHexes[1, 3].oIntersections[5] = oTempIntersections[42];
            oHexes[2, 3].oIntersections[0] = oTempIntersections[34];
            oHexes[2, 3].oIntersections[1] = oTempIntersections[33];
            oHexes[2, 3].oIntersections[2] = oTempIntersections[32];
            oHexes[2, 3].oIntersections[3] = oTempIntersections[42];
            oHexes[2, 3].oIntersections[4] = oTempIntersections[43];
            oHexes[2, 3].oIntersections[5] = oTempIntersections[44];
            oHexes[3, 3].oIntersections[0] = oTempIntersections[36];
            oHexes[3, 3].oIntersections[1] = oTempIntersections[35];
            oHexes[3, 3].oIntersections[2] = oTempIntersections[34];
            oHexes[3, 3].oIntersections[3] = oTempIntersections[44];
            oHexes[3, 3].oIntersections[4] = oTempIntersections[45];
            oHexes[3, 3].oIntersections[5] = oTempIntersections[46];

            oHexes[0, 4].oIntersections[0] = oTempIntersections[41];
            oHexes[0, 4].oIntersections[1] = oTempIntersections[40];
            oHexes[0, 4].oIntersections[2] = oTempIntersections[39];
            oHexes[0, 4].oIntersections[3] = oTempIntersections[47];
            oHexes[0, 4].oIntersections[4] = oTempIntersections[48];
            oHexes[0, 4].oIntersections[5] = oTempIntersections[49];
            oHexes[1, 4].oIntersections[0] = oTempIntersections[43];
            oHexes[1, 4].oIntersections[1] = oTempIntersections[42];
            oHexes[1, 4].oIntersections[2] = oTempIntersections[41];
            oHexes[1, 4].oIntersections[3] = oTempIntersections[49];
            oHexes[1, 4].oIntersections[4] = oTempIntersections[50];
            oHexes[1, 4].oIntersections[5] = oTempIntersections[51];
            oHexes[2, 4].oIntersections[0] = oTempIntersections[45];
            oHexes[2, 4].oIntersections[1] = oTempIntersections[44];
            oHexes[2, 4].oIntersections[2] = oTempIntersections[43];
            oHexes[2, 4].oIntersections[3] = oTempIntersections[51];
            oHexes[2, 4].oIntersections[4] = oTempIntersections[52];
            oHexes[2, 4].oIntersections[5] = oTempIntersections[53];
            #endregion

            #region Connect Buildings (Old)
            //oHexes[0, 0].oIntersections[0].oBuilding = oTempBuildings[2];
            //oHexes[0, 0].oIntersections[1].oBuilding = oTempBuildings[1];
            //oHexes[0, 0].oIntersections[2].oBuilding = oTempBuildings[0];
            //oHexes[0, 0].oIntersections[3].oBuilding = oTempBuildings[8];
            //oHexes[0, 0].oIntersections[4].oBuilding = oTempBuildings[9];
            //oHexes[0, 0].oIntersections[5].oBuilding = oTempBuildings[10];
            //oHexes[1, 0].oIntersections[0].oBuilding = oTempBuildings[4];
            //oHexes[1, 0].oIntersections[1].oBuilding = oTempBuildings[3];
            //oHexes[1, 0].oIntersections[2].oBuilding = oTempBuildings[2];
            //oHexes[1, 0].oIntersections[3].oBuilding = oTempBuildings[10];
            //oHexes[1, 0].oIntersections[4].oBuilding = oTempBuildings[11];
            //oHexes[1, 0].oIntersections[5].oBuilding = oTempBuildings[12];
            //oHexes[2, 0].oIntersections[0].oBuilding = oTempBuildings[6];
            //oHexes[2, 0].oIntersections[1].oBuilding = oTempBuildings[5];
            //oHexes[2, 0].oIntersections[2].oBuilding = oTempBuildings[4];
            //oHexes[2, 0].oIntersections[3].oBuilding = oTempBuildings[12];
            //oHexes[2, 0].oIntersections[4].oBuilding = oTempBuildings[13];
            //oHexes[2, 0].oIntersections[5].oBuilding = oTempBuildings[14];

            //oHexes[0, 1].oIntersections[0].oBuilding = oTempBuildings[9];
            //oHexes[0, 1].oIntersections[1].oBuilding = oTempBuildings[8];
            //oHexes[0, 1].oIntersections[2].oBuilding = oTempBuildings[7];
            //oHexes[0, 1].oIntersections[3].oBuilding = oTempBuildings[17];
            //oHexes[0, 1].oIntersections[4].oBuilding = oTempBuildings[18];
            //oHexes[0, 1].oIntersections[5].oBuilding = oTempBuildings[19];
            //oHexes[1, 1].oIntersections[0].oBuilding = oTempBuildings[11];
            //oHexes[1, 1].oIntersections[1].oBuilding = oTempBuildings[10];
            //oHexes[1, 1].oIntersections[2].oBuilding = oTempBuildings[9];
            //oHexes[1, 1].oIntersections[3].oBuilding = oTempBuildings[19];
            //oHexes[1, 1].oIntersections[4].oBuilding = oTempBuildings[20];
            //oHexes[1, 1].oIntersections[5].oBuilding = oTempBuildings[21];
            //oHexes[2, 1].oIntersections[0].oBuilding = oTempBuildings[13];
            //oHexes[2, 1].oIntersections[1].oBuilding = oTempBuildings[12];
            //oHexes[2, 1].oIntersections[2].oBuilding = oTempBuildings[11];
            //oHexes[2, 1].oIntersections[3].oBuilding = oTempBuildings[21];
            //oHexes[2, 1].oIntersections[4].oBuilding = oTempBuildings[22];
            //oHexes[2, 1].oIntersections[5].oBuilding = oTempBuildings[23];
            //oHexes[3, 1].oIntersections[0].oBuilding = oTempBuildings[15];
            //oHexes[3, 1].oIntersections[1].oBuilding = oTempBuildings[14];
            //oHexes[3, 1].oIntersections[2].oBuilding = oTempBuildings[13];
            //oHexes[3, 1].oIntersections[3].oBuilding = oTempBuildings[23];
            //oHexes[3, 1].oIntersections[4].oBuilding = oTempBuildings[24];
            //oHexes[3, 1].oIntersections[5].oBuilding = oTempBuildings[25];

            //oHexes[0, 2].oIntersections[0].oBuilding = oTempBuildings[18];
            //oHexes[0, 2].oIntersections[1].oBuilding = oTempBuildings[17];
            //oHexes[0, 2].oIntersections[2].oBuilding = oTempBuildings[16];
            //oHexes[0, 2].oIntersections[3].oBuilding = oTempBuildings[27];
            //oHexes[0, 2].oIntersections[4].oBuilding = oTempBuildings[28];
            //oHexes[0, 2].oIntersections[5].oBuilding = oTempBuildings[29];
            //oHexes[1, 2].oIntersections[0].oBuilding = oTempBuildings[20];
            //oHexes[1, 2].oIntersections[1].oBuilding = oTempBuildings[19];
            //oHexes[1, 2].oIntersections[2].oBuilding = oTempBuildings[18];
            //oHexes[1, 2].oIntersections[3].oBuilding = oTempBuildings[29];
            //oHexes[1, 2].oIntersections[4].oBuilding = oTempBuildings[30];
            //oHexes[1, 2].oIntersections[5].oBuilding = oTempBuildings[31];
            //oHexes[2, 2].oIntersections[0].oBuilding = oTempBuildings[22];
            //oHexes[2, 2].oIntersections[1].oBuilding = oTempBuildings[21];
            //oHexes[2, 2].oIntersections[2].oBuilding = oTempBuildings[20];
            //oHexes[2, 2].oIntersections[3].oBuilding = oTempBuildings[31];
            //oHexes[2, 2].oIntersections[4].oBuilding = oTempBuildings[32];
            //oHexes[2, 2].oIntersections[5].oBuilding = oTempBuildings[33];
            //oHexes[3, 2].oIntersections[0].oBuilding = oTempBuildings[24];
            //oHexes[3, 2].oIntersections[1].oBuilding = oTempBuildings[23];
            //oHexes[3, 2].oIntersections[2].oBuilding = oTempBuildings[22];
            //oHexes[3, 2].oIntersections[3].oBuilding = oTempBuildings[33];
            //oHexes[3, 2].oIntersections[4].oBuilding = oTempBuildings[34];
            //oHexes[3, 2].oIntersections[5].oBuilding = oTempBuildings[35];
            //oHexes[4, 2].oIntersections[0].oBuilding = oTempBuildings[26];
            //oHexes[4, 2].oIntersections[1].oBuilding = oTempBuildings[25];
            //oHexes[4, 2].oIntersections[2].oBuilding = oTempBuildings[24];
            //oHexes[4, 2].oIntersections[3].oBuilding = oTempBuildings[35];
            //oHexes[4, 2].oIntersections[4].oBuilding = oTempBuildings[36];
            //oHexes[4, 2].oIntersections[5].oBuilding = oTempBuildings[37];

            //oHexes[0, 3].oIntersections[0].oBuilding = oTempBuildings[30];
            //oHexes[0, 3].oIntersections[1].oBuilding = oTempBuildings[29];
            //oHexes[0, 3].oIntersections[2].oBuilding = oTempBuildings[28];
            //oHexes[0, 3].oIntersections[3].oBuilding = oTempBuildings[38];
            //oHexes[0, 3].oIntersections[4].oBuilding = oTempBuildings[39];
            //oHexes[0, 3].oIntersections[5].oBuilding = oTempBuildings[40];
            //oHexes[1, 3].oIntersections[0].oBuilding = oTempBuildings[32];
            //oHexes[1, 3].oIntersections[1].oBuilding = oTempBuildings[31];
            //oHexes[1, 3].oIntersections[2].oBuilding = oTempBuildings[30];
            //oHexes[1, 3].oIntersections[3].oBuilding = oTempBuildings[40];
            //oHexes[1, 3].oIntersections[4].oBuilding = oTempBuildings[41];
            //oHexes[1, 3].oIntersections[5].oBuilding = oTempBuildings[42];
            //oHexes[2, 3].oIntersections[0].oBuilding = oTempBuildings[34];
            //oHexes[2, 3].oIntersections[1].oBuilding = oTempBuildings[33];
            //oHexes[2, 3].oIntersections[2].oBuilding = oTempBuildings[32];
            //oHexes[2, 3].oIntersections[3].oBuilding = oTempBuildings[42];
            //oHexes[2, 3].oIntersections[4].oBuilding = oTempBuildings[43];
            //oHexes[2, 3].oIntersections[5].oBuilding = oTempBuildings[44];
            //oHexes[3, 3].oIntersections[0].oBuilding = oTempBuildings[36];
            //oHexes[3, 3].oIntersections[1].oBuilding = oTempBuildings[35];
            //oHexes[3, 3].oIntersections[2].oBuilding = oTempBuildings[34];
            //oHexes[3, 3].oIntersections[3].oBuilding = oTempBuildings[44];
            //oHexes[3, 3].oIntersections[4].oBuilding = oTempBuildings[45];
            //oHexes[3, 3].oIntersections[5].oBuilding = oTempBuildings[46];

            //oHexes[0, 4].oIntersections[0].oBuilding = oTempBuildings[41];
            //oHexes[0, 4].oIntersections[1].oBuilding = oTempBuildings[40];
            //oHexes[0, 4].oIntersections[2].oBuilding = oTempBuildings[39];
            //oHexes[0, 4].oIntersections[3].oBuilding = oTempBuildings[47];
            //oHexes[0, 4].oIntersections[4].oBuilding = oTempBuildings[48];
            //oHexes[0, 4].oIntersections[5].oBuilding = oTempBuildings[49];
            //oHexes[1, 4].oIntersections[0].oBuilding = oTempBuildings[43];
            //oHexes[1, 4].oIntersections[1].oBuilding = oTempBuildings[42];
            //oHexes[1, 4].oIntersections[2].oBuilding = oTempBuildings[41];
            //oHexes[1, 4].oIntersections[3].oBuilding = oTempBuildings[49];
            //oHexes[1, 4].oIntersections[4].oBuilding = oTempBuildings[50];
            //oHexes[1, 4].oIntersections[5].oBuilding = oTempBuildings[51];
            //oHexes[2, 4].oIntersections[0].oBuilding = oTempBuildings[45];
            //oHexes[2, 4].oIntersections[1].oBuilding = oTempBuildings[44];
            //oHexes[2, 4].oIntersections[2].oBuilding = oTempBuildings[43];
            //oHexes[2, 4].oIntersections[3].oBuilding = oTempBuildings[51];
            //oHexes[2, 4].oIntersections[4].oBuilding = oTempBuildings[52];
            //oHexes[2, 4].oIntersections[5].oBuilding = oTempBuildings[53];
            #endregion

            #region Connect Nearby Buildings
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if ((y == 0 || y == 4) && x >= 3)
                        continue;
                    else if ((y == 1 || y == 3) && x >= 4)
                        continue;

                    oHexes[x, y].oIntersections[0].oNearbyBuildings[1] = oHexes[x, y].oIntersections[5];
                    oHexes[x, y].oIntersections[0].oNearbyBuildings[2] = oHexes[x, y].oIntersections[1];
                    oHexes[x, y].oIntersections[1].oNearbyBuildings[1] = oHexes[x, y].oIntersections[0];
                    oHexes[x, y].oIntersections[1].oNearbyBuildings[2] = oHexes[x, y].oIntersections[2];
                    oHexes[x, y].oIntersections[2].oNearbyBuildings[0] = oHexes[x, y].oIntersections[1];
                    oHexes[x, y].oIntersections[2].oNearbyBuildings[1] = oHexes[x, y].oIntersections[3];
                    oHexes[x, y].oIntersections[3].oNearbyBuildings[0] = oHexes[x, y].oIntersections[2];
                    oHexes[x, y].oIntersections[3].oNearbyBuildings[1] = oHexes[x, y].oIntersections[4];
                    oHexes[x, y].oIntersections[4].oNearbyBuildings[0] = oHexes[x, y].oIntersections[5];
                    oHexes[x, y].oIntersections[4].oNearbyBuildings[2] = oHexes[x, y].oIntersections[3];
                    oHexes[x, y].oIntersections[5].oNearbyBuildings[0] = oHexes[x, y].oIntersections[0];
                    oHexes[x, y].oIntersections[5].oNearbyBuildings[2] = oHexes[x, y].oIntersections[4];
                }
            }
            #endregion

            #region Connect Nearby Buildings (Old)
            //for (int y = 0; y < 5; y++)
            //{
            //    for (int x = 0; x < 5; x++)
            //    {
            //        if ((y == 0 || y == 4) && x >= 3)
            //            continue;
            //        else if ((y == 1 || y == 3) && x >= 4)
            //            continue;

            //        oHexes[x, y].oIntersections[0].oNearbyBuildings[1] = oHexes[x, y].oIntersections[5].oBuilding;
            //        oHexes[x, y].oIntersections[0].oNearbyBuildings[2] = oHexes[x, y].oIntersections[1].oBuilding;
            //        oHexes[x, y].oIntersections[1].oNearbyBuildings[1] = oHexes[x, y].oIntersections[0].oBuilding;
            //        oHexes[x, y].oIntersections[1].oNearbyBuildings[2] = oHexes[x, y].oIntersections[2].oBuilding;
            //        oHexes[x, y].oIntersections[2].oNearbyBuildings[0] = oHexes[x, y].oIntersections[1].oBuilding;
            //        oHexes[x, y].oIntersections[2].oNearbyBuildings[1] = oHexes[x, y].oIntersections[3].oBuilding;
            //        oHexes[x, y].oIntersections[3].oNearbyBuildings[0] = oHexes[x, y].oIntersections[2].oBuilding;
            //        oHexes[x, y].oIntersections[3].oNearbyBuildings[1] = oHexes[x, y].oIntersections[4].oBuilding;
            //        oHexes[x, y].oIntersections[4].oNearbyBuildings[0] = oHexes[x, y].oIntersections[5].oBuilding;
            //        oHexes[x, y].oIntersections[4].oNearbyBuildings[2] = oHexes[x, y].oIntersections[3].oBuilding;
            //        oHexes[x, y].oIntersections[5].oNearbyBuildings[0] = oHexes[x, y].oIntersections[0].oBuilding;
            //        oHexes[x, y].oIntersections[5].oNearbyBuildings[2] = oHexes[x, y].oIntersections[4].oBuilding;

            //        if (x != 0)
            //        {
            //            oHexes[x - 1, y].oIntersections[0].oNearbyBuildings[0] = oHexes[x, y].oIntersections[1].oBuilding;
            //            oHexes[x, y].oIntersections[2].oNearbyBuildings[2] = oHexes[x - 1, y].oIntersections[1].oBuilding;

            //            oHexes[x - 1, y].oIntersections[5].oNearbyBuildings[1] = oHexes[x, y].oIntersections[4].oBuilding;
            //            oHexes[x, y].oIntersections[3].oNearbyBuildings[2] = oHexes[x - 1, y].oIntersections[4].oBuilding;
            //        }

            //        if (y < 2)
            //        {
            //            oHexes[x, y].oIntersections[3].oNearbyBuildings[2] = oHexes[x, y + 1].oIntersections[2].oBuilding;
            //            oHexes[x, y + 1].oIntersections[1].oNearbyBuildings[0] = oHexes[x, y].oIntersections[2].oBuilding;

            //            oHexes[x, y].oIntersections[4].oNearbyBuildings[1] = oHexes[x, y + 1].oIntersections[5].oBuilding;
            //            oHexes[x, y + 1].oIntersections[0].oNearbyBuildings[0] = oHexes[x, y].oIntersections[5].oBuilding;

            //            if ((x == 2 && y == 0) || (x == 3 && y == 1))
            //            {
            //                oHexes[x, y].oIntersections[5].oNearbyBuildings[1] = oHexes[x + 1, y + 1].oIntersections[0].oBuilding;
            //                oHexes[x + 1, y + 1].oIntersections[1].oNearbyBuildings[0] = oHexes[x, y].oIntersections[0].oBuilding;
            //            }
            //        }
            //        else if (y > 2)
            //        {
            //            oHexes[x, y].oIntersections[1].oNearbyBuildings[0] = oHexes[x + 1, y - 1].oIntersections[2].oBuilding;
            //            oHexes[x + 1, y - 1].oIntersections[3].oNearbyBuildings[2] = oHexes[x, y].oIntersections[2].oBuilding;

            //            oHexes[x, y].oIntersections[0].oNearbyBuildings[0] = oHexes[x + 1, y - 1].oIntersections[5].oBuilding;
            //            oHexes[x + 1, y - 1].oIntersections[4].oNearbyBuildings[1] = oHexes[x, y].oIntersections[5].oBuilding;

            //            if (x == 0)
            //            {
            //                oHexes[x, y].oIntersections[2].oNearbyBuildings[2] = oHexes[x, y - 1].oIntersections[3].oBuilding;
            //                oHexes[x, y - 1].oIntersections[4].oNearbyBuildings[1] = oHexes[x, y].oIntersections[3].oBuilding;
            //            }
            //        }
            //    }
            //}

            //for (int y = 0; y < 2; y++)
            //{
            //    for (int x = 0; x < 5; x++)
            //    {
            //        if ((y == 0 || y == 4) && x >= 3)
            //            continue;
            //        else if ((y == 1 || y == 3) && x >= 4)
            //            continue;

            //        if (x == 0)
            //        {
            //            oHexes[x, y].oIntersections[3].oNearbyBuildings[2] = oHexes[x, y + 1].oIntersections[1].oNearbyBuildings[2];
            //            oHexes[x, y + 1].oIntersections[1].oNearbyBuildings[0] = oHexes[x, y].oIntersections[3].oNearbyBuildings[0];

            //            oHexes[x, y].oIntersections[4].oNearbyBuildings[1] = oHexes[x, y + 1].oIntersections[0].oNearbyBuildings[1];
            //            oHexes[x, y + 1].oIntersections[0].oNearbyBuildings[0] = oHexes[x, y].oIntersections[4].oNearbyBuildings[0];
            //        }
            //    }
            //}
            #endregion

            #region Connect Nearby Roads
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (oHexes[x, y] != null)
                    {
                        int nStartingID; // the first id to be used for a row
                        int nOffset; // offset between the first id on top of a hex and below for each row
                        int[] nOffset2 = new int[5] { 12, 15, 16, 14, 10 };

                        // starting id for each row
                        if (y == 0)
                            nStartingID = 0;
                        else if (y == 1)
                            nStartingID = 10;
                        else if (y == 2)
                            nStartingID = 23;
                        else if (y == 3)
                            nStartingID = 40;
                        else
                            nStartingID = 55;

                        // building id offset for each row
                        if (y == 0)
                            nOffset = 12;
                        else if (y == 1)
                            nOffset = 15;
                        else if (y == 2)
                            nOffset = 16;
                        else if (y == 3)
                            nOffset = 14;
                        else
                            nOffset = 11;

                        oHexes[x, y].oIntersections[2].oNearbyRoads[1] = oTempRoads[nStartingID + (x * 3)];
                        oHexes[x, y].oIntersections[2].oNearbyRoads[0] = oTempRoads[nStartingID + 1 + (x * 3)];

                        oHexes[x, y].oIntersections[1].oNearbyRoads[2] = oTempRoads[nStartingID + 1 + (x * 3)];
                        oHexes[x, y].oIntersections[1].oNearbyRoads[1] = oTempRoads[nStartingID + 2 + (x * 3)];

                        oHexes[x, y].oIntersections[0].oNearbyRoads[2] = oTempRoads[nStartingID + 2 + (x * 3)];
                        oHexes[x, y].oIntersections[0].oNearbyRoads[1] = oTempRoads[nStartingID + 3 + (x * 3)];

                        oHexes[x, y].oIntersections[5].oNearbyRoads[0] = oTempRoads[nStartingID + 3 + (x * 3)];
                        oHexes[x, y].oIntersections[5].oNearbyRoads[2] = oTempRoads[nStartingID + nOffset + 2 + (x * 3)];

                        oHexes[x, y].oIntersections[4].oNearbyRoads[0] = oTempRoads[nStartingID + nOffset + 2 + (x * 3)];
                        oHexes[x, y].oIntersections[4].oNearbyRoads[2] = oTempRoads[nStartingID + nOffset + (x * 3)];

                        oHexes[x, y].oIntersections[3].oNearbyRoads[1] = oTempRoads[nStartingID + nOffset + (x * 3)];
                        oHexes[x, y].oIntersections[3].oNearbyRoads[0] = oTempRoads[nStartingID + (x * 3)];

                        if ((y == 0 && x > 0) || // left
                                (y == 1 && x > 0) ||
                                (y == 2 && x > 0) ||
                                (y > 2))
                            oHexes[x, y].oIntersections[2].oNearbyRoads[2] = oTempRoads[nStartingID - 1 + (x * 3)];

                        if (y > 0) // top left
                            oHexes[x, y].oIntersections[1].oNearbyRoads[0] = oTempRoads[nStartingID - nOffset2[y - 1] + 2 + (x * 3)];

                        if ((y == 0 && x < 2) || // top right
                            (y == 1 && x < 3) ||
                            (y == 2 && x < 4) ||
                            (y > 2))
                            oHexes[x, y].oIntersections[0].oNearbyRoads[0] = oTempRoads[nStartingID + 4 + (x * 3)];

                        if ((y < 2) || // right
                            (y == 2 && x < 4) ||
                            (y == 3 && x < 3) ||
                            (y == 4 && x < 2))
                            oHexes[x, y].oIntersections[5].oNearbyRoads[1] = oTempRoads[nStartingID + nOffset + 3 + (x * 3)];

                        if (y < 4) // bottom right
                            oHexes[x, y].oIntersections[4].oNearbyRoads[1] = oTempRoads[nStartingID + nOffset + 1 + (x * 3)];

                        if ((y < 2) || // bottom left 
                            (y == 2 && x > 0) ||
                            (y == 3 && x > 0) ||
                            (y == 4 && x > 0))
                            oHexes[x, y].oIntersections[3].oNearbyRoads[2] = oTempRoads[nStartingID + nOffset - 1 + (x * 3)];
                    }
                }
            }
            #endregion

            #region Create Intersections
            for (int y = 0; y < 5; y++)
                for (int x = 4; x > -1; x--)
                    if (oHexes[x, y] != null)
                    {
                        oHexes[x, y].CreateIntersections();
                        for (int z = 0; z < 6; z++)
                            oHexes[x, y].oIntersections[z].PositionStructures(z);
                    }
            #endregion

            #region Create Ports
            int[] nHexEdges = { 0, 1, 2, 3, 4, 5, 6, 14, 15, 25, 26, 37, 36, 46, 45, 53, 52, 51, 50, 49, 48, 47, 39, 38, 28, 27, 16, 17, 7, 8 };
            int[] nPortCount = { 1, 1, 1, 1, 1, 4 };
            int nPosition = rand.Next() % 6;
            int nPortType;

            do
            {
                nPosition += 3;
                if (nPosition > (nHexEdges.Count() - 1))
                    nPosition = nPosition - (nHexEdges.Count() - 1);

                do
                {
                    nPortType = rand.Next() % 6;
                } 
                while (nPortCount[nPortType] == 0);

                nPortCount[nPortType]--;
                oTempIntersections[nHexEdges[nPosition]].nPortType = nPortType;

                if (nPosition + 1 > (nHexEdges.Count() - 1))
                    nPosition = 0;
                oTempIntersections[nHexEdges[nPosition + 1]].nPortType = nPortType;
            }
            while ((nPortCount[0] + nPortCount[1] + nPortCount[2] + nPortCount[3] + nPortCount[4] + nPortCount[5]) > 0);

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    if (oHexes[x, y] != null)
                    {
                        oHexes[x, y].PositionPorts();
                    }
                }
            }
            #endregion

            #region Count Used Roads
            //            for (int y = 0; y < 5; y++)
            //            {
            //                for (int x = 0; x < 5; x++)
            //                {
            //                    if (oHexes[x, y] != null)
            //                    {
            //                        for (int z = 0; z < 6; z++)
            //                        {
            //                            for (int a = 0; a < 3; a++)
            //                            {
            //                                if (oHexes[x, y].oIntersections[z].oNearbyRoads[a] != null)
            //                                {
            //                                    oHexes[x, y].oIntersections[z].oNear
            //                //oTempIntersections[nHexEdges[nPosition]].Visible = true;
            //byRoads[a].nUsed++;
            //                                    oHexes[x, y].oIntersections[
            //z].oNearbyRoads[a].Label = "" + oHexes[x, y].oIntersections[z].oNearbyRoads[a].nUsed;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            #endregion

            #region Count Used Buildings
            //for (int y = 0; y < 5; y++)
            //{
            //    for (int x = 0; x < 5; x++)
            //    {
            //        if (oHexes[x, y] != null)
            //        {
            //            for (int z = 0; z < 6; z++)
            //            {
            //                for (int a = 0; a < 3; a++)
            //                {
            //                    if (oHexes[x, y].oIntersections[z].oNearbyBuildings[a] != null)
            //                    {
            //                        oHexes[x, y].oIntersections[z].oNearbyBuildings[a].nUsed++;
            //                        oHexes[x, y].oIntersections[z].oNearbyBuildings[a].Label = "" + oHexes[x, y].oIntersections[z].oNearbyBuildings[a].nUsed;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //for (int y = 0; y < 5; y++)
            //{
            //    for (int x = 0; x < 5; x++)
            //    {
            //        if (oHexes[x, y] != null)
            //        {
            //            for (int z = 0; z < 6; z++)
            //            {
            //                for (int a = 0; a < 3; a++)
            //                {
            //                    if (oHexes[x, y].oIntersections[z].oNearbyBuildings[a] != null)
            //                        oHexes[x, y].oIntersections[z].oNearbyBuildings[a].nUsed = 0;
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion

            #region Delete Unused Roads
            oTempRoads[67].RemoveFromAutoDrawSet();
            oTempRoads[70].RemoveFromAutoDrawSet();
            oTempRoads[73].RemoveFromAutoDrawSet();
            #endregion

            // test data
            oHexes[1, 3].oIntersections[1].nOwner = 0;
            oHexes[1, 1].oIntersections[2].nOwner = 0;
            oHexes[1, 2].oIntersections[3].nOwner = 0;
            oHexes[1, 3].oIntersections[1].eState = Intersection.STATE.SETTLEMENT;
            oHexes[1, 1].oIntersections[2].eState = Intersection.STATE.SETTLEMENT;
            oHexes[1, 2].oIntersections[3].eState = Intersection.STATE.SETTLEMENT;
            oHexes[1, 3].oIntersections[1].Visible = true;
            oHexes[1, 1].oIntersections[2].Visible = true;
            oHexes[1, 2].oIntersections[3].Visible = true;

            oRobber = new Robber(oRobberStartingLocation, oRobberStartingHex);
            oHUD = new HUD(oStats);
            oHUD.ShowCards(oStats.oPlayers[0].nCards, oCamera);
            //oChatMenu = new ChatMenu(oCamera);
        }

        public Game1.MENU_STATE Update(Cursor oCursor, Camera oCamera)
        {
            //XNACS1Base.EchoToBottomStatus("basicgame:" + oStats.nLongestRoad + " " + oStats.nLongestRoadOwner);
            //XNACS1Base.EchoToBottomStatus("HUD:" + oHexes[2, 0].oIntersections[0].bPorts[0] );
            //XNACS1Base.EchoToBottomStatus("HUD:" + oHexes[1, 0].oIntersections[0].bPorts[0]);
            oKeyState = Keyboard.GetState();

            #region Create Cards (1-7)
            if (nCurrentButtonDelay > 0)
                nCurrentButtonDelay--;

            if (nCurrentButtonDelay == 0)
            {
                if (oKeyState.IsKeyDown(Keys.D1))
                {
                    for(int x = 0; x < oStats.oPlayers.Count(); x++)
                        oStats.oPlayers[x].nCards[0]++;

                    nCurrentButtonDelay = nButtonDelay;
                    oHUD.ShowCards(oStats.oPlayers[0].nCards, oCamera);
                }
                else if (oKeyState.IsKeyDown(Keys.D2))
                {
                    for (int x = 0; x < oStats.oPlayers.Count(); x++)
                        oStats.oPlayers[x].nCards[1]++;

                    nCurrentButtonDelay = nButtonDelay;
                    oHUD.ShowCards(oStats.oPlayers[0].nCards, oCamera);
                }
                else if (oKeyState.IsKeyDown(Keys.D3))
                {
                    for (int x = 0; x < oStats.oPlayers.Count(); x++)
                        oStats.oPlayers[x].nCards[2]++;

                    nCurrentButtonDelay = nButtonDelay;
                    oHUD.ShowCards(oStats.oPlayers[0].nCards, oCamera);
                }
                else if (oKeyState.IsKeyDown(Keys.D4))
                {
                    for (int x = 0; x < oStats.oPlayers.Count(); x++)
                        oStats.oPlayers[x].nCards[3]++;

                    nCurrentButtonDelay = nButtonDelay;
                    oHUD.ShowCards(oStats.oPlayers[0].nCards, oCamera);
                }
                else if (oKeyState.IsKeyDown(Keys.D5))
                {
                    for (int x = 0; x < oStats.oPlayers.Count(); x++)
                        oStats.oPlayers[x].nCards[4]++;

                    nCurrentButtonDelay = nButtonDelay;
                    oHUD.ShowCards(oStats.oPlayers[0].nCards, oCamera);
                }
            }
            #endregion

            oRobber.Update(oHexes, oCamera, oCursor, oStats);
            oStats.Update();
            //oChatMenu.Update(oCamera, oCursor, oStats.oPlayers[0]);

            #region Update Hexes
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (oHexes[x, y] != null)
                        oHexes[x, y].Update(oCursor, oCamera, oStats, oHUD);
                }
            }
            #endregion

            oHUD.Update(oCursor, oCamera, oStats, oHexes, oRobber);

            #region Calculate Longest Road (Tab)
            if (nCurrentButtonDelay == 0)
            {
                if (oKeyState.IsKeyDown(Keys.Tab))
                {
                    nCurrentButtonDelay = nButtonDelay;

                    GetLongestRoad(oStats.nCurrentPlayer);
                }
            }
            #endregion

            return Game1.MENU_STATE.BASIC_GAME;
        }

        public void GetLongestRoad(int nPlayer)
        {
            GroupRoads(nPlayer);

        //    Intersection oTemp = new Intersection();

        //    for (int y = 0; y < 5; y++)
        //    {
        //        for (int x = 0; x < 5; x++)
        //        {
        //            if (oHexes[x, y] != null)
        //            {
        //                for (int z = 0; z < 6; z++)
        //                {
        //                    for (int a = 0; a < 3; a++)
        //                    {
        //                        if (oHexes[x, y].oIntersections[z].oNearbyRoads[a] != null &&
        //                           oHexes[x, y].oIntersections[z].oNearbyRoads[a].nOwner == nPlayer)
        //                        {
        //                            oTemp = oHexes[x, y].oIntersections[z];
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    bool bEnd;
        //    do {
        //        bEnd = true;

        //        for (int x = 0; x < 3; x++)
        //        {
        //            if (oTemp.oNearbyRoads[x] != null && oTemp.oNearbyRoads[x].nOwner == nPlayer)
        //            {
        //                oTemp.oNearbyRoads[x].Color = Color.Purple;

        //            }
        //        }
        //    } while(bEnd == false);

        //    return oTemp;
        }

        public void GroupRoads(int nPlayer)
        {
            int nCounter = 1;

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (oHexes[x, y] != null)
                    {
                        for (int z = 0; z < 6; z++)
                        {
                            for (int a = 0; a < 3; a++)
                            {
                                if (oHexes[x, y].oIntersections[z].oNearbyRoads[a] != null &&
                                    oHexes[x, y].oIntersections[z].oNearbyRoads[a].nOwner == nPlayer &&
                                    oHexes[x, y].oIntersections[z].oNearbyRoads[a].nGroup != nCounter)
                                {
                                    oHexes[x, y].oIntersections[z].oNearbyRoads[a].nGroup = nCounter;
                                    Helper(oHexes[x, y].oIntersections[z].oNearbyBuildings[a], nPlayer, nCounter, 0);
                                    nCounter++;
                                }
                            }
                        }
                    }
                }
            }

            //int nCounter = 1;

            //for (int y = 0; y < 5; y++)
            //{
            //    for (int x = 0; x < 5; x++)
            //    {
            //        if (oHexes[x, y] != null)
            //        {
            //            for (int z = 0; z < 6; z++)
            //            {
            //                for (int a = 0; a < 3; a++)
            //                {
            //                    if (oHexes[x, y].oIntersections[z].oNearbyRoads[a] != null &&
            //                        oHexes[x, y].oIntersections[z].oNearbyRoads[a].nOwner == nPlayer &&
            //                        oHexes[x, y].oIntersections[z].oNearbyRoads[a].nGroup == 0)
            //                    {
            //                        oHexes[x, y].oIntersections[z].oNearbyRoads[a].nGroup = nCounter;
            //                        Helper(oHexes[x, y].oIntersections[z].oNearbyBuildings[a], nPlayer, nCounter);
            //                        nCounter++;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        public void Helper(Intersection oIntersection, int nPlayer, int nCounter, int nCurrentLongestRoad)
        {
            nCurrentLongestRoad++;

            if (nCurrentLongestRoad > oStats.nLongestRoad)
            {
                oStats.nLongestRoad = nCurrentLongestRoad;
                oStats.nLongestRoadOwner = nPlayer;
            }

            for (int a = 0; a < 3; a++)
            {
                if (oIntersection.oNearbyRoads[a] != null &&
                    oIntersection.oNearbyRoads[a].nOwner == nPlayer &&
                    oIntersection.oNearbyRoads[a].nGroup != nCounter)
                {
                    
                    oIntersection.oNearbyRoads[a].nGroup = nCounter;
                    Helper(oIntersection.oNearbyBuildings[a], nPlayer, nCounter, nCurrentLongestRoad);
                }
            }
        }
    }
}
