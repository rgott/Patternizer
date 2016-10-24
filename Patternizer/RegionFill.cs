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

        public static SVG FillRegionList(Bitmap Img, int FillWidth, int FillHeight)
        {
            Rectangle ImgRect = new Rectangle(0, 0, Img.Width, Img.Height); // generate for all objects
            SVG svg = new SVG(ImgRect);
            int count = 0;
            for (int currentX = 0; currentX < Img.Width; currentX += FillWidth)
            {
                for (int currentY = (count % 2 == 0) ? 0 : -FillHeight / 2; currentY < ((count % 2 == 0) ? Img.Height : FillHeight + Img.Height); currentY += FillHeight)
                {
                    svg.addPoint(new LeftPointTriangle(ImgRect, currentX, currentY, FillWidth, FillHeight).getAverageColor(Img),new Point[]
                    {
                        new Point(currentX + FillWidth, currentY),
                        new Point(currentX, currentY + (FillHeight / 2)),
                        new Point(currentX, currentY - (FillHeight / 2))
                    });

                    svg.addPoint(new RightPointTriangle(ImgRect, currentX, currentY + FillHeight / 2, FillWidth, FillHeight).getAverageColor(Img) ,new Point[]
                    {
                        new Point(currentX, currentY + (FillHeight / 2)),
                        new Point(currentX + FillWidth, currentY + FillHeight),
                        new Point(currentX + FillWidth, currentY)
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

        private Color getAverageColor(Bitmap Img)
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
