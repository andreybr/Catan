using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using XNACS1Lib;

namespace CatanGameV2
{
    public class Game1 : XNACS1Base
    {
        public enum MENU_STATE { START, CHARACTER, BASIC_GAME }; 
        public MENU_STATE oMenuState = MENU_STATE.START;

        KeyboardState oKeyState = new KeyboardState();

        StartMenu oStartMenu;
        BasicGame oBasicGame;
        Cursor oCursor;
        Camera oCamera;

        protected override void InitializeWorld()
        {
            SetAppWindowPixelDimension(false, Global.nWindowPixelWidth, Global.nWindowPixelHeight);
            //XNACS1Base.World.SetBackground(Color.Wheat);
            IsMouseVisible = true;

            oCursor = new Cursor();
            oCamera = new Camera();
        }

        protected override void UpdateWorld()
        {
            oKeyState = Keyboard.GetState();

            //XNACS1Base.SetBottomEchoColor(Color.Red);
            //XNACS1Base.EchoToBottomStatus("HUD:" + oHexes[0,0].o);

            oCamera.Update(oCursor);
            oCursor.Update(oCamera);

            #region Exit Game (F1)
            if (oKeyState.IsKeyDown(Keys.F1))
            {
                this.Exit();
            }
            #endregion

            #region Update Current Menu
            if (oMenuState == MENU_STATE.START)
            {
                if (oStartMenu == null)
                    oStartMenu = new StartMenu();
                else
                {
                    oMenuState = oStartMenu.Update(oCursor, oCamera);

                    if (oMenuState != MENU_STATE.START)
                        oStartMenu = null;
                }
            }
            else if (oMenuState == MENU_STATE.BASIC_GAME)
            {
                if (oBasicGame == null)
                    oBasicGame = new BasicGame(oCamera);
                else
                {
                    oMenuState = oBasicGame.Update(oCursor, oCamera);

                    if (oMenuState != MENU_STATE.BASIC_GAME)
                        oBasicGame = null;
                }
            }
            #endregion
        }
    }
}
