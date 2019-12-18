using System.Collections.Generic;

namespace TemperaturePrediction.Model
{
    public class MapPoint : Point // point for map
    {
        public List<ChanelValue> ChanelValues { get; set; }
    }
}
