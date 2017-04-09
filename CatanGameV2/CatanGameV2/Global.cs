
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace CatanGameV2
{
    public class Global
    {
        public static int nWindowPixelWidth = 1000; 
        public static int nWindowPixelHeight = 700; 

        public static float PixelSize(int nPixelSize)
        {
            return XNACS1Base.World.WorldDimension.X / nWindowPixelWidth * nPixelSize;
        }

        public static float MouseXToWorldDimension(int nX)
        {
            return ((float)nX / (float)nWindowPixelWidth) * XNACS1Base.World.WorldDimension.X - (XNACS1Base.World.WorldDimension.X / 2);
        }

        public static float MouseYToWorldDimension(int nY)
        {
            return -(((float)nY / (float)nWindowPixelHeight) * XNACS1Base.World.WorldDimension.Y - (XNACS1Base.World.WorldDimension.Y / 2));
        }
    }
}
