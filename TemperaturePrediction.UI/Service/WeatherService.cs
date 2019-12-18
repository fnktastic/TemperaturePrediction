﻿using System;
using System.Threading.Tasks;
using WorldWeatherOnline;

namespace TemperaturePrediction.UI.Service
{
    public interface IWeatherService
    {
        Task GetPastWeatherForLocationAsync(string location, DateTime startDate);
    }

    public class WeatherService : IWeatherService
    {
        private readonly Api _api;
        private const string API_KEY = "1f44692ab2c545de9b5132926191812";

        public WeatherService()
        {
            _api = _api ?? new Api(API_KEY);
        }

        public async Task GetPastWeatherForLocationAsync(string location, DateTime startDate)
        {
            var res = await _api.BuildPastWeatherQuery(location, startDate)
                .WithEnddate(startDate)
                .WithIncludeLocation(true)
                .GetResult();

            var temp = res.data.weather;
        }
    }
}
