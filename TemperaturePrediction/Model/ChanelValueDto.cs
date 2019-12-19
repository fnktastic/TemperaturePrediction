using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemperaturePrediction.Model
{
    public class ChanelValueDto // value of the map channel
    {
        public ChanelValueDto(int no, float value)
        {
            No = no;
            Value = value;
        }

        public int No { get; set; }
        public float Value { get; set; }
    }
}
