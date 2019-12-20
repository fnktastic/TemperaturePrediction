using AutoMapper;
using CoordinateSharp;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
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
                new Point(3500, 3500, 20),
                new Point(3750, 3750, 20),
                new Point(5000, 5000, 20),
            };

            var scenes = await _dataService.GetScenesAsync(scenesPath, points);

            Scenes = new SceneCollection();

            foreach (var scene in scenes)
            {
                var mappedScene = _mapper.Map<Model.Scene>(scene);

                await Task.Run(async () => await Task.Delay(TimeSpan.FromSeconds(rnd.Next(1, 4))));

                Scenes.Add(mappedScene);
            }

            //FillTimeLine();

            //await _dbService.InsertSceneRangeAsync(Scenes.ToList());
        }

        private async void FillTimeLine()
        {
            var anyScene = _scenes.FirstOrDefault();

            for (int i = 0; i < anyScene.Areas.Count; i++)
            {
                TimeLines.Add(i + 1, new TimeLinePointCollection());
            }


            var rnd = new Random();
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
                        Meteo = (await _weatherService.GetPastWeatherForLocationAsync(anyScene.Areas[i].LonLat.ToString(), _startDate)).Now,
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
                    try
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
                    catch
                    {
                        continue;
                    }
                }
            }

            foreach (var timeLine in TimeLines)
            {
                await _dbService.InsertTimeLinePoints(timeLine.Value.ToList());
            }
        }
        #endregion

        #region commands

        private RelayCommand _trainModelCommand;
        public RelayCommand TrainModelCommand => _trainModelCommand ?? (_trainModelCommand = new RelayCommand(TrainModel));
        private async void TrainModel()
        {
            var mapPoints = await _dbService.GetTimeLinePoints();

            //ch 1 meteo
            var predicted = mapPoints.GroupBy(x => x.DateTime).OrderBy(x => x.Key).Select(y => { 
            {
                    var val = y.Average(z => z.Map);
                    var val2 = y.Average(t => t.Meteo);
                    return new DateModel()
                    {
                        DateTime = y.Key,
                        Value = ((val + val2) / 2)
                    };
            }
            }).OrderBy(x => x.DateTime);

            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => (double)dayModel.DateTime.Ticks / TimeSpan.FromDays(_interval).Ticks)
                .Y(dayModel => dayModel.Value);

            Predicted = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    Title = "Predicted, C",
                    PointGeometry = DefaultGeometries.Square,
                    Values = new ChartValues<DateModel>(predicted),
                    Fill = Brushes.Transparent
                }
            };
        }


        private RelayCommand _fetchDataCommand;
        public RelayCommand FetchDataCommand => _fetchDataCommand ?? (_fetchDataCommand = new RelayCommand(FetchData));
        private async void FetchData()
        {
            IsDataFetching = true;
            await FetchScenes();
            IsDataFetching = false;
        }



        public class DateModel
        {
            public System.DateTime DateTime { get; set; }
            public double Value { get; set; }
        }

        private Func<double, string> _formatter;
        public Func<double, string> Formatter
        {
            get { return _formatter; }
            set
            {
                _formatter = value;
                RaisePropertyChanged(nameof(Formatter));
            }
        }

        private SeriesCollection _series;
        public SeriesCollection Series
        {
            get { return _series; }
            set
            {
                _series = value;
                RaisePropertyChanged(nameof(Series));
            }
        }

        private SeriesCollection _series2;
        public SeriesCollection Series2
        {
            get { return _series2; }
            set
            {
                _series2 = value;
                RaisePropertyChanged(nameof(Series2));
            }
        }

        private SeriesCollection _series3;
        public SeriesCollection Series3
        {
            get { return _series3; }
            set
            {
                _series3 = value;
                RaisePropertyChanged(nameof(Series3));
            }
        }
        private SeriesCollection _predicted;
        public SeriesCollection Predicted
        {
            get { return _predicted; }
            set
            {
                _predicted = value;
                RaisePropertyChanged(nameof(Predicted));
            }
        }

        private RelayCommand _showTimeSeriesCommand;
        public RelayCommand ShowTimeSeriesCommand => _showTimeSeriesCommand ?? (_showTimeSeriesCommand = new RelayCommand(ShowTimeSeries));
        private async void ShowTimeSeries()
        {
            var mapPoints = await _dbService.GetTimeLinePoints();

            //ch 1 meteo
            var chanel1Meteo = mapPoints.Where(x => x.Area == 1 && x.Map == 0).Select(x => new DateModel()
            {
                DateTime = x.DateTime,
                Value = x.Meteo
            }).OrderBy(x => x.DateTime);
            //ch 1 map
            var chanel1Map = mapPoints.Where(x => x.Area == 1 && x.Meteo == 0).Select(x => new DateModel()
            {
                DateTime = x.DateTime,
                Value = x.Map
            }).OrderBy(x => x.DateTime);


            //ch 2 meteo
            var chanel2Meteo = mapPoints.Where(x => x.Area == 2 && x.Map == 0).Select(x => new DateModel()
            {
                DateTime = x.DateTime,
                Value = x.Meteo
            }).OrderBy(x => x.DateTime);
            //ch 2 map
            var chanel2Map = mapPoints.Where(x => x.Area == 2 && x.Meteo == 0).Select(x => new DateModel()
            {
                DateTime = x.DateTime,
                Value = x.Map
            }).OrderBy(x => x.DateTime);

            //ch 3 meteo
            var chanel3Meteo = mapPoints.Where(x => x.Area == 3 && x.Map == 0).Select(x => new DateModel()
            {
                DateTime = x.DateTime,
                Value = x.Meteo
            }).OrderBy(x => x.DateTime);
            //ch 3 map
            var chanel3Map = mapPoints.Where(x => x.Area == 3 && x.Meteo == 0).Select(x => new DateModel()
            {
                DateTime = x.DateTime,
                Value = x.Map
            }).OrderBy(x => x.DateTime);

            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => (double)dayModel.DateTime.Ticks / TimeSpan.FromDays(_interval).Ticks)
                .Y(dayModel => dayModel.Value);

            Series = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    Title = "Meteostation, C",
                    PointGeometry = DefaultGeometries.Square,
                    Values = new ChartValues<DateModel>(chanel1Meteo),
                    Fill = Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "Calculated, C",
                    PointGeometry = DefaultGeometries.Cross,
                    Values = new ChartValues<DateModel>(chanel1Map),
                }
            };
            Series2 = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    Title = "Meteostation, C",
                    PointGeometry = DefaultGeometries.Square,
                    Values = new ChartValues<DateModel>(chanel2Meteo),
                    Fill = Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "Calculated, C",
                    PointGeometry = DefaultGeometries.Cross,
                    Values = new ChartValues<DateModel>(chanel2Map),
                }
            };
            Series3 = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    Title = "Meteostation, C",
                    PointGeometry = DefaultGeometries.Square,
                    Values = new ChartValues<DateModel>(chanel3Meteo),
                    Fill = Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "Calculated, C",
                    PointGeometry = DefaultGeometries.Cross,
                    Values = new ChartValues<DateModel>(chanel3Map),
                }
            };
            Formatter = value => new System.DateTime((long)(value * TimeSpan.FromDays(_interval).Ticks)).ToString();
        }
        #endregion
    }
}