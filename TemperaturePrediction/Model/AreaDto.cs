using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemperaturePrediction.Model
{
    public class AreaDto // area of the scene
    {
        public int Number { get; set; }
        public List<MapPointDto> MapPoints { get; set; }
        public List<UnitPointDto> UnitPoints { get; set; }
        public LatLonDto LonLat { get; set; }
        public MeteoDto Meteo { get; set; }
    }
}
