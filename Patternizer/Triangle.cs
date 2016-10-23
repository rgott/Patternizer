using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patternizer
{
    public class RightPointTriangle : RegionFill
    {
        public RightPointTriangle(Bitmap Img, int StartX, int StartY, int Width, int Height) : base(Img, StartX, StartY, Width, Height)
        {
        }

        protected override List<RowRangeData> getIndexArray(int StartX, int StartY, int Width, int Height)
        {
            var list = new List<RowRangeData>();

            SpecialFraction frac = new SpecialFraction(Height / 2, Width);

            Range r = new Range(StartX, StartX + Width);
            list.Add(new RowRangeData(StartY, r, ImgRect));// middle

            for (int i = 1; i <= Width; i++)
            {
                r = new Range(StartX, StartX + Width - (frac.Denominator * i));
                // pos slope
                list.Add(new RowRangeData(StartY - (frac.Numerator * i), r, ImgRect));
                // neg slope
                list.Add(new RowRangeData(StartY + (frac.Numerator * i), r, ImgRect));
            }
            return list;
        }
    }
    public class LeftPointTriangle : RegionFill
    {
        public LeftPointTriangle(Bitmap Img, int StartX, int StartY, int Width, int Height) : base(Img, StartX, StartY, Width, Height)
        {
        }

        protected override List<RowRangeData> getIndexArray(int StartX, int StartY, int Width, int Height)
        {
            var list = new List<RowRangeData>();
            
            SpecialFraction frac = new SpecialFraction(Height / 2, Width);
            
            Range r = new Range(StartX, StartX + Width);
            list.Add(new RowRangeData(StartY, r,ImgRect));// middle

            for (int i = 1; i <= Width; i++)
            {
                r = new Range(((frac.Denominator * i) + StartX), StartX + Width);
                // pos slope
                list.Add(new RowRangeData(StartY - (frac.Numerator * i), r, ImgRect));
                // neg slope
                list.Add(new RowRangeData(StartY + (frac.Numerator * i), r, ImgRect));
            }
            return list;
        }
    }
}
