using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemperaturePrediction.Model
{
    public class LatLonDto
    {
        public double Lon { get; set; }
        public double Lat { get; set; }

        public LatLonDto(double lat, double lon)
        {
            Lon = lon;
            Lat = lat;
        }

        public LatLonDto()
        {

        }

        public override string ToString()
        {
            string specifier = "00.0000";

            return string.Format("{0}, {1}", 
                                    Lat.ToString(specifier, CultureInfo.InvariantCulture), 
                                    Lon.ToString(specifier, CultureInfo.InvariantCulture));
        }
    }
}
