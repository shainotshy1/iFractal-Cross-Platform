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
            
            Visibility1 = true;

            SwitchOptions = new Command(() =>
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

            });
        }

        public Command SwitchOptions { get; }
    }
}
