using BitMiracle.LibTiff.Classic;
using CoordinateSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.Model;

namespace TemperaturePrediction.Data.Helper
{
    public class GeoTIFF
    {
        public float[,] HeightMap { get; private set; }
        public int NWidth { get; private set; }
        public int NHeight { get; private set; }

        private static LatLonDto GetLatLon(double x, double y, int zone)
        {
            UniversalTransverseMercator utm = new UniversalTransverseMercator("Q", zone, x, y);
            Coordinate c = UniversalTransverseMercator.ConvertUTMtoLatLong(utm);

            return new LatLonDto(c.Latitude.DecimalDegree, c.Longitude.DecimalDegree);
        }

        public static LatLonDto LonLat(string fn, int zone, int X1, int Y1, int OFFSET)
        {
            using (Tiff tiff = Tiff.Open(fn, "r"))
            {
                int height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                FieldValue[] modelPixelScaleTag = tiff.GetField(TiffTag.GEOTIFF_MODELPIXELSCALETAG);
                FieldValue[] modelTiepointTag = tiff.GetField(TiffTag.GEOTIFF_MODELTIEPOINTTAG);

                byte[] modelPixelScale = modelPixelScaleTag[1].GetBytes();
                double pixelSizeX = BitConverter.ToDouble(modelPixelScale, 0);
                double pixelSizeY = BitConverter.ToDouble(modelPixelScale, 8) * -1;

                byte[] modelTransformation = modelTiepointTag[1].GetBytes();
                double originLon = BitConverter.ToDouble(modelTransformation, 24);
                double originLat = BitConverter.ToDouble(modelTransformation, 32);

                double startLon = originLon + (pixelSizeX / 2.0);
                double startLat = originLat + (pixelSizeY / 2.0);

                var scanline16Bit = new ushort[OFFSET];

                double currentLat = startLat;
                double currentLon = startLon;

                var latitude = currentLat + (pixelSizeY * ((X1 + OFFSET) / 2));

                var longitude = currentLon + (pixelSizeX * scanline16Bit.Length / 2);

                return GetLatLon(longitude, latitude, zone);
            }
        }

        public GeoTIFF(string fn, int X1 = 5000, int Y1 = 5000, int OFFSET = 100)
        {
            using (Tiff tiff = Tiff.Open(fn, "r"))
            {
                int height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                FieldValue[] modelPixelScaleTag = tiff.GetField(TiffTag.GEOTIFF_MODELPIXELSCALETAG);
                FieldValue[] modelTiepointTag = tiff.GetField(TiffTag.GEOTIFF_MODELTIEPOINTTAG);

                byte[] modelPixelScale = modelPixelScaleTag[1].GetBytes();
                double pixelSizeX = BitConverter.ToDouble(modelPixelScale, 0);
                double pixelSizeY = BitConverter.ToDouble(modelPixelScale, 8) * -1;

                byte[] modelTransformation = modelTiepointTag[1].GetBytes();
                double originLon = BitConverter.ToDouble(modelTransformation, 24);
                double originLat = BitConverter.ToDouble(modelTransformation, 32);

                double startLon = originLon + (pixelSizeX / 2.0);
                double startLat = originLat + (pixelSizeY / 2.0);

                var scanline = new byte[tiff.ScanlineSize()];

                //TODO: Check if band is stored in 1 byte or 2 bytes. 
                //If 2, the following code would be required
                var scanline16Bit = new ushort[OFFSET];
                //Buffer.BlockCopy(scanline, 0, scanline16Bit, 0, scanline.Length);

                NHeight = height;
                NWidth = scanline16Bit.Length;
                HeightMap = new float[OFFSET, OFFSET];

                double currentLat = startLat;
                double currentLon = startLon;

                for (int i = X1, p = 0; i < X1 + OFFSET; i++, p++)
                {
                    tiff.ReadScanline(scanline, i); //Loading ith Line            
                    Buffer.BlockCopy(scanline, Y1, scanline16Bit, 0, OFFSET * 2);

                    var latitude = currentLat + (pixelSizeY * i);
                    for (int j = Y1, q = 0; q < scanline16Bit.Length; j++, q++)
                    {
                        var longitude = currentLon + (pixelSizeX * j);
                        HeightMap[p, q] = scanline16Bit[q];
                    }
                }
            }
        }
    }
}
