using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Astronomy
{
    public static class Events
    {
        public static JulianDay Easter(int Year)
        {
            if (Year <= 1582)
            {
                int a = Year % 4;
                int b = Year % 7;
                int c = Year % 19;
                int d = ((19 * c) + 15) % 30;
                int e = ((2 * a) + (4 * b) - d + 34) % 7;
                double f = (d + e + 114) / 31;
                int g = (d + e + 114) % 31;
                /* Debug information */
                Debug.WriteLine("a\t= " + a);
                Debug.WriteLine("b\t= " + b);
                Debug.WriteLine("c\t= " + c);
                Debug.WriteLine("d\t= " + d);
                Debug.WriteLine("e\t= " + e);
                Debug.WriteLine("f\t= " + f);
                Debug.WriteLine("g\t= " + g);
                return new JulianDay(Year, (int)f, g + 1);
            }
            else
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
                Debug.WriteLine("a\t= " + a);
                Debug.WriteLine("b\t= " + b);
                Debug.WriteLine("c\t= " + c);
                Debug.WriteLine("d\t= " + d);
                Debug.WriteLine("e\t= " + e);
                Debug.WriteLine("f\t= " + f);
                Debug.WriteLine("g\t= " + g);
                Debug.WriteLine("h\t= " + h);
                Debug.WriteLine("i\t= " + i);
                Debug.WriteLine("k\t= " + k);
                Debug.WriteLine("l\t= " + l);
                Debug.WriteLine("m\t= " + m);
                Debug.WriteLine("n\t= " + n);
                Debug.WriteLine("p\t= " + p);
                return new JulianDay(Year, (int)n, p + 1);
            }
        }

        public static JulianDay Pesach(int Year)
        {
            if (Year < -3761)
            { throw new ArgumentOutOfRangeException("Year", "The Hebrew Calendar is not valid prior to 3761 BCE"); }
            int X = Year;
            int month = 3;
            int C = AstroMath.Int(X / 100);
            int S = AstroMath.Int((C * 3 - 5) / 4);
            int A = X + 3760;
            int a = AstroMath.Mod(X * 12 + 12, 19);
            int b = AstroMath.Mod(X, 4);

            if (X < 1583)
            { S = 0; }

            double Q = -1.904412361576 + 1.554241796621 * a + 0.25 * b - 0.003177784022 * X + S;
            int j = AstroMath.Mod(AstroMath.Int(Q) + 3 * X + 5 * b + 2 - S, 7);
            double r = Q - AstroMath.Int(Q);
            int D;

            if (j == 2 || j == 4 || j == 6)
            { D = AstroMath.Int(Q) + 23; }
            else if (j == 1 && a > 6 && r >= 0.632870370)
            { D = AstroMath.Int(Q) + 24; }
            else if (j == 0 && a > 11 && r >= 0.897723765)
            { D = AstroMath.Int(Q) + 23; }
            else
            { D = AstroMath.Int(Q) + 22; }

            if (D > 31)
            {
                month = month + 1;
                D = D - 31;
            }
            /* Debug information */
            Debug.WriteLine("X\t= " + X);
            Debug.WriteLine("C\t= " + C);
            Debug.WriteLine("S\t= " + S);
            Debug.WriteLine("A\t= " + A);
            Debug.WriteLine("a\t= " + a);
            Debug.WriteLine("b\t= " + b);
            Debug.WriteLine("Q\t= " + Q);
            Debug.WriteLine("j\t= " + j);
            Debug.WriteLine("r\t= " + r);
            Debug.WriteLine("D\t= " + D);
            return new JulianDay(Year, month, D);
        }
    }
}
