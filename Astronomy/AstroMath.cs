using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Astronomy
{
    public static class AstroMath
    {
        /// <summary>
        /// Normalize an angle between 0 and 360 degrees
        /// </summary>
        public static double Rev(double x) { return x - Math.Floor(x / 360.0) * 360.0; }

        /// <summary>
        /// Normalize a number between 0 and <paramref name="ammt"/>
        /// </summary>
        public static int Rev(int x, int ammt) { return x - (int)Math.Floor((double)x / ammt) * ammt; }

        /// <summary>
        /// Convert from degrees to radians.
        /// </summary>
        public const double DegRad = Math.PI / 180.0;

        /// <summary>
        /// Convert from radians to degrees;
        /// </summary>
        public const double RadDeg = 180.0 / Math.PI;

        /// <summary>
        /// Convert between hours and radians.
        /// </summary>
        public const double HourRad = 15.0 * DegRad;


        #region Trig. functions in degrees
        /// <summary>
        /// Returns the sine of a specified angle
        /// </summary>
        /// <param name="a">an angle, measured in degrees</param>
        /// <returns>The sine of a. If a is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public static double Sin(double a) { return Math.Sin(a * DegRad); }
        /// <summary>
        /// Returns the cosine of the specified angle.
        /// </summary>
        /// <param name="d">An angle, measured in degrees</param>
        /// <returns>The cosine of d</returns>
        public static double Cos(double d) { return Math.Cos(d * DegRad); }
        /// <summary>
        /// Returns the tangent of the specified angle.
        /// </summary>
        /// <param name="a">An angle, measured in degrees</param>
        /// <returns>The tangent of a. If a is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN</returns>
        public static double Tan(double a) { return Math.Tan(a * DegRad); }
        /// <summary>
        /// Returns the angle whose sine is the specified number.
        /// </summary>
        /// <param name="d">A number representing a sine, where -1 ≤d≤ 1</param>
        /// <returns>An angle, θ, measured in degrees, such that -π/2 ≤θ≤π/2 -or- System.Double.NaN if d < -1 or d > 1</returns>
        public static double ASin(double d) { return RadDeg * Math.Asin(d); }
        /// <summary>
        /// Returns the angle whose cosine is the specified number.
        /// </summary>
        /// <param name="d">A number representing a cosine, where -1 ≤d≤ 1</param>
        /// <returns>An angle, θ, measured in degrees, such that 0 ≤θ≤π -or- System.Double.NaN if d < -1 or d > 1</returns>
        public static double ACos(double d) { return RadDeg * Math.Acos(d); }
        /// <summary>
        /// Returns the angle whose tangent is the specified number.
        /// </summary>
        /// <param name="d">A number representing a tangent</param>
        /// <returns>An angle, θ, measured in degrees, such that -π/2 ≤θ≤π/2.  -or- System.Double.NaN if d equals System.Double.NaN, -π/2 rounded to double precision (-1.5707963267949) if d equals System.Double.NegativeInfinity, or π/2 rounded to double precision (1.5707963267949) if d equals System.Double.PositiveInfinity</returns>
        public static double ATan(double d) { return RadDeg * Math.Atan(d); }
        //arctan in all four quadrants
        /// <summary>
        /// Returns the angle whose tangent is the quotient of two specified numbers.
        /// </summary>
        /// <param name="y">The y coordinate of a point</param>
        /// <param name="x">The x coordinate of a point</param>
        /// <returns>An angle, θ, measured in degrees, such that -π≤θ≤π, and tan(θ) = y / x, where (x, y) is a point in the Cartesian plane. Observe the following: For (x, y) in quadrant 1, 0 < θ < π/2.  For (x, y) in quadrant 2, π/2 < θ≤π.  For (x, y) in quadrant 3, -π < θ < -π/2.  For (x, y) in quadrant 4, -π/2 < θ < 0.  For points on the boundaries of the quadrants, the return value is the following: If y is 0 and x is not negative, θ = 0.  If y is 0 and x is negative, θ = π.  If y is positive and x is 0, θ = π/2.  If y is negative and x is 0, θ = -π/2.</returns>
        public static double ATan2(double y, double x) { return RadDeg * Math.Atan2(y, x); }
        #endregion

        /// <summary>
        /// A helper function 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="τ"></param>
        /// <returns><paramref name="A"/> Cos(<paramref name="B" />+<paramref name="C"/><paramref name="T"/>)</returns>
        public static double CalculatePeriodicTerms(double A, double B, double C, double τ) { return A * Math.Cos(B + (C * τ)); }

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
