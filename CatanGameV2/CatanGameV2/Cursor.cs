using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class Cursor
    {
        private MouseState oMouseState; // the mouse pixel position

        public XNACS1Circle oHitbox;
        private const float fCursorRadius = 0.2f;

        private Vector2 oPointingVector; // mouse position in world coordinates

        private bool bResetLeft = true;
        private bool bResetRight = true;
        private ButtonState oLastStateLeft;
        private ButtonState oLastStateRight;

        public Vector2 oDragPosition = new Vector2();

        public Cursor()
        {
            oHitbox = new XNACS1Circle(new Vector2(), fCursorRadius);
            oHitbox.Color = Color.Red;
            //oHitbox.Visible = false;
        }

        public void Update(Camera oCamera)
        {
            //XNACS1Base.SetBottomEchoColor(Color.Red);
            //XNACS1Base.EchoToBottomStatus("Mouse:" + oHitbox.Center);

            #region Get Mouse Pixel Position
            oMouseState = Mouse.GetState();
            #endregion

            CalculatePosition(oCamera);

            #region Reset Left Mouse Click
            if (oMouseState.LeftButton != oLastStateLeft && oMouseState.LeftButton == ButtonState.Pressed)
                bResetLeft = true;
            else
                bResetLeft = false;

            oLastStateLeft = oMouseState.LeftButton;
            #endregion

            #region Reset Right Mouse Click
            if (oMouseState.RightButton != oLastStateRight && oMouseState.RightButton == ButtonState.Pressed)
                bResetRight = true;
            else
                bResetRight = false;

            oLastStateRight = oMouseState.RightButton;
            #endregion
        }

        public void CalculatePosition(Camera oCamera)
        {
            oPointingVector = new Vector2((oMouseState.X - (float)(Global.nWindowPixelWidth / 2)) / (float)(Global.nWindowPixelWidth / 2) * 50f,
                                         -(oMouseState.Y - (float)(Global.nWindowPixelHeight / 2)) / (float)(Global.nWindowPixelHeight / 2) * 35f);
            oHitbox.Center = oPointingVector;
            oHitbox.CenterX += XNACS1Base.World.WorldDimension.X / 2;
            oHitbox.CenterY += XNACS1Base.World.WorldDimension.Y / 2;

            oHitbox.Center += oCamera.oCenter;
        }

        public bool IsLeftMouseDown()
        {
            if (oMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public bool IsRightMouseDown()
        {
            if (oMouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public bool LeftClick()
        {
            if (oMouseState.LeftButton == ButtonState.Pressed && bResetLeft == true)
                return true;
            else
                return false;
        }

        public bool RightClick()
        {
            if (oMouseState.RightButton == ButtonState.Pressed && bResetRight == true)
                return true;
            else
                return false;
        }
    }
}
