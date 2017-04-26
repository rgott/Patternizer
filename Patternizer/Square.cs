using System;
using System.Collections.Generic;
using System.Drawing;

namespace Patternizer
{

    public class Square : RegionFill
    {
        public Square(Rectangle ImgRect,int StartX, int StartY, int Width, int Height) 
            : base(ImgRect,StartX, StartY, Width, Height)
        {
        }

        protected override List<RowRangeData> getIndexArray(Rectangle ImgRect,int StartX, int StartY, int Width, int Height)
        {
            var list = new List<RowRangeData>();
            for (int i = 0; i < Height; i++)
                list.Add(new RowRangeData(StartY + i, new Range(StartX, StartX + Width)).cutToBounds(ImgRect));
            return list;
        }
    }
}
