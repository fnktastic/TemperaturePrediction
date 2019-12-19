using AutoMapper;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using TemperaturePrediction.DataAccess;
using TemperaturePrediction.Repository;
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

            SimpleIoc.Default.Register<Context>();

            SimpleIoc.Default.Register<ISceneRepository, SceneRepository>();

            SimpleIoc.Default.Register<IDbService, DbService>();

            SimpleIoc.Default.Register<ITimelineRepository, TimelineRepository>();
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