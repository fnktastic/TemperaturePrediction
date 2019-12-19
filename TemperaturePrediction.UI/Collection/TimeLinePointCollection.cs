using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.UI.Model;

namespace TemperaturePrediction.UI.Collection
{
    public class TimeLinePoint
    {
        public int Area { get; set; }
        public double Map { get; set; }
        public double Meteo { get; set; }
        public DateTime DateTime { get; set; }
        public LatLon LonLatn { get; set; }
    }

    public class TimeLinePointCollection : ObservableCollection<TimeLinePoint>
    {
        public TimeLinePointCollection(IList<TimeLinePoint> timeLinePoints) : base(timeLinePoints)
        {

        }

        public TimeLinePointCollection()
        {

        }
    }
}
