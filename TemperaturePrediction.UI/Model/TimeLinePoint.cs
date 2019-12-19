using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperaturePrediction.UI.Model
{
    public class TimeLinePoint
    {
        public int Area { get; set; }
        public double Map { get; set; }
        public double Meteo { get; set; }
        public DateTime DateTime { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
