using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperaturePrediction.Model
{
    public class TimeLinePointDto
    {
        [Key]
        public int Id { get; set; }
        public int Area { get; set; }
        public double Map { get; set; }
        public double Meteo { get; set; }
        public DateTime DateTime { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
