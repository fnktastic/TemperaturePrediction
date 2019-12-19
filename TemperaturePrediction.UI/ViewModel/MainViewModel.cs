using AutoMapper;
using CoordinateSharp;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TemperaturePrediction.UI.Collection;
using TemperaturePrediction.UI.Service;

namespace TemperaturePrediction.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IDataService _dataService;
        private readonly IMapper _mapper;

        private SceneCollection _scenes;
        public SceneCollection Scenes
        {
            get { return _scenes; }
            set
            {
                _scenes = value;
                RaisePropertyChanged(nameof(Scenes));
            }
        }

        public MainViewModel(IWeatherService weatherService, IDataService dataService, IMapper mapper)
        {
            _weatherService = weatherService;
            _dataService = dataService;
            _mapper = mapper;
            //weatherService.GetPastWeatherForLocationAsync("44.24593, -98.52156", new DateTime(2019,10,17));

            FetchData();
        }

        private async void FetchData()
        {
            string path = @"C:\Users\fnkta\Documents\Scenes";

            var points = new List<Point>()
            {
                new Point(3000, 3000, 2),
                new Point(2000, 4000, 2),
                new Point(4000, 2000, 2),
            };

            var scenes = _dataService.GetScenes(path, points);

            foreach (var scene in scenes)
            {
                foreach (var area in scene.Areas)
                {
                    var meteo = await _weatherService.GetPastWeatherForLocationAsync(area.LonLat.ToString(), scene.TimeStamp);

                    area.Meteo = meteo;
                }
            }

            Scenes = new SceneCollection(_mapper.Map<List<Model.Scene>>(scenes));
        }
    }
}