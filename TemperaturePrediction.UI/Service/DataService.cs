using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.Model;

namespace TemperaturePrediction.UI.Service
{
    public interface IDataService
    {
        Task<List<SceneDto>> GetScenesAsync(string scenesPath, List<Point> points);
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
                    var meteo = await _weatherService.GetPastWeatherForLocationAsync(area.LonLat.ToString(), scene.TimeStamp);

                    area.Meteo = meteo;
                }
            }

            return scenes.OrderBy(x => x.TimeStamp).ToList();
        }

        private List<SceneDto> ProcessScenes(string scenesPath, List<Point> points)
        {
            var dataFiller = new SceneProcessor(scenesPath, points);

            return dataFiller.ProcessMap();
        }
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
}
