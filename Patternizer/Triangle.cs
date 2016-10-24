using System.Collections.Generic;
using System.Drawing;

namespace Patternizer
{
    public class RightPointTriangle : RegionFill
    {
        public RightPointTriangle(Rectangle ImgRect, int StartX, int StartY, int Width, int Height) 
            : base(ImgRect, StartX, StartY, Width, Height)
        {
            
        }

        protected override List<RowRangeData> getIndexArray(Rectangle ImgRect, int StartX, int StartY, int Width, int Height)
        {
            var list = new List<RowRangeData>();
            var frac = new SpecialFraction(Height / 2, Width);

            var range = new Range(StartX, StartX + Width);
            list.Add(new RowRangeData(StartY, range, ImgRect));// middle

            for (int i = 1; i <= Width; i++)
            {
                range = new Range(StartX, StartX + Width - (frac.Denominator * i));
                // pos slope
                list.Add(new RowRangeData(StartY - (frac.Numerator * i), range, ImgRect));
                // neg slope
                list.Add(new RowRangeData(StartY + (frac.Numerator * i), range, ImgRect));
            }
            return list;
        }
    }
    public class LeftPointTriangle : RegionFill
    {

        public LeftPointTriangle(Rectangle ImgRect, int StartX, int StartY, int Width, int Height) : base(ImgRect, StartX, StartY, Width, Height)
        {
        }

        protected override List<RowRangeData> getIndexArray(Rectangle ImgRect,int StartX, int StartY, int Width, int Height)
        {
            var list = new List<RowRangeData>();
            var frac = new SpecialFraction(Height / 2, Width);

            var range = new Range(StartX, StartX + Width);
            list.Add(new RowRangeData(StartY, range,ImgRect));// middle

            for (int i = 1; i <= Width; i++)
            {
                range = new Range(((frac.Denominator * i) + StartX), StartX + Width);
                // pos slope
                list.Add(new RowRangeData(StartY - (frac.Numerator * i), range, ImgRect));
                // neg slope
                list.Add(new RowRangeData(StartY + (frac.Numerator * i), range, ImgRect));
            }
            return list;
        }
    }
}
