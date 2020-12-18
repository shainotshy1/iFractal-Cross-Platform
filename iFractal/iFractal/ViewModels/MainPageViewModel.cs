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
