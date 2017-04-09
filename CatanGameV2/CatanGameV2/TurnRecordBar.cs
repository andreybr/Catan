using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class TurnRecordBar : XNACS1Rectangle
    {
        Vector2 oDisplacement;

        Color oNoneColor = new Color(70, 70, 70, 70);
        Color oWoolColor = new Color(200, 200, 200);
        Color oLumberColor = new Color(100, 255, 100);
        Color oOreColor = new Color(50, 154, 230);
        Color oGrainColor = new Color(221, 191, 37);
        Color oBrickColor = new Color(231, 50, 50);

        XNACS1Rectangle[] oBoxes = new XNACS1Rectangle[22];

        public TurnRecordBar(int[] nValues, Vector2 oNewDisplacement)
        {
            this.LabelColor = Color.White; 
            this.LabelFont = "TurnMenu";
            this.Texture = "Empty";
            oDisplacement = oNewDisplacement;

            // create boxes
            for (int x = 0; x < oBoxes.Count(); x++)
            {
                oBoxes[x] = new XNACS1Rectangle(new Vector2(), Global.PixelSize(27), Global.PixelSize(27), "TurnRecordBox");
                oBoxes[x].LabelFont = "TurnMenu";          
            }

            // record appearance for empty record 
            for (int x = 0; x < oBoxes.Count(); x++)
            {
                if (nValues == null)
                {
                    oBoxes[x].Label = "-";
                    this.Label = "     ";
                    this.Label = "-    -";
                    this.Label = "                                                   ";    
                }
                else
                    oBoxes[x].Label = "" + nValues[x];
            }

            // make each box correct color
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    oBoxes[y + x * 5].LabelColor = Color.Black; 

                    if (y == 0)
                        oBoxes[y + x * 5].TextureTintColor = oWoolColor;
                    else if (y == 1)
                        oBoxes[y + x * 5].TextureTintColor = oLumberColor;
                    else if (y == 2)
                        oBoxes[y + x * 5].TextureTintColor = oOreColor;
                    else if (y == 3)
                        oBoxes[y + x * 5].TextureTintColor = oGrainColor;
                    else if (y == 4)
                        oBoxes[y + x * 5].TextureTintColor = oBrickColor;

                    if (nValues == null || nValues[y + x * 5] == 0)
                    {
                        oBoxes[y + x * 5].TextureTintColor = oNoneColor;
                        oBoxes[y + x * 5].LabelColor = Color.Black; 
                    }
                }
            }
            oBoxes[20].TextureTintColor = oNoneColor;
            oBoxes[21].TextureTintColor = oNoneColor;
            // assign correct color to boxes
        }

        public void Recenter(Vector2 oCenter)
        {
            this.Center = oCenter + oDisplacement;

            oBoxes[20].Center = new Vector2(this.CenterX - 29.7f, this.CenterY);
            oBoxes[21].Center = new Vector2(this.CenterX - 34.0f, this.CenterY);
            oBoxes[0].Center = new Vector2(this.CenterX - 25.5f, this.CenterY);
            for (int x = 1; x < oBoxes.Count() - 2; x++)
            {
                oBoxes[x].CenterX = oBoxes[x - 1].CenterX + Global.PixelSize(28);
                oBoxes[x].CenterY = oBoxes[x - 1].CenterY;

                if ((x) % 5 == 0)
                    oBoxes[x].CenterX += 1.95f;
            }
        }

        public void Clear()
        {
            foreach (XNACS1Rectangle box in oBoxes)
            {
                box.RemoveFromAutoDrawSet();
            }
        }
    }
}
