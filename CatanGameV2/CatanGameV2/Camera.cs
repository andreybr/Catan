using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class Camera
    {
        public Vector2 oCenter = new Vector2(0, 0);
        int nZoom = 100;
        float fCursorSpeed = 2f;
        int nZoomRate = 4;
        int nLastScrollWheelValue = 0;

        MouseState oMouseState;
        KeyboardState oKeyState;

        Vector2 oLastMouseLocation = new Vector2();
        Vector2 oTemp = new Vector2();

        public float fLeftBoundary = -17f;
        public float fRightBoundary = 17f;
        public float fBottomBoundary = -10f;
        public float fTopBoundary = 10f;

        public bool bAllowMovement = false;
        public bool bHasMoved = false;

        public Camera()
        {

        }

        public void Update(Cursor oCursor)
        {
            //XNACS1Base.SetBottomEchoColor(Color.Red);
            //XNACS1Base.EchoToBottomStatus("camera" + oCenter);

            oMouseState = Mouse.GetState();
            oKeyState = Keyboard.GetState();

            if (bAllowMovement == true)
            {
                //XNACS1Base.EchoToBottomStatus("Camera:" + bHasMoved);

                bHasMoved = false;

                //#region Move Camera (WASD)
                //if (oKeyState.IsKeyDown(Keys.A) && oCenter.X > fLeftBoundary)
                //{
                //    oCenter.X -= fCursorSpeed;
                //    bHasMoved = true;
                //}
                //else if (oKeyState.IsKeyDown(Keys.D) && oCenter.X < fRightBoundary)
                //{
                //    oCenter.X += fCursorSpeed;
                //    bHasMoved = true;
                //}

                //if (oKeyState.IsKeyDown(Keys.W) && oCenter.Y < fTopBoundary)
                //{
                //    oCenter.Y += fCursorSpeed;
                //    bHasMoved = true;
                //}
                //else if (oKeyState.IsKeyDown(Keys.S) && oCenter.Y > fBottomBoundary)
                //{
                //    oCenter.Y -= fCursorSpeed;
                //    bHasMoved = true;
                //}

                //if (oCenter != new Vector2() && oKeyState.IsKeyDown(Keys.Space))
                //{
                //    oCenter = new Vector2();
                //    bHasMoved = true;
                //}
                //#endregion

                #region Move Camera (Mouse)
                if (oCursor.RightClick())
                {
                    oCursor.oDragPosition = oCursor.oHitbox.Center;
                }
                else if (oCursor.IsRightMouseDown())
                {
                    if (oCursor.oHitbox.Center != oCursor.oDragPosition)
                    {
                        oCenter -= oCursor.oHitbox.Center - oCursor.oDragPosition;
                        XNACS1Base.World.SetWorldCoordinate(oCenter, nZoom);
                        oCursor.CalculatePosition(this);
                        oCursor.oDragPosition = oCursor.oHitbox.Center;
                        bHasMoved = true;
                    }
                }
                #endregion
            }

            XNACS1Base.World.SetWorldCoordinate(oCenter, nZoom);
        }

        public void Recenter()
        {
            oCenter = new Vector2();
        }
    }
}
