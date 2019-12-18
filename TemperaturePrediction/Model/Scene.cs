using System;
using System.Collections.Generic;

namespace TemperaturePrediction.Model
{
    public class Scene // scene of the map
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public List<Area> Areas { get; set; }
        public string Cloudity { get; set; }
        public string Path { get; set; }
        public DateTime TimeStamp { get; set; }

        public Scene()
        {
            Areas = new List<Area>();
        }
    }
}
