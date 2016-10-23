using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patternizer
{
    public class RowRangeData : Range
    {
        public int row { get; set; }

        public RowRangeData(int row, Range columns) : base(columns.From,columns.To)
        {
            this.row = row;
        }
        public RowRangeData(int row, Range columns, Rectangle rect) : this(row,columns)
        {
            cutToBounds(rect);
        }

        public new RowRangeData cutToBounds(Rectangle rect)
        {
            base.cutToBounds(rect);
            if(row >= rect.Bottom)
                row = rect.Bottom - 1;
            else if(row < rect.Top)
            {
                row = 0;
            }
            return this;
        }
    }




    [DebuggerDisplay("{From} - {To}")]
    /// <summary>
    /// Assuming [From] Less than [To]
    /// </summary>
    public class Range
    {
        public int From { get; set; }
        public int To { get; set; }

        public int Distance { get { return (int)Math.Abs(To - From); } }
        public Range cutToBounds(Rectangle rect)
        {
            if (From < rect.X)
            {
                From = 0;
            }
            if (To > rect.Width)
            {
                To = rect.Width;
            }
            return this;
        }

        public override string ToString()
        {
            return From + " - " + To;
        }

        public bool isZeroRange()
        {
            return To == 0;
        }
        public Range(int From, int To)
        {
            this.From = From;
            this.To = To;
        }
    }
}
