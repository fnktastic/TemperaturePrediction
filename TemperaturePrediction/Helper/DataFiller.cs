using BitMiracle.LibTiff.Classic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.Model;

namespace TemperaturePrediction.Data.Helper
{
    public static class MetadataTags
    {
        public static string RADIANCE_MULT_BAND_10 = "RADIANCE_MULT_BAND_10";
        public static string RADIANCE_ADD_BAND_10 = "RADIANCE_ADD_BAND_10";

        public static string K1_CONSTANT_BAND_10 = "K1_CONSTANT_BAND_10";
        public static string K2_CONSTANT_BAND_10 = "K2_CONSTANT_BAND_10";
    }

    public class DataFiller
    {
        private const string scenesPath = @"C:\Users\fnkta\Documents\Диплом 2019\Scenes";

        private const string scenesOutputPath = @"C:\Users\fnkta\Documents\Диплом 2019\Scenes\Output";

        private int X1 = 5000, Y1 = 5000, OFFSET = 100;

        private GeoTIFF Band10Tiff;
        private GeoTIFF Band4Tiff;
        private GeoTIFF Band5Tiff;

        private float[,] Ndvi;
        private double[,] Eps;
        private double[,] BT;
        private double[,] LST;

        private float GetMetadataValue(string path, string tag)
        {
            var lines = File.ReadAllLines(path);

            var targetLine = lines.FirstOrDefault(x => x.Trim().StartsWith(tag));

            if (targetLine != null)
            {
                string stringValue = targetLine.Split('=').ElementAt(1).Trim();

                float value = float.Parse(stringValue, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                return value;
            }

            return 0;
        }

        private byte GetTemperatureForPoint(byte point)
        {
            return 5;
        }

        public Scene BuildScene()
        {
            var scenes = Directory.GetDirectories(scenesPath);

            foreach (var scene in scenes)
            {
                string sceneName = Path.GetDirectoryName(scene);

                var files = Directory.GetFiles(scene);

                var band10 = files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).EndsWith("B10"));

                var band5 = files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).EndsWith("B5"));

                var band4 = files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).EndsWith("B4"));

                var metadata = files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).EndsWith("MTL"));

                if (band10 != null && band4 != null && band5 != null && metadata != null)
                {
                    Band10Tiff = new GeoTIFF(band10, X1, Y1, OFFSET);

                    Band4Tiff = new GeoTIFF(band4, X1, Y1, OFFSET);

                    Band5Tiff = new GeoTIFF(band5, X1, Y1, OFFSET);

                    float Ml = GetMetadataValue(metadata, MetadataTags.RADIANCE_MULT_BAND_10);

                    float Al = GetMetadataValue(metadata, MetadataTags.RADIANCE_ADD_BAND_10);

                    float K1 = GetMetadataValue(metadata, MetadataTags.K1_CONSTANT_BAND_10);

                    float K2 = GetMetadataValue(metadata, MetadataTags.K2_CONSTANT_BAND_10);

                    int ndviHeight = OFFSET;
                    int ndviWidth = OFFSET;

                    BT = new double[ndviHeight, ndviWidth];

                    Ndvi = new float[ndviHeight, ndviWidth];
                    Eps = new double[ndviHeight, ndviWidth];
                    LST = new double[ndviHeight, ndviWidth];

                    float ndviMin = float.MaxValue, ndviMax = float.MinValue;

                    for (int i = 0; i < ndviHeight; i++)
                        for (int j = 0; j < ndviWidth; j++)
                        {
                            float band4Value = Band4Tiff.HeightMap[i, j];
                            float band5value = Band5Tiff.HeightMap[i, j];

                            if (band4Value > 0 && band5value > 0)
                            {
                                float ndvi = (band5value - band4Value) / (band5value + band4Value);

                                if (ndvi > ndviMax)
                                    ndviMax = ndvi;

                                if (ndvi < ndviMin)
                                    ndviMin = ndvi;

                                Ndvi[i, j] = ndvi;
                            }
                        }

                    for (int i = 0; i < ndviHeight; i++)
                        for (int j = 0; j < ndviWidth; j++)
                        {
                            var height = Band10Tiff.HeightMap[i, j];

                            if (height > 0)
                            {
                                var L = (Ml * height) + Al; //step 1

                                BT[i, j] = ((K2 / Math.Log(K1 / L) + 1F) - 273.15F); // step 2

                                double pv = Math.Pow((Ndvi[i, j] - ndviMin) / (ndviMax - ndviMin), 2); // step 3

                                Eps[i, j] = 0.004 * pv + 0.986; // step 4

                                double val = (BT[i, j] / (1 + (0.00115 * BT[i, j] / 1.4388) * Math.Log(Eps[i, j]))); // step 5

                                if (val > 0)
                                {
                                    Console.WriteLine(val);
                                    LST[i, j] = val;
                                }
                            }
                        }
                }
            }

            return new Scene();
        }
    }
}
