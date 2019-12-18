using GalaSoft.MvvmLight;
using System;
using TemperaturePrediction.UI.Service;

namespace TemperaturePrediction.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IWeatherService weatherService)
        {
            weatherService.GetPastWeatherForLocationAsync("44.24593, -98.52156", new DateTime(2019,10,17));
        }
    }
}