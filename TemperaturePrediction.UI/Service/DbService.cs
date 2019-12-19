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
    }

    public class DbService : IDbService
    {
        private readonly ISceneRepository _sceneRepository;
        private readonly IMapper _mapper;

        public DbService(ISceneRepository sceneRepository, IMapper mapper)
        {
            _sceneRepository = sceneRepository;
            _mapper = mapper;
        }

        public async Task InsertSceneRangeAsync(List<Scene> scenes)
        {
            await _sceneRepository.InsertRange(_mapper.Map<List<SceneDto>>(scenes));
        }
    }
}
