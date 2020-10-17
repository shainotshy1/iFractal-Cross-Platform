using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaSharpFormsDemos
{
    public class Pixel
    {
        private int red;
        private int green;
        private int blue;
        
        public Pixel(int r, int g, int b)
        {
            red = r;
            green = g;
            blue = b;
        }

        public void SetRed(int a)
        {
            red = a;
        }
        public void SetGreen(int a)
        {
            green = a;
        }
        public void SetBlue(int a)
        {
            blue = a;
        }

        public int GetRed()
        {
            return red;
        }

        public int GetGreen()
        {
            return green;
        }

        public int GetBlue()
        {
            return blue;
        }
        public int GetColorInt()
        {
            return red * 16 * 16 * 16 * 16 + green * 16 * 16 + blue; // to put into base 16
        }
    }
}
