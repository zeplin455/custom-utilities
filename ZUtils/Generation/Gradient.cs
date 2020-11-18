using System;
using System.Collections.Generic;
using System.Text;

namespace ZUtils.Generation
{
    public class Gradient
    {
        public static double LinearGradientRight(double x, double y, double blocksize)
        {
            double posX = (x / blocksize) - Math.Floor(x / blocksize);
            return posX;
        }
        public static double LinearGradientLeft(double x, double y, double blocksize)
        {
            return 1 - LinearGradientRight(x, y, blocksize);
        }
        public static double LinearGradientDown(double x, double y, double blocksize)
        {
            double posY = (y / blocksize) - Math.Floor(y / blocksize);
            return posY;
        }
        public static double LinearGradientUp(double x, double y, double blocksize)
        {
            return 1 - LinearGradientDown(x, y, blocksize);
        }

        public static double LinearGradientUpRight(double x, double y, double blocksize)
        {
            double posX = (x / blocksize) - Math.Floor(x / blocksize);
            double posY = (y / blocksize) - Math.Floor(y / blocksize);
            return (((1 - posY) + posX) / 2);
        }

        public static double LinearGradientUpLeft(double x, double y, double blocksize)
        {
            double posX = (x / blocksize) - Math.Floor(x / blocksize);
            double posY = (y / blocksize) - Math.Floor(y / blocksize);
            return (((1-posY) + (1-posX)) / 2);
        }

        public static double LinearGradientDownRight(double x, double y, double blocksize)
        {
            double posX = (x / blocksize) - Math.Floor(x / blocksize);
            double posY = (y / blocksize) - Math.Floor(y / blocksize);
            return (posY + posX) / 2;
        }

        public static double LinearGradientDownLeft(double x, double y, double blocksize)
        {
            double posX = (x / blocksize) - Math.Floor(x / blocksize);
            double posY = (y / blocksize) - Math.Floor(y / blocksize);
            return (posY + (1-posX))/2;
        }

        public static double LinearPyramidGradient(double x, double y, double blocksize)
        {
            double xProgress = (x / blocksize) - Math.Floor(x / blocksize);
            double xVal = 0;
            
            if(xProgress >= 0.5d)
            {
                xVal = 1 - ((x / (blocksize/2)) - Math.Floor(x / (blocksize/2)));
            }
            else
            {
                xVal = x / (blocksize / 2) - Math.Floor(x / (blocksize / 2));
            }

            double yProgress = (y / blocksize) - Math.Floor(y / blocksize);
            double yVal = 0;

            if (yProgress >= 0.5d)
            {
                yVal = 1 - ((y / (blocksize / 2)) - Math.Floor(y / (blocksize / 2)));
            }
            else
            {
                yVal = y / (blocksize / 2) - Math.Floor(y / (blocksize / 2));
            }

            if(xVal < yVal)
            {
                return xVal;
            }
            else
            {
                return yVal;
            }
        }

        public static double LinearPyramidGradientInvert(double x, double y, double blocksize)
        {
            return 1 - LinearPyramidGradient(x, y, blocksize);
        }

    }
}
