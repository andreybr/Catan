using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class Road : XNACS1Rectangle
    {
        private float fWidth = Global.PixelSize(36); // width of image
        private float fHeight = Global.PixelSize(29); // height of image
        public int nOwner = -1; // player who owns this road, -1 = not owned

        public Color oUnselectedColor = new Color(0, 0, 0, 250);
        public Color oHoverColor = new Color(0, 0, 0, 160);

        public int nUsed;
        public int nID;

        public int nGroup = 0;
        public int nCount = 0; // test

        public Road(int nID, Vector2 oCenter)
        {
            this.Center = oCenter;
            this.Width = fWidth;
            this.Height = fHeight;
            this.LabelColor = Color.White;
            this.LabelFont = "Small";
            this.nID = nID;

            // test
            //this.Label = "" + nID;
            //this.TextureTintColor = Color.Green;
        }

        public void Update()
        {
            this.Label = "" + nOwner;
        }
    }
}
