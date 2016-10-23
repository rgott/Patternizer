using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patternizer
{
    class SpecialFraction
    {
        public int Numerator { get; private set; }
        public int Denominator { get; private set; }

        // TODO: make 2/3 != 1/1
        public SpecialFraction(int numerator,int denominator)
        {
            int wholeValue = numerator / denominator;
            if(wholeValue == 0)
            {
                wholeValue = denominator / numerator;
                denominator = wholeValue;
                numerator = 1;
            }
            else
            {
                numerator = wholeValue;
                denominator = 1;
            }


            if (numerator == 0)
                numerator = 1;
            if (denominator == 0)
                denominator = 1;

            Numerator = numerator;
            Denominator = denominator;
        }

        public void simplify()
        {
            if (Numerator % 2 == 0 && Denominator % 2 == 0)
            {
                Numerator /= 2;
                Denominator /= 2;
                simplify();
            }
            return;
        }
    }
}
