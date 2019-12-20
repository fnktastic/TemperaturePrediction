using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemperaturePrediction.Model
{
    public class SceneDto // scene of the map
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }
        public List<AreaDto> Areas { get; set; }
        public string Cloudity { get; set; }
        public string Path { get; set; }
        public string Metadata { get; set; }
        public DateTime TimeStamp { get; set; }
        public TimeSpan Time { get; set; }

        public SceneDto()
        {
            Areas = new List<AreaDto>();
        }
    }
}
