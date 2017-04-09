using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class Player
    {
        public string sName = "Player";
        public int nPlayerNumber = 0;
        public Color oPlayerColor = Color.Green;
        public int nVictoryPoints = 0;
        public int nArmyCount = 0;
        public int nLongestRoad = 0;
        public int nResourceCardCount = 0;
        public int nDevelopmentCardCount = 0;
        public int nCities = 0;
        public int nSettlements = 0;
        public bool[] bPorts = { true, false, true, false, true, false }; // which port types are owned
        //public int[] nCards = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}; // cards in hand
        //public int[] nCards = { 2, 2, 2, 2, 2, 1, 1, 1, 1, 1 }; // cards in hand
        public int[] nCards = {1,1,1,1,1, 0, 0, 0, 0, 0}; // cards in hand
        //public int[] nCards = { 3,3,3,3,3,3,3,3,3,3 }; // cards in hand
        public enum CurrentlyBuilding { NONE, ROAD, SETTLEMENT, CITY, ROAD_BUILDING };
        public CurrentlyBuilding eCurrentlyBuilding = CurrentlyBuilding.NONE;
        public int nFreeRoads = 0;
        public bool bCanBeRobbed = false;
        
        public Player()
        {
            
        }

        public void UpdateCardCount()
        {
            nResourceCardCount = nCards[0] + nCards[1] + nCards[2] + nCards[3] + nCards[4];
            nDevelopmentCardCount = nCards[5] + nCards[6] + nCards[7] + nCards[8] + nCards[9];
        }

        public void UpdateVictoryPoints(int nLongestRoadOwner, int nLargestArmyOwner)
        {
            int nPoints = 0;

            if (nLongestRoadOwner == nPlayerNumber)
                nPoints += 2;

            if (nLargestArmyOwner == nPlayerNumber)
                nPoints += 2;

            nPoints += nCities * 2;
            nPoints += nSettlements;
            nPoints += nCards[6];

            nVictoryPoints = nPoints;
        }

        public bool CanBuildRoad()
        {
            if (nCards[1] >= 1 && nCards[4] >= 1)
                return true;
            else
                return false;
        }

        public bool CanBuildSettlement()
        {
            if (nCards[0] >= 1 && nCards[1] >= 1 &&
                nCards[3] >= 1 && nCards[4] >= 1)
                return true;
            else
                return false;
        }

        public bool CanBuildCity()
        {
            if (nCards[2] >= 3 && nCards[3] >= 2)
                return true;
            else
                return false;
        }

        public bool CanGetDevelopment(int[] nResourceCardsLeft)
        {
            if (nCards[0] >= 1 && nCards[2] >= 1 &&
                nCards[3] >= 1)
            {
                if (nResourceCardsLeft[0] > 0 || nResourceCardsLeft[1] > 0 ||
                   nResourceCardsLeft[2] > 0 || nResourceCardsLeft[3] > 0 ||
                   nResourceCardsLeft[4] > 0)
                {
                    return true;
                }
            }
            
            return false;
        }

        public void BuildRoad()
        {
            nCards[1]--;
            nCards[4]--;
        }

        public void BuildSettlement()
        {
            nCards[0]--;
            nCards[1]--;
            nCards[3]--;
            nCards[4]--;
        }

        public void BuildCity()
        {
            nCards[2] -= 3;
            nCards[3] -= 2;
        }

        public void GetDevelopment(int[] nResourceCardsLeft, HUD oHUD)
        {
            Random rand = new Random();
            int nNumCards = nResourceCardsLeft[0] + nResourceCardsLeft[1] + nResourceCardsLeft[2] + nResourceCardsLeft[3] + nResourceCardsLeft[4];
            int nRoll = rand.Next() % nNumCards + 1;
            int nTotal = 0;

            for (int x = 0; x < nResourceCardsLeft.Count(); x++)
            {
                nTotal += nResourceCardsLeft[x];

                if (nTotal >= nRoll)
                {
                    nResourceCardsLeft[x]--;
                    nCards[x + 5]++;
                    break;
                }
            }
        }

        public int GetRobbed()
        {
            Random rand = new Random();
            int nRoll = rand.Next() % nResourceCardCount + 1;
            int nTotal = 0;

            for (int x = 0; x < nCards.Count(); x++)
            {
                nTotal += nCards[x];

                if (nTotal >= nRoll)
                {
                    nCards[x]--;
                    return x;
                }
            }

            return -1;
        }
    }
}
