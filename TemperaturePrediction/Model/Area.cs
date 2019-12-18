using System.Collections.Generic;

namespace TemperaturePrediction.Model
{
    public class Area // area of the scene
    {
        public int Number { get; set; }
        public List<MapPoint> MapPoints { get; set; }
        public List<UnitPoint> UnitPoints { get; set; }
    }
}
