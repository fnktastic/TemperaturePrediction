using AutoMapper;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using TemperaturePrediction.UI.Helper;
using TemperaturePrediction.UI.Service;

namespace TemperaturePrediction.UI.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();

            SimpleIoc.Default.Register<IWeatherService, WeatherService>();

            SimpleIoc.Default.Register<IDataService, DataService>();

            SimpleIoc.Default.Register<IConfigurationProvider, MyConfig>();

            SimpleIoc.Default.Register<IMapper, MyMapper>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}