using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Patternizer
{
    public abstract class RegionFill
    {
        private object Lock = new object();

        public static Bitmap CreateNonIndexedImage(Image src)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }
        private Rectangle _ImgRect;
        public Rectangle ImgRect
        {
            get
            {
                if (_ImgRect == default(Rectangle))
                {
                    return _ImgRect = new Rectangle(0, 0, Img.Width, Img.Height);
                }
                return _ImgRect;
            }
            set
            {
                _ImgRect = value;
            }
        }


        private List<RowRangeData> _Fill;
        public List<RowRangeData> Fill { get { return _Fill ?? (_Fill = getIndexArray(StartX, StartY, Width, Height)); } }

        private Color _AvgColor;
        public Color AvgColor
        {
            get
            {
                if (_AvgColor == default(Color))
                {
                    return (_AvgColor = getAverageColor(Img, Fill));
                }
                return _AvgColor;
            }
        }

        int StartX;
        int StartY;

        int Width;
        int Height;
        public Bitmap Img { get; set; }

        protected RegionFill(Bitmap Img, int StartX, int StartY, int Width, int Height)
        {
            this.StartX = StartX;
            this.StartY = StartY;
            this.Width = Width;
            this.Height = Height;
            this.Img = Img;
        }

        public static void FillRegionList(Bitmap Img, int FillWidth, int FillHeight)
        {
            //new RightPointTriangle(Img, 380, 176, FillWidth, FillHeight).FillRegion();
            int count = 0;
            for (int currentX = 0; currentX < Img.Width; currentX += FillWidth)
            {
                for (int currentY = (count % 2 == 0) ? 0 : -FillHeight / 2; currentY < ((count % 2 == 0) ? Img.Height : FillHeight + Img.Height); currentY += FillHeight)
                {
                    new LeftPointTriangle(Img, currentX, currentY, FillWidth, FillHeight).FillRegion();
                    new RightPointTriangle(Img, currentX, currentY + FillHeight / 2, FillWidth, FillHeight).FillRegion();
                    //new Square(Img, currentX, currentY, FillWidth, FillHeight).FillRegion();
                }
                count++;
            }
        }

        public void FillRegion()
        {
            foreach (RowRangeData item in Fill)
            {
                for (int i = item.From; i < item.To; i++)
                {
                    Img.SetPixel(i, item.row, AvgColor);
                }
            }
            //Img.Save("SDFK.bmp");// Debugging
        }


        //int start;
        //int stop;
        //int width;
        //int height;
        //Bitmap picture;

        // assumed startX and startY are > 0
        /// <summary>
        ///
        /// </summary>
        /// <param name="StartX"></param>
        /// <param name="StartY"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns>Should not return any negative numbers</returns>
        protected abstract List<RowRangeData> getIndexArray(int StartX, int StartY, int Width, int Height);

        // just so the implementer knows that fill exists
        protected Color getAverageColor(Bitmap Image, List<RowRangeData> Fill)
        {
            ColorAvg cavg = new ColorAvg();
            foreach (RowRangeData range in Fill)
            {
                for (int i = range.From; i < range.To; i++)
                    cavg.Add(Image.GetPixel(i, range.row));
            }
            return cavg.getAverage();
        }


        public class ColorAvg
        {
            ulong A = 0;
            ulong R = 0;
            ulong G = 0;
            ulong B = 0;
            ulong Count = 0;
            public void Add(Color color)
            {
                A += color.A;
                R += color.R;
                G += color.G;
                B += color.B;
                Count++;
            }

            public Color getAverage()
            {
                A /= Count;
                R /= Count;
                G /= Count;
                B /= Count;
                return Color.FromArgb((int)A, (int)R, (int)G, (int)B);                
            }

        }
    }
}
