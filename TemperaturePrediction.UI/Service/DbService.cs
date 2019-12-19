using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.Model;
using TemperaturePrediction.Repository;
using TemperaturePrediction.UI.Model;

namespace TemperaturePrediction.UI.Service
{
    public interface IDbService
    {
        Task InsertSceneRangeAsync(List<Scene> scenes);

        Task<List<TimeLinePoint>> GetTimeLinePoints();
        Task InsertTimeLinePoints(List<TimeLinePoint> timeLinePoints);
    }

    public class DbService : IDbService
    {
        private readonly ISceneRepository _sceneRepository;
        private readonly ITimelineRepository _timelineRepository;
        private readonly IMapper _mapper;

        public DbService(ISceneRepository sceneRepository, IMapper mapper, ITimelineRepository timelineRepository)
        {
            _sceneRepository = sceneRepository;
            _mapper = mapper;
            _timelineRepository = timelineRepository;
        }

        public async Task InsertSceneRangeAsync(List<Scene> scenes)
        {
            await _sceneRepository.InsertRange(_mapper.Map<List<SceneDto>>(scenes));
        }

        public async Task<List<TimeLinePoint>> GetTimeLinePoints()
        {
            var points = await _timelineRepository.GetAll();

            return _mapper.Map<List<TimeLinePoint>>(points);
        }

        public async Task InsertTimeLinePoints(List<TimeLinePoint> timeLinePoints)
        {
            var points = _mapper.Map<List<TimeLinePointDto>>(timeLinePoints);

            await _timelineRepository.InsertRange(points);
        }
    }
}
