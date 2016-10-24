using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patternizer
{

    public class Square : RegionFill
    {
        public Square(Bitmap Img,int StartX, int StartY, int Width, int Height) : base(Img,StartX, StartY, Width, Height)
        {
        }

        public override Color FillRegion()
        {
            foreach (RowRangeData item in Fill)
            {
                for (int i = item.From; i < item.To; i++)
                {
                    Img.SetPixel(i, item.row, AvgColor);
                }
            }
            return AvgColor;
            //Img.Save("SDFK.bmp");// Debugging
        }

        protected override List<RowRangeData> getIndexArray(int StartX, int StartY, int Width, int Height)
        {
            var list = new List<RowRangeData>();
            for (int i = 0; i < Height; i++)
                list.Add(new RowRangeData(StartY + i, new Range(StartX, StartX + Width)).cutToBounds(new Rectangle(0,0,Img.Width,Img.Height)));
            return list;
        }
    }
}
