using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class Hex : XNACS1Rectangle
    {
        //static public float fWidth = Global.PixelSize(112); // width of image
        //static public float fHeight = Global.PixelSize(92); // height of image
        static public float fWidth = Global.PixelSize(132); // width of image
        static public float fHeight = Global.PixelSize(100); // height of image
        public int nNumber; // roll needed to get resources from this hex
        public string sType; // resource type
        public bool bRobber = false;
        
        public XNACS1Rectangle oPort;
        public Vector2[] oPortDisplacements = new Vector2[6]; // old
        public Vector2 oPortDisplacement = new Vector2(0f, Global.PixelSize(-2));

        public Intersection[] oIntersections = new Intersection[6];

        public XNACS1Rectangle oPortSymbol;
        public float fPortSymbolWidth = Global.PixelSize(31);
        public float fPortSymbolHeight = Global.PixelSize(32);
        public Vector2[] oPortSymbolDisplacements = new Vector2[6];

        public XNACS1Rectangle oNumber;
        Vector2 oNumberOffset = new Vector2(0, 0.6f);

        // test
        XNACS1Rectangle otest;
        Vector2 otestvelocity = new Vector2(0f, 0.1f);
        int otesttimer = 0;
        int oteststarttimer = 30;

        // rename these
        bool blink = false;
        int nblinktimer = 0;
        int nblinktimerstart = 40;
        Color blinkcolor = new Color(170, 170, 200);

        public Hex(Vector2 oCenter, int nNumber, string sType, string sLabel = null) 
        {
            this.Center = oCenter + new Vector2(0f, 10f);
            this.nNumber = nNumber;
            this.Width = fWidth;
            this.Height = fHeight;
            this.sType = sType;

            otest = new XNACS1Rectangle(this.Center, 3f, 2f);
            otest.Color = new Color(0, 0, 0, 170);
            otest.LabelFont = "SmallHUD";
            otest.LabelColor = Color.Red;
            otest.Visible = false;

            oNumber = new XNACS1Rectangle(this.Center + oNumberOffset, Global.PixelSize(28), Global.PixelSize(25));
            oNumber.SetTextureSpriteSheet("ResourceNumbers2", 11, 1, 0);
            oNumber.UseSpriteSheet = true;
            oNumber.SetTextureSpriteAnimationFrames(nNumber - 2, 0, nNumber - 2, 0, 1, SpriteSheetAnimationMode.AnimateForward);

            oPortDisplacements[0] = new Vector2(0f, 5.0f);
            oPortDisplacements[1] = new Vector2(-6.1f, 3.45f);
            oPortDisplacements[2] = new Vector2(-6.1f, -3.45f);
            oPortDisplacements[3] = new Vector2(0f, -6.2f);
            oPortDisplacements[4] = new Vector2(6.1f, -3.45f);
            oPortDisplacements[5] = new Vector2(6.1f, 3.45f);

            oPortSymbolDisplacements[0] = new Vector2(0f, 10f);
            oPortSymbolDisplacements[1] = new Vector2(-10f, 5f);
            oPortSymbolDisplacements[2] = new Vector2(-10f, -5f);
            oPortSymbolDisplacements[3] = new Vector2(0f, -10f);
            oPortSymbolDisplacements[4] = new Vector2(10f, -5f);
            oPortSymbolDisplacements[5] = new Vector2(10f, 5f);

            switch (sType)
            {
                case "wool":
                    this.Texture = "WoolHex2";
                    break;
                case "lumber":
                    this.Texture = "LumberHex2";
                    break;
                case "ore":
                    this.Texture = "OreHex2";
                    break;
                case "grain":
                    this.Texture = "GrainHex2";
                    break;
                case "brick":
                    this.Texture = "BrickHex2";
                    break;
                case "empty":
                    this.Texture = "OreHex2";
                    this.TextureTintColor = new Color(0, 0, 0, 256);
                    break;
            }
        }

        public void CreateIntersections()
        {
            for (int x = 0; x < oIntersections.Count(); x++)
                oIntersections[x].Create(this.Center, this.Label);
        }

        public void Update(Cursor oCursor, Camera oCamera, Stats oStats, HUD oHUD)
        { 
            for (int x = 0; x < oIntersections.Count(); x++)
                oIntersections[x].Update(oCursor, oCamera, x, oIntersections, oStats, oHUD, oPort);

            // test effect, currently unused
            #region Show Gained Amount
            if (otest.Visible == true)
            {
                otesttimer--;

                if (otesttimer == 0)
                {
                    otest.Velocity = new Vector2();
                    otest.Visible = false;
                    otest.ShouldTravel = false;
                }
            }
            #endregion

            #region blink effect
            if(blink == true)
            {
                nblinktimer--;

                if(nblinktimer > 0)
                {
                    if(nblinktimer % 6 > 2)
                    {
                        this.TextureTintColor = blinkcolor;
                    } 
                    else
                    {
                        this.TextureTintColor = new Color(255, 255, 255);
                    }
                }
                else
                {
                    blink = false;
                }
            }
            #endregion
        }

        public void GiveResources(int nRoll, Stats oStats, int[] nResourceRecord)
        {
            if(bRobber == false)
            {
                for (int x = 0; x < oIntersections.Count(); x++)
                {
                    if (oIntersections[x].nOwner > -1)
                    {
                        if (nRoll == nNumber)
                        {
                            int nReward = 0;

                            if (oIntersections[x].eState == Intersection.STATE.SETTLEMENT)
                                nReward = 1;
                            else if (oIntersections[x].eState == Intersection.STATE.CITY)
                                nReward = 2;

                            oStats.nTotalResourcesGained[oIntersections[x].nOwner] += nReward;

                            //otest.ShouldTravel = true;
                            //otest.Label = "+" + nReward;
                            //otest.Visible = true;
                            //otest.Velocity = otestvelocity;
                            //otesttimer = oteststarttimer;

                            blink = true;
                            nblinktimer = nblinktimerstart;

                            switch (sType)
                            {
                                case "wool":
                                    oStats.oPlayers[oIntersections[x].nOwner].nCards[0] += nReward;
                                    nResourceRecord[(oIntersections[x].nOwner) * 5] += nReward;
                                    oStats.AddResourceRecord(0, nReward);
                                    break;
                                case "lumber":
                                    oStats.oPlayers[oIntersections[x].nOwner].nCards[1] += nReward;
                                    nResourceRecord[(oIntersections[x].nOwner) * 5 + 1] += nReward;
                                    oStats.AddResourceRecord(1, nReward);
                                    break;
                                case "ore":
                                    oStats.oPlayers[oIntersections[x].nOwner].nCards[2] += nReward;
                                    nResourceRecord[(oIntersections[x].nOwner) * 5 + 2] += nReward;
                                    oStats.AddResourceRecord(2, nReward);
                                    break;
                                case "grain":
                                    oStats.oPlayers[oIntersections[x].nOwner].nCards[3] += nReward;
                                    nResourceRecord[(oIntersections[x].nOwner) * 5 + 3] += nReward;
                                    oStats.AddResourceRecord(3, nReward);
                                    break;
                                case "brick":
                                    oStats.oPlayers[oIntersections[x].nOwner].nCards[4] += nReward;
                                    nResourceRecord[(oIntersections[x].nOwner) * 5 + 4] += nReward;
                                    oStats.AddResourceRecord(4, nReward);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void PositionPorts()
        {
            int nPosition = -1;
            int nPortType = -1;

            // look for port
            for (int x = 0; x < oIntersections.Count(); x++)
            {
                if (oIntersections[x].nPortType == -1)
                    continue;

                if (x == 5)
                {
                    if (oIntersections[5].nPortType > -1 && oIntersections[0].nPortType > -1)
                    {
                        nPosition = 5;
                        nPortType = oIntersections[x].nPortType;
                    }
                }
                else
                {
                    if (oIntersections[x].nPortType > -1 && oIntersections[x + 1].nPortType > -1)
                    {
                        nPosition = x;
                        nPortType = oIntersections[x].nPortType;
                    }
                }
            }

            // port found
            if (nPosition > -1)
            {
                //oPort = new XNACS1Rectangle(oPortDisplacements[nPosition] + this.Center, Global.PixelSize(72), Global.PixelSize(72));
                oPort = new XNACS1Rectangle(this.Center + oPortDisplacement, Global.PixelSize(190), Global.PixelSize(170));
                //oPort.Label = "" + nPortType;
                oPort.UseSpriteSheet = true;
                oPort.SetTextureSpriteSheet("Ports2", 6, 1, 0);
                oPort.SetTextureSpriteAnimationFrames(nPosition, 0, nPosition, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                oPort.Visible = true;

                oPortSymbol = new XNACS1Rectangle(this.Center + oPortSymbolDisplacements[nPosition], fPortSymbolWidth, fPortSymbolHeight);
                oPortSymbol.UseSpriteSheet = true;
                oPortSymbol.SetTextureSpriteSheet("PortSymbols", 6, 1, 0);
                oPortSymbol.SetTextureSpriteAnimationFrames(nPortType, 0, nPortType, 0, 1, SpriteSheetAnimationMode.AnimateForward);
                oPortSymbol.Visible = true;
            }   
        }
    }
}
