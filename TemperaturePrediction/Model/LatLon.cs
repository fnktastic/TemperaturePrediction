using System.Globalization;

namespace TemperaturePrediction.Model
{
    public struct LatLon
    {
        public double Lon { get; set; }
        public double Lat { get; set; }

        public LatLon(double lat, double lon)
        {
            Lon = lon;
            Lat = lat;
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
