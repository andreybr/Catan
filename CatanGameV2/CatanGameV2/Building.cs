using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    class Building : XNACS1Rectangle
    {
        private const float fSettlementWidth = 3f; // width of image
        private const float fSettlementHeight = 3f; // height of image
        private const float fCityWidth = 4f; // width of image
        private const float fCityHeight = 4f; // height of image
        public int nOwner = 0; // player who owns this road, 0 = not owned
        public enum STATE { NONE, SETTLEMENT, CITY }
        public STATE oState = STATE.NONE;
        public Color oUnselectedColor = new Color(0, 0, 0, 160);
        public Color oHoverColor = new Color(0, 0, 0, 160);
        public int nCount;
        public int nUsed;
        public string str;
        public int nID;

        public Building(int nID)
        {
            this.Width = fSettlementWidth;
            this.Height = fSettlementHeight;
            this.LabelColor = Color.White;
            this.LabelFont = "Small";
            this.Color = oUnselectedColor;
            this.nID = nID;
        }

        public void Update(Cursor oCursor)
        {
            this.Label = "";
        }
    }
}
