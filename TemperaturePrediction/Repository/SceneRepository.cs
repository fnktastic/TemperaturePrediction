using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.DataAccess;
using TemperaturePrediction.Model;

namespace TemperaturePrediction.Repository
{
    public interface ISceneRepository
    {
        Task InsertRange(List<SceneDto> scenes);
    }

    public class SceneRepository : ISceneRepository
    {
        private readonly Context _context;

        public SceneRepository(Context context)
        {
            _context = context;
        }

        public async Task InsertRange(List<SceneDto> scenes)
        {
            _context.Scenes.AddRange(scenes);

            await _context.SaveChangesAsync();
        }
    }
}
