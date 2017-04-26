using System.Drawing;

namespace Patternizer
{
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
