using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Patternizer
{
    public abstract class RegionFill
    {
        public Rectangle ImgRect { get; set; }

        // masks and caches fill method
        private List<RowRangeData> _Fill;
        public List<RowRangeData> Fill { get { return _Fill ?? (_Fill = getIndexArray(ImgRect,StartX, StartY, Width, Height)); } }

        int StartX;
        int StartY;

        int Width;
        int Height;

        protected RegionFill(Rectangle ImgRect, int StartX, int StartY, int Width, int Height)
        {
            this.StartX = StartX;
            this.StartY = StartY;
            this.Width = Width;
            this.Height = Height;
            this.ImgRect = ImgRect;
        }
        public static void FillRegionList(Bitmap Img, int FillSize,params Color[] colors)
        {
            Rectangle ImgRect = new Rectangle(0, 0, Img.Width, Img.Height); // generate for all objects
            int count = 0;
            int colorCount = 0;
            for (int currentX = 0; currentX < Img.Width; currentX += FillSize)
            {
                for (int currentY = (count % 2 == 0) ? 0 : -FillSize / 2; currentY < FillSize + Img.Height; currentY += FillSize)
                {
                    new LeftPointTriangle(ImgRect, currentX, currentY, FillSize, FillSize).setAverageColorUnsafe(Img,colors[colorCount++]);
                    new RightPointTriangle(ImgRect, currentX, currentY + FillSize / 2, FillSize, FillSize).setAverageColorUnsafe(Img,colors[colorCount++]);

                    //new Square(Img, currentX, currentY, FillWidth, FillHeight).FillRegion();
                }
                count++;
            }
        }
        public static SVG FillRegionList(Bitmap Img, int FillWidth, int FillHeight)
        {
            Rectangle ImgRect = new Rectangle(0, 0, Img.Width, Img.Height); // generate for all objects
            SVG svg = new SVG(ImgRect);
            int count = 0;
            for (int currentX = 0; currentX < Img.Width; currentX += FillWidth)
            {
                for (int currentY = (count % 2 == 0) ? 0 : -FillHeight / 2; currentY < FillHeight + Img.Height; currentY += FillHeight)
                {
                    svg.addPoint(new LeftPointTriangle(ImgRect, currentX, currentY, FillWidth, FillHeight).getandSetAverageColor(Img),new System.Windows.Point[]
                    {
                        new System.Windows.Point(currentX + FillWidth, currentY),
                        new System.Windows.Point(currentX, currentY + (FillHeight / 2.0)),
                        new System.Windows.Point(currentX, currentY - (FillHeight / 2.0))
                    });

                    svg.addPoint(new RightPointTriangle(ImgRect, currentX, currentY + FillHeight / 2, FillWidth, FillHeight).getandSetAverageColor(Img) ,new System.Windows.Point[]
                    {
                        new System.Windows.Point(currentX, currentY + (FillHeight / 2.0)),
                        new System.Windows.Point(currentX + FillWidth, currentY + FillHeight),
                        new System.Windows.Point(currentX + FillWidth, currentY)
                    });

                    
                    //new Square(Img, currentX, currentY, FillWidth, FillHeight).FillRegion();
                }
                count++;
            }
            return svg;
        }

        //public abstract Color FillRegion(Bitmap Img);
        
        /// <returns>Should not return any negative numbers</returns>
        protected abstract List<RowRangeData> getIndexArray(Rectangle ImgRect,int StartX, int StartY, int Width, int Height);

        private Color getandSetAverageColor(Bitmap Img)
        {
            //return getAverageColorUnsafe(Img);
            return setAndgetAverageColorUnsafe(Img);
        }
        private Color getAverageColorUnsafe(Bitmap processedBitmap)
        {
            unsafe
            {
                BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);

                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                long R = 0;
                long G = 0;
                long B = 0;
                int count = 0;
                for (int i = 0; i < Fill.Count; i++)
                {
                    byte* currentLine = PtrFirstPixel + (Fill[i].row * bitmapData.Stride);
                    widthInBytes = (Fill[i].To) * bytesPerPixel;
                    for (int x = (Fill[i].From) * bytesPerPixel; x < widthInBytes; x += bytesPerPixel)
                    {
                        B += currentLine[x];
                        G += currentLine[x + 1];
                        R += currentLine[x + 2];
                        count += 1;
         
                        // set values of average
                        //currentLine[x] = (byte)oldBlue;
                        //currentLine[x + 1] = (byte)oldGreen;
                        //currentLine[x + 2] = (byte)oldRed;
                     }
                }

                processedBitmap.UnlockBits(bitmapData);

                return Color.FromArgb((int)(R / count),(int)(G / count), (int)(B / count));
            }
        }
        private Color setAndgetAverageColorUnsafe(Bitmap processedBitmap)
        {
            unsafe
            {
                BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);

                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                long R = 0;
                long G = 0;
                long B = 0;
                int count = 0;

                // gets the average Color for the region
                for (int i = 0; i < Fill.Count; i++)
                {
                    byte* currentLine = PtrFirstPixel + (Fill[i].row * bitmapData.Stride);
                    widthInBytes = (Fill[i].To) * bytesPerPixel;
                    for (int x = (Fill[i].From) * bytesPerPixel; x < widthInBytes; x += bytesPerPixel)
                    {
                        B += currentLine[x];
                        G += currentLine[x + 1];
                        R += currentLine[x + 2];
                        count += 1;

                        // set values of average
                        //currentLine[x] = (byte)oldBlue;
                        //currentLine[x + 1] = (byte)oldGreen;
                        //currentLine[x + 2] = (byte)oldRed;
                    }
                }

                byte r = (byte)(int)(R / count);
                byte g = (byte)(int)(G / count);
                byte b = (byte)(int)(B / count);

                // sets all colors in region to that color
                for (int i = 0; i < Fill.Count; i++)
                {
                    byte* currentLine = PtrFirstPixel + (Fill[i].row * bitmapData.Stride);
                    widthInBytes = (Fill[i].To) * bytesPerPixel;
                    for (int x = (Fill[i].From) * bytesPerPixel; x < widthInBytes; x += bytesPerPixel)
                    {
                        // set values of average
                        currentLine[x] = b;
                        currentLine[x + 1] = g;
                        currentLine[x + 2] = r;
                    }
                }

                processedBitmap.UnlockBits(bitmapData);

                return Color.FromArgb(r,g,b);
            }
        }

        private void setAverageColorUnsafe(Bitmap processedBitmap,Color color)
        {
            unsafe
            {
                BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);

                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                long R = 0;
                long G = 0;
                long B = 0;

                // set the color for the region
                for (int i = 0; i < Fill.Count; i++)
                {
                    byte* currentLine = PtrFirstPixel + (Fill[i].row * bitmapData.Stride);
                    widthInBytes = (Fill[i].To) * bytesPerPixel;
                    for (int x = (Fill[i].From) * bytesPerPixel; x < widthInBytes; x += bytesPerPixel)
                    {
                        currentLine[x    ] = color.B;
                        currentLine[x + 1] = color.G;
                        currentLine[x + 2] = color.R;
                    }
                }
                processedBitmap.UnlockBits(bitmapData);
            }
        }
        //protected Color getAverageColor(Bitmap Image)
        //{ 
        //    ColorAvg cavg = new ColorAvg(); 
        //    foreach (RowRangeData range in Fill)
        //    {
        //        for (int i = range.From; i < range.To; i++)
        //        {
        //            cavg.Add(Image.GetPixel(i, range.row));
        //        }
        //    }
        //    return cavg.getAverage();
        //}
    }
}
