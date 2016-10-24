using System.Collections.Generic;
using System.Drawing;

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

        protected Color getAverageColor(Bitmap Image)
        {
            ColorAvg cavg = new ColorAvg();
            foreach (RowRangeData range in Fill)
            {
                for (int i = range.From; i < range.To; i++)
                {
                    cavg.Add(Image.GetPixel(i, range.row));
                }
            }
            return cavg.getAverage();
        }
    }
}
