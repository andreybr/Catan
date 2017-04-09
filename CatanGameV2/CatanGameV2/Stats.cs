using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class Stats
    {
        bool bGameOver = false;
        public int nLongestRoad = 0;
        public int nLongestRoadOwner = -1;
        int nLargestArmy = 0;
        int nLargestArmyOwner = -1;
        int[] nResourcesGainedCount = {0, 0, 0, 0, 0}; // count of resources gained, for graph
        int[] nRollCount = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}; // count of different values rolled, for graph
        public int nCurrentPlayer = 0;
        public Player[] oPlayers = new Player[4];
        public int[] nResourceCardsLeft = { 14, 5, 2, 2, 2 };
        public int nCurrentTurn = 1;

        public List<int[]> nTurnResourceRecords = new List<int[]>(); // records of resources gained each turn
        const int nMaxRecords = 10; // number of records to show on  

        public int[] nRollCounts = new int[11]; // count of all rolls made
        public int[] nResourceCounts = new int[5]; // count of all resouces gained

        public int[] nTotalResourcesGained = new int[4];
        public int[] nTotalResourcesLost = new int[4];

        public int[] nResourcesGainedFromTrading = new int[4]; // resources gained from trading / lost from trading
        public int[] nResourcesLostFromTrading = new int[4]; // resources gained from trading / lost from trading

        public int[] nResourcesGainedByRobber = new int[4];
        public int[] nResourcesLostByRobber = new int[4];

        public Stats()
        {
            for (int x = 0; x < oPlayers.Count(); x++)
            {
                oPlayers[x] = new Player();
                oPlayers[x].nPlayerNumber = x;
                oPlayers[x].sName = "Player" + x;
            }
            
            // test
            //oPlayers[0].oPlayerColor = Color.Red;
            //oPlayers[1].oPlayerColor = Color.Purple;
            //oPlayers[2].oPlayerColor = Color.Orange;
            //oPlayers[3].oPlayerColor = Color.Blue;
        }

        public void Update()
        {
            for (int x = 0; x < oPlayers.Count(); x++)
            {
                oPlayers[x].UpdateVictoryPoints(nLongestRoadOwner, nLargestArmyOwner);
                oPlayers[x].UpdateCardCount();
            }

            //XNACS1Base.EchoToTopStatus("statistics:" + oPlayers[0].nResourceCards[2] + "" + oPlayers[0].nResourceCards[3]);
            XNACS1Base.EchoToTopStatus("statistics:" + nCurrentPlayer);
        }

        public void AddTurnRecord(int[] nNewRecord)
        {
            nTurnResourceRecords.Insert(0, nNewRecord); //insert new record at the front

            if (nTurnResourceRecords.Count > nMaxRecords)
                nTurnResourceRecords.RemoveAt(nMaxRecords);
        }

        public void AddRollRecord(int nIndex)
        {
            nRollCounts[nIndex]++;
        }

        public void AddResourceRecord(int nIndex, int nGainedAmount)
        {
            nResourceCounts[nIndex] += nGainedAmount;
        }

        public void PassTurn()
        {
            nCurrentPlayer++;

            if (nCurrentPlayer == 4)
                nCurrentPlayer = 0;

            nCurrentTurn++;
        }
    }
}
