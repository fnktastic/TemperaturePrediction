using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperaturePrediction.Model;
using TemperaturePrediction.UI.Model;

namespace TemperaturePrediction.UI.Service
{
    public interface IDataService
    {
        Task<List<SceneDto>> GetScenesAsync(string scenesPath, List<Point> points);
        Task Predict(List<TimeLinePoint> timeLinePoints, int step);
    }

    public class DataService : IDataService
    {
        private readonly IWeatherService _weatherService;
        private readonly IMapper _mapper;

        public DataService(IWeatherService weatherService, IMapper mapper)
        {

            _weatherService = weatherService;
            _mapper = mapper;
        }

        public async Task<List<SceneDto>> GetScenesAsync(string scenesPath, List<Point> points)
        {
            var scenes = ProcessScenes(scenesPath, points);

            foreach (var scene in scenes)
            {
                foreach (var area in scene.Areas)
                {
                    //var meteo = await _weatherService.GetPastWeatherForLocationAsync(area.LonLat.ToString(), scene.TimeStamp);

                    //area.Meteo = meteo;
                }
            }

            return scenes.OrderBy(x => x.TimeStamp).ToList();
        }

        private List<SceneDto> ProcessScenes(string scenesPath, List<Point> points)
        {
            var dataFiller = new SceneProcessor(scenesPath, points);

            return dataFiller.ProcessMap();
        }

        public async Task Predict(List<TimeLinePoint> timeLinePoints, int step)
        {
            //var dataset = timeLinePoints.Where(x => x.Area == 1 && x.Map == 0).Select(x => new ModelInput()
            //{
            //    RentalDate = x.DateTime,
            //    Temperature = float.Parse(x.Meteo.ToString()),
            //    Year = 0
            //});

            //MLContext mlContext = new MLContext();

            //var dataView = mlContext.Data.LoadFromEnumerable<ModelInput>(dataset);

            //IDataView firstYearData = mlContext.Data.FilterRowsByColumn(dataView, "Year", upperBound: 1);
            //IDataView secondYearData = mlContext.Data.FilterRowsByColumn(dataView, "Year", lowerBound: 1);
            

            //var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
            //    outputColumnName: "ForecastedTemperature",
            //    inputColumnName: "Temperature",
            //    windowSize: step,
            //    seriesLength: 30,
            //    trainSize: timeLinePoints.Count,
            //    horizon: 7,
            //    confidenceLevel: 0.95f,
            //    confidenceLowerBoundColumn: "LowerBoundTemperature",
            //    confidenceUpperBoundColumn: "UpperBoundTemperature");

            //SsaForecastingTransformer forecaster = forecastingPipeline.Fit(firstYearData);

            //Evaluate(secondYearData, forecaster, mlContext);
        }

        //static void Evaluate(IDataView testData, ITransformer model, MLContext mlContext)
        //{
        //    IDataView predictions = model.Transform(testData);

        //    IEnumerable<float> actual = mlContext.Data.CreateEnumerable<ModelInput>(testData, true)
        //        .Select(observed => observed.Temperature);

        //    IEnumerable<float> forecast =
        //        mlContext.Data.CreateEnumerable<ModelOutput>(predictions, true)
        //        .Select(prediction => prediction.ForecastedTemperature[0]);

        //    var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);

        //    var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
        //    var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error
        //}

        //private List<SceneDto> ProcessScenes(string scenesPath, List<Point> points)
        //{
        //    var dataFiller = new SceneProcessor(scenesPath, points);

        //    return dataFiller.ProcessMap();
        //}
    }

    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int OFFSET { get; set; }

        public Point(int x, int y, int offset)
        {
            X = x;
            Y = y;
            OFFSET = offset;
        }
    }

    public class ModelInput
    {
        public DateTime RentalDate { get; set; }

        public float Year { get; set; }

        public float Temperature { get; set; }
    }

    public class ModelOutput
    {
        public float[] ForecastedTemperature { get; set; }

        public float[] LowerBoundTemperature { get; set; }

        public float[] UpperBoundTemperature { get; set; }
    }
}
