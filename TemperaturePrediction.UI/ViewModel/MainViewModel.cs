using AutoMapper;
using CoordinateSharp;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TemperaturePrediction.Model;
using TemperaturePrediction.UI.Collection;
using TemperaturePrediction.UI.Model;
using TemperaturePrediction.UI.Service;
using Point = TemperaturePrediction.UI.Service.Point;

namespace TemperaturePrediction.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region fields
        private readonly IWeatherService _weatherService;
        private readonly IDataService _dataService;
        private readonly IDbService _dbService;
        private readonly IMapper _mapper;
        #endregion

        #region properties
        private Dictionary<int, TimeLinePointCollection> _timeLines;
        public Dictionary<int, TimeLinePointCollection> TimeLines
        {
            get { return _timeLines; }
            set
            {
                _timeLines = value;
                RaisePropertyChanged(nameof(TimeLines));
            }
        }

        private TimeLinePointCollection _timeLinePointCollection;
        public TimeLinePointCollection TimeLinePointCollection
        {
            get { return _timeLinePointCollection; }
            set
            {
                _timeLinePointCollection = value;
                RaisePropertyChanged(nameof(TimeLinePointCollection));
            }
        }

        private int _interval;
        public int Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                RaisePropertyChanged(nameof(Interval));
            }
        }

        private int _minCloudity;
        public int MinCloudity
        {
            get { return _minCloudity; }
            set
            {
                _minCloudity = value;
                RaisePropertyChanged(nameof(MinCloudity));
            }
        }

        private int _maxCloudity;
        public int MaxCloudity
        {
            get { return _maxCloudity; }
            set
            {
                _maxCloudity = value;
                RaisePropertyChanged(nameof(MaxCloudity));
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                RaisePropertyChanged(nameof(StartDate));
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                RaisePropertyChanged(nameof(EndDate));
            }
        }

        private double _lat;
        public double Lat
        {
            get { return _lat; }
            set
            {
                _lat = value;
                RaisePropertyChanged(nameof(Lat));
            }
        }

        private double _lon;
        public double Lon
        {
            get { return _lon; }
            set
            {
                _lon = value;
                RaisePropertyChanged(nameof(Lon));
            }
        }

        private Scene _selectedScene;
        public Scene SelectedScene
        {
            get { return _selectedScene; }
            set
            {
                _selectedScene = value;
                RaisePropertyChanged(nameof(SelectedScene));
            }
        }

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

        private bool _isDataFetching;
        public bool IsDataFetching
        {
            get { return _isDataFetching; }
            set
            {
                if (value == _isDataFetching) return;
                _isDataFetching = value;

                RaisePropertyChanged(nameof(IsDataFetching));
            }
        }
        #endregion

        public MainViewModel(IWeatherService weatherService, IDataService dataService, IMapper mapper, IDbService dbService)
        {
            _weatherService = weatherService;
            _dataService = dataService;
            _mapper = mapper;

            Lat = -97.32156;
            Lon = 45.54593;

            StartDate = new DateTime(2019, 10, 01);
            EndDate = DateTime.Now;

            MaxCloudity = 50;
            MinCloudity = 0;

            Interval = 14;

            _dbService = dbService;

            TimeLinePointCollection = new TimeLinePointCollection();
            TimeLines = new Dictionary<int, TimeLinePointCollection>();
        }

        #region private methods
        List<Point> points = null;
        private async Task FetchScenes()
        {
            var rnd = new Random();

            string scenesPath = @"C:\Users\fnkta\Documents\Scenes";

            points = new List<Point>()
            {
                new Point(3500, 3500, 2),
                new Point(3750, 3750, 2),
                new Point(5000, 5000, 2),
            };

            var scenes = await _dataService.GetScenesAsync(scenesPath, points);

            Scenes = new SceneCollection();

            foreach (var scene in scenes)
            {
                var mappedScene = _mapper.Map<Model.Scene>(scene);

                await Task.Run(async () => await Task.Delay(TimeSpan.FromSeconds(rnd.Next(1, 4))));

                Scenes.Add(mappedScene);
            }

            FillTimeLine();
            //await _dbService.InsertSceneRangeAsync(Scenes.ToList());
        }

        private async void FillTimeLine()
        {
            var anyScene = _scenes.FirstOrDefault();

            for(int i = 0; i < anyScene.Areas.Count; i++)
            {
                TimeLines.Add(i+1, new TimeLinePointCollection());
            }
            

            while (_startDate < _endDate)
            {
                for (int i = 0; i < anyScene.Areas.Count; i++)
                {
                    var timeLinePoint = new TimeLinePoint()
                    {
                        Area = anyScene.Areas[i].Number,
                        DateTime = _startDate,
                        Lon = anyScene.Areas[i].LonLat.Lon,
                        Lat = anyScene.Areas[i].LonLat.Lat,
                        Meteo = (await _weatherService.GetPastWeatherForLocationAsync(anyScene.Areas[i].LonLat.ToString(), _startDate)).AVG,
                        Map = 0
                    };

                    TimeLines[timeLinePoint.Area].Add(timeLinePoint);
                }

                _startDate = _startDate.AddDays(_interval);
            }

            foreach (var scene in Scenes)
            {
                for (int i = 0; i < scene.Areas.Count; i++)
                {
                    var timeLinePoint = new TimeLinePoint()
                    {
                        Area = scene.Areas[i].Number,
                        DateTime = scene.TimeStamp,
                        Lat = scene.Areas[i].LonLat.Lat,
                        Lon = scene.Areas[i].LonLat.Lon,
                        Meteo = 0,
                        Map = scene.Areas[i].UnitPoints.Select(x => x.PictureTemperature).Average()
                    };

                    TimeLines[timeLinePoint.Area].Add(timeLinePoint);
                }
            }
        }
        #endregion

        #region commands
        private RelayCommand _fetchDataCommand;
        public RelayCommand FetchDataCommand => _fetchDataCommand ?? (_fetchDataCommand = new RelayCommand(FetchData));
        private async void FetchData()
        {
            IsDataFetching = true;
            await FetchScenes();
            IsDataFetching = false;
        }
        #endregion
    }
}