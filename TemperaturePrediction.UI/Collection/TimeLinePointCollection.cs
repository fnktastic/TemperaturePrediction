using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.UI.Model;

namespace TemperaturePrediction.UI.Collection
{
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
