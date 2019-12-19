using System.Collections.Generic;

namespace TemperaturePrediction.UI.Model
{
    public class MapPoint : Point // point for map
    {
        public List<ChanelValue> ChanelValues { get; set; }
    }
}
