using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    class Graph
    {
        XNACS1Rectangle oBox = new XNACS1Rectangle(new Vector2(), 25f, 15f);
        Vector2 oBoxOffset;
        XNACS1Rectangle oTitleBox;
        XNACS1Rectangle[] oBars;
        XNACS1Rectangle[] oBarLabels;
        XNACS1Rectangle[] oOtherBars;
        XNACS1Rectangle[] oBarValues;
        int[] nBarValues;

        float fBottomLabelsOffset = 1.3f;

        float fHorizontalBorders = 1.4f;
        float fVerticalBorders = 4.5f;
        float fSpaceBetweenBars = 0.25f;
        float fMinimumBarHeight = 0.201f;

        public Graph(Vector2 oBoxOffset, Vector2 oSize, int nXBars, int[] nValues, string sTitle, string[] sLabels, string sTexture = null)
        {
            //this.oBoxOffset = new Vector2(-50f, -50f); ;
            this.oBoxOffset = oBoxOffset;
            //this.oBoxOffset = new Vector2(100f,100f);
            //oBox.Color = new Color(0, 0, 0, 0);
            oBox.Color = Color.Blue;
            oBox.Width = oSize.X;
            oBox.Height = oSize.Y;
            //oBox.Color = new Color(235, 176, 48);

            if (sTexture == null)
                oBox.Color = new Color(0, 0, 0, 0);
            else
                oBox.Texture = sTexture;

            //oBox.TextureTintColor = new Color(100, 100, 100, 100);
            
            oTitleBox = new XNACS1Rectangle();
            //oTitleBox.Color = new Color(0, 0, 0, 160);
            oTitleBox.Color = new Color(0, 0, 0, 0);
            oTitleBox.Label = sTitle;
            oTitleBox.LabelColor = Color.White;
            oTitleBox.LabelFont = "GraphLabels";
            oTitleBox.Width = oSize.X;
            oTitleBox.Height = 3.4f;

            oBars = new XNACS1Rectangle[nXBars];
            nBarValues = new int[nXBars];
            oBarLabels = new XNACS1Rectangle[nXBars];
            oBarValues = new XNACS1Rectangle[nXBars];
            oOtherBars = new XNACS1Rectangle[2];

            // test data
            //nBarValues[0] = 0;
            //nBarValues[1] = 1;
            //nBarValues[2] = 2;
            //nBarValues[3] = 3;
            //nBarValues[4] = 10;  
            for (int x = 0; x < oBars.Count(); x++)
            {
                nBarValues[x] = nValues[x];
            }

            for (int x = 0; x < oBars.Count(); x++)
            {
                oBarLabels[x] = new XNACS1Rectangle();
                oBarLabels[x].Color = new Color(0, 0, 0, 150);
                oBarLabels[x].LabelColor = Color.White;
                oBarLabels[x].Label = sLabels[x];
                oBarLabels[x].LabelFont = "GraphLabels";
            }

            for (int x = 0; x < 2; x++)
            {
                oOtherBars[x] = new XNACS1Rectangle();
                oOtherBars[x].Color = Color.White;
                oOtherBars[x].Visible = false;
            }
            oOtherBars[0].Width = 0.2f;
            oOtherBars[0].Height = oBox.Height - fVerticalBorders;
            oOtherBars[1].Width = oBox.Width;
            oOtherBars[1].Height = 0.2f;

            for (int x = 0; x < oBars.Count(); x++)
            {
                oBars[x] = new XNACS1Rectangle(new Vector2(), (oBox.Width - (fHorizontalBorders * 2) - ((oBars.Count() - 1) * fSpaceBetweenBars)) / oBars.Count(), fMinimumBarHeight);
                oBarLabels[x].Width = oBars[x].Width;
                oBarLabels[x].Height = 2f;
                oBars[x].Color = Color.LightGoldenrodYellow;
                oBarValues[x] = new XNACS1Rectangle();
                oBarValues[x].Color = new Color(0, 0, 0, 0);        
                oBarValues[x].LabelColor = Color.Red;
                oBarValues[x].LabelFont = "GraphLabels";

                if(nBarValues[x] > 0)
                    oBarValues[x].Label = "" + nBarValues[x];
            }


        }

        public void ResizeBars()
        {
            #region Resize Bars
            int nHighestValue = 0;

            for (int x = 0; x < oBars.Count(); x++)
                if(nBarValues[x] > nHighestValue)
                    nHighestValue = nBarValues[x];

            for (int x = 0; x < oBars.Count(); x++)
            {
                if (nBarValues[x] > 0)
                    oBars[x].Height = (oBox.Height - (fVerticalBorders * 2)) * nBarValues[x] / nHighestValue;
                else
                    oBars[x].Height = fMinimumBarHeight;
            }
            #endregion
        }

        public void Recenter(XNACS1Rectangle oContainer)
        {
            oBox.Center = oContainer.Center + oBoxOffset;

            oTitleBox.Center = new Vector2(oBox.CenterX, oBox.CenterY + oBox.Height / 2f - 2.3f);
            oBars[0].Center = new Vector2(oBox.CenterX - (oBox.Width / 2) + (oBars[0].Width / 2) + fHorizontalBorders,
                                          oBox.CenterY - (oBox.Height / 2) + (oBars[0].Height / 2) + fVerticalBorders);

            // the actual bars
            for (int x = 1; x < oBars.Count(); x++)
            {
                oBars[x].Center = oBars[x - 1].Center + new Vector2(oBars[x - 1].Width + fSpaceBetweenBars, 0f);
                oBars[x].CenterY = oBox.CenterY - (oBox.Height / 2) + (oBars[x].Height / 2) + fVerticalBorders;
            }

            // values on the bars
            for (int x = 0; x < oBarValues.Count(); x++)
            {
                oBarValues[x].CenterX = oBars[x].CenterX;

                if (oBars[x].Height > 1.5f)
                    oBarValues[x].CenterY = oBars[x].CenterY;
                else
                    oBarValues[x].CenterY = oBars[x].CenterY + oBars[x].Height / 2f + 1f;
            }

            // x-axis labels 
            for (int x = 0; x < oBars.Count(); x++)
            {
                oBarLabels[x].CenterX = oBars[x].CenterX;
                oBarLabels[x].CenterY = oBox.CenterY - (oBox.Height / 2) + fBottomLabelsOffset + 1.2f; ;
            }

            oOtherBars[0].Center = oBox.Center + new Vector2(-oBox.Width / 2 + fHorizontalBorders - 0.4f, 0f);
            oOtherBars[1].Center = oBox.Center + new Vector2(0f, -oBox.Height / 2 + fVerticalBorders - 0.4f);
        } 

        public void Clear()
        {
            oBox.RemoveFromAutoDrawSet();
            oTitleBox.RemoveFromAutoDrawSet();

            for (int x = 0; x < oOtherBars.Count(); x++)
            {
                oOtherBars[x].RemoveFromAutoDrawSet();
            }

            for (int x = 0; x < oBars.Count(); x++)
            {
                oBarValues[x].RemoveFromAutoDrawSet();
                oBars[x].RemoveFromAutoDrawSet();
                oBarLabels[x].RemoveFromAutoDrawSet();
            }
        }
    }
}
