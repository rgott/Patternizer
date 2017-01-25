using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patternizer
{
    public class BasicSVG
    {
        StringBuilder builder = new StringBuilder();
        public BasicSVG(Rectangle rect)
        {
            // create header
            builder
                .Append("<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:cc=\"http://web.resource.org/cc/\" viewBox = \"")
                .Append(rect.Left)
                .Append(" ")
                .Append(rect.Top)
                .Append(" ")
                .Append(rect.Width)
                .Append(" ")
                .Append(rect.Height)
                .AppendLine("\">");
        }

        public override string ToString()
        {
            return builder.ToString();
        }

        public void endInit()
        {
            builder.Append("</svg>");
        }

        public void addPoint(Color color,params System.Windows.Point[] points)
        {
            // initiate polygon
            builder.Append("<polygon points=\"");
            if(points.Length > 0)
            {
                builder
                    .Append(points[0].X)
                    .Append(",")
                    .Append(points[0].Y);
            }

            for (int i = 1; i < points.Length; i++)
            {
                builder
                    .Append(" ")
                    .Append(points[i].X)
                    .Append(",")
                    .Append(points[i].Y);

            }
            builder.Append("\"");// end points

            builder
                .Append(" style=\"fill:")
                .Append(HexConverter(color))
                .Append("\"");

            // TODO: add other attributes
            builder.AppendLine("></polygon>");// end polygon
        }
        private static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        /*
         * <svg viewBox = "0 0 500 500">   
                <triangle></triangle>
                <polygon points="195,140 140,210" style="fill:red;"></polygon>
                <polygon points="295,140 345,210 240,210" style="fill:red;"></polygon>
                <polygon points="395,140 445,210 340,210" style="fill:red;"></polygon>
                <polygon points="495,140 545,210 440,210" style="fill:red;"></polygon>
            </svg>
        */
    }
}
