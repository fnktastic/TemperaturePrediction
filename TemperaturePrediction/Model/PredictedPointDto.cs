using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperaturePrediction.Model
{
    public class PredictedPointDto
    {
        [Key]
        public int Id { get; set; }
        public double PredictedValue { get; set; }
        public double Confidence {get;set; }
        public DateTime DateTime { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
