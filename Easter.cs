using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Astronomy
{
    public class Easter
    {
        public JulianDay Date { get; protected set; }

        public Easter(int Year)
        {
            if (Year <= 1582)
            { JulianEaster(Year); }
            else
            { GregorianEaster(Year); }
        }

        private void JulianEaster(int Year)
        {
            int a = Year % 4;
            int b = Year % 7;
            int c = Year % 19;
            int d = ((19 * c) + 15) % 30;
            int e = ((2 * a) + (4 * b) - d + 34) % 7;
            double f = (d + e + 114) / 31;
            int g = (d + e + 114) % 31;

            this.Date = new JulianDay(Year, (int)f, g + 1);
        }

        private void GregorianEaster(int Year)
        {
            int a = Year % 19;
            double b = Year / 100;
            int c = Year % 100;
            double d = b / 4;
            int e = (int)b % 4;
            double f = (b + 8) / 25;
            double g = (b - f + 1) / 3;
            int h = (int)((19 * a) + b - d - g + 15) % 30;
            double i = c / 4;
            int k = c % 4;
            int l = (int)(32 + (2 * 3) + (2 * i) - h - k) % 7;
            double m = (a + (11 * h) + (22 * l)) / 451;
            double n = (h + l - (7 * m) + 114) / 31;
            int p = (int)(h + l - (7 * m) + 114) % 31;

            this.Date = new JulianDay(Year, (int)n, p + 1);
        }
    }
}