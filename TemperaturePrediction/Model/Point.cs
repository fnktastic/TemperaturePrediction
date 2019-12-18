using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperaturePrediction.Model
{
    public abstract class Point // base pont
    {
        public int X { get; set; }
        public int Y { get; set; }

        public double Lon { get; set; }
        public double Lat { get; set; }
    }
}
