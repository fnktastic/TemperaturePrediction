using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemperaturePrediction.Model
{
    public class UnitPointDto : PointDto // unit point for prediction
    {
        public float Ndvi { get; set; }
        public double PictureTemperature { get; set; }
        public float? StationTemperature { get; set; }
    }
}
