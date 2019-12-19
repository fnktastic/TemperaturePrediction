using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemperaturePrediction.Model
{
    public abstract class PointDto // base pont
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
