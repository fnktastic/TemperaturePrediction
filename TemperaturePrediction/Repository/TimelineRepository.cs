using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.DataAccess;
using TemperaturePrediction.Model;

namespace TemperaturePrediction.Repository
{
    public interface ITimelineRepository
    {
        Task InsertRange(List<TimeLinePointDto> timeLinePoints);
        Task<List<TimeLinePointDto>> GetAll();
    }

    public class TimelineRepository : ITimelineRepository
    {
        private readonly Context _context;

        public TimelineRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<TimeLinePointDto>> GetAll()
        {
            return await _context.Points.ToListAsync();
        }

        public async Task InsertRange(List<TimeLinePointDto> timeLinePoints)
        {
            _context.Points.AddRange(timeLinePoints);

            await _context.SaveChangesAsync();
        }
    }
}
