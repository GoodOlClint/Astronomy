using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Astronomy
{
    public class IslamicCalendar
    {
        public int Year { get; protected set; }
        public int Month { get; protected set; }
        public int Day { get; protected set; }

        public IslamicCalendar(int Year, int Month, int Day)
        {
            this.Year = Year;
            this.Month = Month;
            this.Day = Day;
        }

        public JulianDay ToJulianDay()
        {
            int N = this.Day + INT(29.5001 * (this.Month - 1) + 0.99);
            int Q = INT(this.Year / 30);
            int R = this.Year % 30;
            int A = INT(((11 * R) + 3) / 30);
            int W = (404 * Q) + (354 * R) + 208 + A;
            int Q1 = INT(W / 1461);
            int Q2 = W % 1461;
            int G = 621 + 4 * (INT((7 * Q) + Q1));
            int K = INT(Q2 / 365.2422);
            int E = INT(365.2422 * K);
            int J = Q2 - E + N - 1;
            int X = G + K;

            if ((J > 366) && (X % 4 == 0))
            {
                J = J - 366;
                X = X + 1;
            }
            if ((J > 354) && (X % 4 > 0))
            {
                J = J - 365;
                X = X + 1;
            }

            double JD = INT(365.25 * (X - 1)) + 1721423 + J;

            return new JulianDay(JD);
        }

        public static IslamicCalendar FromJulianDay(JulianDay JD)
        {
            int X = JD.Year;
            int M = JD.Month;
            int D = JD.Day;
            if (M < 3)
            {
                X = X - 1;
                M = M + 12;
            }

            int α = INT(X / 100);
            int β = 2 - α + INT(α / 4);

            int b = INT(365.25 * X) + INT(30.6001 * (M + 1)) + D + 1722519 + β;
            int c = INT((b - 122.1) / 365.25);
            int d = INT(365.25 * c);
            int e = INT((b - d) / 30.6001);

            D = b - d - INT(30.6001 * e);
            if (e < 14)
            { M = e - 1; }
            else
            { M = e - 13; }

            X = c - 4716;

            int W = 2;
            if (X % 4 == 0)
            { W = 1; }
            int N = INT((275 * M) / 9) - W * INT((M + 9) / 12) + D - 30;
            int A = X - 623;
            int B = INT(A / 4);
            int C = A % 4;
            double C1 = 365.2501 * C;
            int C2 = INT(C1);

            if (C1 - C2 > 0.5)
            { C2 = C2 + 1; }

            int D1 = (1461 * B) + 170 + C2;

            int Q = INT(D1 / 10631);
            int R = D1 % 10631;
            int J = INT(R / 354);
            int K = R % 354;

            int O = INT(((11 * J) + 14) / 30);

            int H = (30 * Q) + J + 1;
            int JJ = K - O + N - 1;


            int CL = H % 30;
            int DL = ((11 * CL) + 3) % 30;
            if (DL < 19)
            {
                JJ = JJ - 354;
                H = H + 1;
            }

            if (DL > 18)
            {
                JJ = JJ - 355;
                H = H + 1;
            }

            int S = INT((JJ - 1) / 29.5);

            int m = 1 + S;
            d = INT(JJ - (29.5 * S));

            if (JJ == 355)
            {
                m = 12;
                d = 30;
            }

            return new IslamicCalendar(H, m, d);
        }

        private static int INT(double input)
        { return Convert.ToInt32(Math.Floor(input)); }
    }
}
