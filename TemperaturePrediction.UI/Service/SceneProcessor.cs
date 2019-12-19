using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TemperaturePrediction.Data.Helper;
using TemperaturePrediction.Model;

namespace TemperaturePrediction.UI.Service
{
    public class SceneProcessor
    {
        private readonly string _scenesPath;
        private readonly List<string> _scenes;

        private readonly List<Point> _points;

        private GeoTIFF Band10Tiff;
        private GeoTIFF Band4Tiff;
        private GeoTIFF Band5Tiff;

        private float[,] Ndvi;
        private double[,] Eps;
        private double[,] BT;
        private double[,] LST;

        public SceneProcessor(string scenesPath, List<Point> points)
        {
            _scenesPath = scenesPath;
            _points = points;
            _scenes = Directory.GetDirectories(_scenesPath).ToList();
        }

        public List<SceneDto> ProcessMap()
        {
            var mapScenes = new List<SceneDto>();

            foreach (var scenePath in _scenes)
            {
                var scene = BuildScene(scenePath);

                mapScenes.Add(scene);
            }

            return mapScenes;
        }

        private SceneDto BuildScene(string scenePath)
        {
            var scene = new SceneDto();

            scene.Id = GetMetadataStringValue(scenePath, "LANDSAT_SCENE_ID"); 
            scene.Name = GetMetadataStringValue(scenePath, "LANDSAT_PRODUCT_ID");
            scene.Path = scenePath;
            scene.Metadata = Directory.GetFiles(scenePath).FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).EndsWith("MTL"));
            scene.Cloudity = GetMetadataStringValue(scenePath, "CLOUD_COVER");
            scene.TimeStamp = DateTime.Parse(GetMetadataStringValue(scenePath, "DATE_ACQUIRED"));
            scene.Image = Path.Combine(scenePath, "preview.jpg");

            for (int i = 0; i < _points.Count; i++)
            {
                var point = _points[i];
                var area = BuildArea(scenePath, i+1, point.X, point.Y, point.OFFSET);

                scene.Areas.Add(area);
            }

            return scene;
        }

        private float GetMetadataValue(string path, string tag)
        {
            var str = GetMetadataStringValue(path, tag);

            float value = float.Parse(str, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);

            return value;
        }

        private string GetMetadataStringValue(string path, string tag)
        {
            var mtl = Directory.GetFiles(path).FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).EndsWith("MTL"));

            var lines = File.ReadAllLines(mtl);

            var targetLine = lines.FirstOrDefault(x => x.Trim().StartsWith(tag));

            if (targetLine != null)
            {
                string stringValue = targetLine.Split('=').ElementAt(1).Trim();

                return stringValue;
            }

            return string.Empty;
        }

        private AreaDto BuildArea(string scene, int areaNumber, int X1, int Y1, int OFFSET)
        {
            LatLonDto lonLat = new LatLonDto();

            var mapPoints = new List<MapPointDto>();
            var unitPoints = new List<UnitPointDto>();

            string sceneName = Path.GetDirectoryName(scene);

            var files = Directory.GetFiles(scene);

            var band10 = files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).EndsWith("B10"));

            var band5 = files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).EndsWith("B5"));

            var band4 = files.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).EndsWith("B4"));

            if (band10 != null && band4 != null && band5 != null)
            {
                int utm_zone = int.Parse(GetMetadataValue(scene, MetadataTags.UTM_ZONE).ToString());

                lonLat = GeoTIFF.LonLat(band10, utm_zone, X1, Y1, OFFSET);

                Band10Tiff = new GeoTIFF(band10, X1, Y1, OFFSET);

                Band4Tiff = new GeoTIFF(band4, X1, Y1, OFFSET);

                Band5Tiff = new GeoTIFF(band5, X1, Y1, OFFSET);

                float Ml = GetMetadataValue(scene, MetadataTags.RADIANCE_MULT_BAND_10);

                float Al = GetMetadataValue(scene, MetadataTags.RADIANCE_ADD_BAND_10);

                float K1 = GetMetadataValue(scene, MetadataTags.K1_CONSTANT_BAND_10);

                float K2 = GetMetadataValue(scene, MetadataTags.K2_CONSTANT_BAND_10);

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

                                unitPoints.Add(new UnitPointDto()
                                {
                                    X = X1 + i,
                                    Y = Y1 + j,
                                    Ndvi = Ndvi[i, j],
                                    PictureTemperature = LST[i, j],
                                    StationTemperature = null,
                                });

                                mapPoints.Add(new MapPointDto()
                                {
                                    X = X1 + i,
                                    Y = Y1 + j,
                                    ChanelValues = new List<ChanelValueDto>()
                                    {
                                        new ChanelValueDto(10, Band10Tiff.HeightMap[i,j]),
                                        new ChanelValueDto(5, Band5Tiff.HeightMap[i,j]),
                                        new ChanelValueDto(4, Band4Tiff.HeightMap[i,j]),
                                    }
                                });
                            }
                        }
                    }
            }

            return new AreaDto()
            {
                MapPoints = mapPoints,
                UnitPoints = unitPoints,
                Number = areaNumber,
                LonLat = lonLat
            };
        }
    }
}
