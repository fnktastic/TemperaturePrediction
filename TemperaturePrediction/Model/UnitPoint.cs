namespace TemperaturePrediction.Model
{
    public class UnitPoint : Point // unit point for prediction
    {
        public float Ndvi { get; set; }
        public double PictureTemperature { get; set; }
        public float? StationTemperature { get; set; }
    }
}
