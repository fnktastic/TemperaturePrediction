namespace TemperaturePrediction.UI.Model
{
    public struct Meteo
    {
        public float Min { get; set; }
        public float Max { get; set; }

        public Meteo(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
