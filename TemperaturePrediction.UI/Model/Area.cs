using System.Collections.Generic;

namespace TemperaturePrediction.UI.Model
{
    public class Area // area of the scene
    {
        public int Number { get; set; }
        public List<MapPoint> MapPoints { get; set; }
        public List<UnitPoint> UnitPoints { get; set; }
        public LatLon LonLat { get; set; }
        public Meteo Meteo { get; set; }
        public float R { get; set; }
    }
}
