using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemperaturePrediction.Model
{
    public class MapPointDto : PointDto // point for map
    {
        public List<ChanelValueDto> ChanelValues { get; set; }
    }
}
