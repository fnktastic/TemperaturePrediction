using AutoMapper;
using CoordinateSharp;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TemperaturePrediction.UI.Collection;
using TemperaturePrediction.UI.Service;

namespace TemperaturePrediction.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region fields
        private readonly IWeatherService _weatherService;
        private readonly IDataService _dataService;
        private readonly IMapper _mapper;
        #endregion

        #region properties
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

        public MainViewModel(IWeatherService weatherService, IDataService dataService, IMapper mapper)
        {
            _weatherService = weatherService;
            _dataService = dataService;
            _mapper = mapper;
        }

        #region private methods
        private async Task FetchScenes()
        {
            string scenesPath = @"C:\Users\fnkta\Documents\Scenes";

            var points = new List<Point>()
            {
                new Point(3000, 3000, 2),
                new Point(2000, 4000, 2),
                new Point(4000, 2000, 2),
            };

            var scenes = await _dataService.GetScenesAsync(scenesPath, points);

            Scenes = new SceneCollection(_mapper.Map<List<Model.Scene>>(scenes));
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