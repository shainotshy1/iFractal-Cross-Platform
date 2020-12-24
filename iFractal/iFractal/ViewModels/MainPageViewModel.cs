using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace iFractal.ViewModels
{
    public class MainPageViewModel: BaseViewModel
    {
        public MainPageViewModel()
        {
            XSeed = "X Value:";
            YSeed = "Y Value";

            Image1 = "mandelbrotButtonOff.png";
            Image2 = "juliaButton.png";

            Visibility1 = true;

            Title = "Mandelbrot Options";

            BorderColor1 = "#61F8FA";
            BorderColor2 = "Transparent";

            SwitchButton1 = new Command(() =>
            {
                if(Image1 == "mandelbrotButton.png")
                {
                    SwitchOptions();
                }

                Image1 = "mandelbrotButtonOff.png";
                Image2 = "juliaButton.png";

                BorderColor1 = "#61F8FA";
                BorderColor2 = "Transparent";

                Title = "Mandelbrot Options";

            });
            SwitchButton2 = new Command(() =>
            {
                if (Image1 == "mandelbrotButtonOff.png")
                {
                    SwitchOptions();
                }

                Image1 = "mandelbrotButton.png";
                Image2 = "juliaButtonOff.png";

                BorderColor1 = "Transparent";
                BorderColor2 = "#61F8FA";

                Title = "Julia Set Options";
            });
            Test = new Command(() =>
            {
                Title = "Test";
            });
        }
        private void SwitchOptions()
        {
            double cache1 = CacheXValue;
            double cache2 = CacheYValue;

            CacheXValue = RiValue;
            CacheYValue = CiValue;

            Visibility1 = !Visibility1;
            if (Visibility1)
            {
                XSeed = "X Value:";
                YSeed = "Y Value";

                Min = 0;

                if (cache1 == 0.0000000000001)
                {
                    cache1 = 0;
                }
                if (cache2 == 0.0000000000001)
                {
                    cache2 = 0;
                }
            }
            else
            {
                XSeed = "X Seed:";
                YSeed = "Y Seed";

                Min = -1;

                if (cache1 == 0)
                {
                    cache1 = 0.0000000000001;
                }
                if (cache2 == 0)
                {
                    cache2 = 0.0000000000001;
                }
            }



            RiValue = cache1;
            CiValue = cache2;

            UpdateX = Convert.ToString(cache1);
            UpdateY = Convert.ToString(cache2);
        }
        public Command SwitchButton1 { get; }
        public Command SwitchButton2 { get; }
        public Command Test { get; }
    }
}
