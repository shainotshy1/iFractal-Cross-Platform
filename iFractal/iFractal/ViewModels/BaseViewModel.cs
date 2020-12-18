using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using iFractal.Services;
using iFractal.Models;

namespace iFractal.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        bool visibility1 = true;
        public bool Visibility1
        {
            get { return visibility1; }
            set { SetProperty(ref visibility1, value); }
        }

        double riValue = 0;
        public double RiValue
        {
            get { return riValue; }
            set { SetProperty(ref riValue, value); }
        }

        double ciValue = 0;
        public double CiValue
        {
            get { return ciValue; }
            set { SetProperty(ref ciValue, value); }
        }

        double cachexValue = 0;
        public double CacheXValue
        {
            get { return cachexValue; }
            set { SetProperty(ref cachexValue, value); }
        }

        double cacheyValue = 0;
        public double CacheYValue
        {
            get { return cacheyValue; }
            set { SetProperty(ref cacheyValue, value); }
        }

        double min = 0;
        public double Min
        {
            get { return min; }
            set { SetProperty(ref min, value); }
        }

        string updateX = string.Empty;
        public string UpdateX
        {
            get { return updateX; }
            set { SetProperty(ref updateX, value); }
        }

        string updateY = string.Empty;
        public string UpdateY
        {
            get { return updateY; }
            set { SetProperty(ref updateY, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
