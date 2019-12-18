using BitMiracle.LibTiff.Classic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperaturePrediction.Data.Helper
{
    public class GeoTIFF
    {
        private int X1, Y1, X2, Y2;


        private float[,] heightMap;
        public float[,] HeightMap
        {
            get { return heightMap; }
            private set { heightMap = value; }
        }

        private int nWidth;
        public int NWidth
        {
            get { return nWidth; }
            private set { nWidth = value; }
        }

        private int nHeight;
        public int NHeight
        {
            get { return nHeight; }
            private set { nHeight = value; }
        }

        public GeoTIFF(string fn)
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

                double startLat = originLat + (pixelSizeY / 2.0);
                double startLon = originLon + (pixelSizeX / 2.0);

                var scanline = new byte[tiff.ScanlineSize()];

                //TODO: Check if band is stored in 1 byte or 2 bytes. 
                //If 2, the following code would be required
                var scanline16Bit = new ushort[tiff.ScanlineSize() / 2];
                //Buffer.BlockCopy(scanline, 0, scanline16Bit, 0, scanline.Length);

                nHeight = height;
                nWidth = scanline16Bit.Length;
                heightMap = new float[nHeight, nWidth];

                double currentLat = startLat;
                double currentLon = startLon;

                for (int i = 0; i < height; i++)
                {
                    tiff.ReadScanline(scanline, i); //Loading ith Line            
                    Buffer.BlockCopy(scanline, 0, scanline16Bit, 0, scanline.Length);

                    var latitude = currentLat + (pixelSizeY * i);
                    for (var j = 0; j < scanline16Bit.Length; j++)
                    {
                        var longitude = currentLon + (pixelSizeX * j);
                        //geodata.Points[0] = new[] { new PointXY(longitude, latitude) };
                        //byte value = scanline[j];

                        heightMap[i, j] = scanline16Bit[j];
                    }
                }
            }
        }
    }
}
