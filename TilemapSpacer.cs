using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TilemapSpacer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine(@"usage: TilemapSpacer source_image tile_size gap_size");
                return;
            }

            string inputPath = args[0];
            int tileSize = int.Parse(args[1]);
            int gapSize = int.Parse(args[2]);

            Bitmap sourceImage = Read(inputPath);

            int sliceW = sourceImage.Width / tileSize;
            int sliceH = sourceImage.Height / tileSize;
            if (sourceImage.Width % tileSize != 0 || sourceImage.Height % tileSize != 0)
            {
                throw new System.Exception("Source image not divisible by tile size");
            }

            int wGap = (sliceW + 1) * gapSize;
            int hGap = (sliceH + 1) * gapSize;
            Bitmap destImage = new Bitmap(sourceImage.Width + wGap, sourceImage.Height + hGap);

            destImage.MakeTransparent(destImage.GetPixel(1, 1));

            using (Graphics grD = Graphics.FromImage(destImage))
            {
                for (int xSlice = 0; xSlice < sliceW; xSlice++)
                {
                    int xGap = (xSlice + 1) * gapSize;
                    int xSrc = xSlice * tileSize;
                    for (int ySlice = 0; ySlice < sliceH; ySlice++)
                    {
                        int yGap = (ySlice + 1) * gapSize;
                        int ySrc = ySlice * tileSize;

                        Rectangle src = new Rectangle(xSrc, ySrc, tileSize, tileSize);
                        Rectangle dst = new Rectangle(xSrc + xGap, ySrc + yGap, tileSize, tileSize);
                        grD.DrawImage(sourceImage, dst, src, GraphicsUnit.Pixel);
                    }
                }
            }




            string outPath = Path.Join(Path.GetDirectoryName(inputPath), Path.GetFileNameWithoutExtension(inputPath) + "_G.png");
            destImage.Save(outPath, GetPNGEncoder(), null);

        }

        private static ImageCodecInfo GetPNGEncoder()
        {
            ImageCodecInfo[] encs = ImageCodecInfo.GetImageEncoders();
            foreach (var i in encs)
            {
                if (i.FormatDescription == "PNG") return i;
            }
            throw new System.Exception("No PNG encoder found");
        }

        private static Bitmap Read(string inputPath)
        {
            using (FileStream pngStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            {
                return new Bitmap(pngStream);
            }
        }
    }
}
