using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Astronomy
{
    public static class AstroMath
    {
        /// <summary>
        /// Convert between degrees and radians.
        /// </summary>
        public const double DegRad = Math.PI / 180.0;

        /// <summary>
        /// Convert between hours and radians.
        /// </summary>
        public const double HourRad = 15.0 * DegRad;


        public static double Sin(double angle)
        { return Math.Sin(angle * DegRad); }

        public static double Cos(double angle)
        { return Math.Cos(angle * DegRad); }

        public static double Mod(double a, double b)
        {
            double result = a % b;
            if (result < 0)
            { result += b; }
            return result;
        }
        
        public static int Mod(int a, int b)
        {
            return (int)Mod((double)a, (double)b);
        }

        public static int Int(double input)
        { return Convert.ToInt32(Math.Floor(input)); }
    }
}
