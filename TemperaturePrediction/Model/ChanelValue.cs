namespace TemperaturePrediction.Model
{
    public struct ChanelValue // value of the map channel
    {
        public ChanelValue(int no, float value)
        {
            No = no;
            Value = value;
        }

        public int No { get; set; }
        public float Value { get; set; }
    }
}
