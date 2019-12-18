using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperaturePrediction.Model
{
    public abstract class Point // base pont
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float Cloudity { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class UnitPoint : Point // unit point for prediction
    {
        public float Ndvi { get; set; }
        public float PictureTemperature { get; set; }
        public float? StationTemperature { get; set; }
    }

    public class MapPoint : Point // point for map
    {
        public List<ChanelValue> ChanelValues { get; set; }
    }

    public struct ChanelValue // value of the map channel
    {
        public int No { get; set; }
        public int Value { get; set; }
    }

    public class Area // area of the scene
    {
        public int Number { get; set; }
        public List<MapPoint> MapPoints { get; set; }
        public List<UnitPoint> UnitPoints { get; set; }
    }

    public class Scene // scene of the map
    {
        public string Id { get; set; }
        public List<Area> Areas { get; set; }
    }
}
