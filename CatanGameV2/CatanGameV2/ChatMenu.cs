using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    class ChatMenu
    {
        XNACS1Rectangle oOutputBox;
        float fOutputBoxWidth = Global.PixelSize(220);
        float fOutputBoxHeight = Global.PixelSize(280);
        Vector2 oOutputBoxDisplacement = new Vector2(11.41f, 29.21f);

        XNACS1Rectangle oInputBox;
        float fInputBoxWidth = Global.PixelSize(220);
        float fInputBoxHeight = Global.PixelSize(30);
        Vector2 oInputBoxDisplacement = new Vector2(0.01f, -15.6f);
        string sCurrentMessage = "";

        List<XNACS1Rectangle> oMessages = new List<XNACS1Rectangle>();
        Vector2 oNewestMessageDisplacement = new Vector2(0f, 12f);
        Vector2 oDisplacementBetweenMessages = new Vector2(0f, Global.PixelSize(16));
        float fSpacePerLetter = Global.PixelSize(7);

        bool bSelected = false;

        public ChatMenu(Camera oCamera)
        {
            oOutputBox = new XNACS1Rectangle(new Vector2(), fOutputBoxWidth, fOutputBoxHeight);
            oOutputBox.Color = new Color(0, 0, 0, 200);
            
            oInputBox = new XNACS1Rectangle(new Vector2(), fInputBoxWidth, fInputBoxHeight);
            oInputBox.Color = new Color(0, 0, 0, 200);
            oInputBox.LabelFont = "ChatMenu";
            oInputBox.LabelColor = Color.White;

            PositionWithCamera(oCamera);
            PositionMessagesWithBox();
        }

        public void Update(Camera oCamera, Cursor oCursor, Player oPlayer)
        {
            //XNACS1Base.EchoToBottomStatus("Chat:" + bSelected);

            #region Type In Chat Box
            if (bSelected == true)
            {
                KeyboardState currentKeyboardState = Keyboard.GetState();
                Keys[] pressedKeys = currentKeyboardState.GetPressedKeys();

                foreach (Keys key in pressedKeys)
                {
                    if (key == Keys.Back)
                    {
                        XNACS1Base.EchoToTopStatus("Chat:" + bSelected);

                        if (sCurrentMessage.Length > 0)
                        {
                            sCurrentMessage = sCurrentMessage.Remove(sCurrentMessage.Length - 1);
                        }
                    }
                    else if (key == Keys.Space)
                    {
                        sCurrentMessage += " ";
                    }
                    else if (key == Keys.Enter)
                    {
                        if (sCurrentMessage.Length > 0)
                        {
                            AddMessage(sCurrentMessage, oPlayer.sName);
                            PositionMessagesWithBox();
                            sCurrentMessage = "";
                        }
                    }
                    else
                        sCurrentMessage += key.ToString();
                }

                oInputBox.Label = sCurrentMessage;
            }
            #endregion

            #region Select Chat Box
            if (oCursor.LeftClick())
            {
                if(oCursor.oHitbox.Collided(oInputBox))
                    bSelected = true;
                else
                    bSelected = false;
            }
            #endregion

            if (oCamera.bHasMoved == true)
            {
                PositionWithCamera(oCamera);
                PositionMessagesWithBox();
            }
        }

        public void AddMessage(string sMessage, string sPlayerName)
        {
            oMessages.Add(new XNACS1Rectangle());
            oMessages[oMessages.Count - 1].Color = new Color(0, 0, 0, 0);
            oMessages[oMessages.Count - 1].Label = sPlayerName + ": " + sMessage;
            oMessages[oMessages.Count - 1].LabelFont = "ChatMenu";
            oMessages[oMessages.Count - 1].LabelColor = Color.White;
        }

        public void PositionWithCamera(Camera oCamera)
        {
            oOutputBox.Center = oCamera.oCenter + oOutputBoxDisplacement;
            oInputBox.Center = oOutputBox.Center + oInputBoxDisplacement;
        }

        public void PositionMessagesWithBox()
        {
            for (int x = 0; x < oMessages.Count(); x++)
            {
                oMessages[x].CenterY = 17f + oDisplacementBetweenMessages.Y * x + 0.01f;
                oMessages[x].CenterX = 1.3f + (fSpacePerLetter * oMessages[x].Label.Length) / 2f;
            }
        }
    }
}
