using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Astronomy
{
    public class LunarPhases
    {
        private static IList<double>[] PlanetaryArguments { get; set; }
        private static double[] AdditionalCorrections { get; set; }

        static LunarPhases()
        {
            PlanetaryArguments = new List<double>[14] {
                                        new List<double> {299.77,  0.107408, 0.009173},
                                        new List<double> {251.88,  0.016321,   0},
                                        new List<double> {251.83, 26.651886,   0},
                                        new List<double> {349.42, 36.412478,   0},
                                        new List<double> { 84.66, 18.206239,   0},
                                        new List<double> {141.74, 53.303771,   0},
                                        new List<double> {207.14,  2.453732,   0},
                                        new List<double> {154.84,  7.306860,   0},
                                        new List<double> { 34.52, 27.261239,   0},
                                        new List<double> {207.19,  0.121824,   0},
                                        new List<double> {291.34,  1.844379,   0},
                                        new List<double> {161.72, 24.198154,   0},
                                        new List<double> {239.56, 25.513099,   0},
                                        new List<double> {331.55,  3.592518,   0}};
            AdditionalCorrections = new double[14]{
                0.000325,
                0.000165,
                0.000164,
                0.000126,
                0.000110,
                0.000062,
                0.000060,
                0.000056,
                0.000047,
                0.000042,
                0.000040,
                0.000037,
                0.000035,
                0.000023};
        }

        public static JulianDay FindPhase(int year, int month, int day, MoonPhases phase)
        {
            var jd = new JulianDay(year, month, day);
            double length = 365;
            if (jd.IsLeapYear)
            { length = 366; }

            double Y = Math.Round(year + ((double)jd.DayOfYear / length), 2);
            double k = ((Y - 2000) * 12.3685);
            
            switch (phase)
            {
                case MoonPhases.New:
                    k = Math.Round(k);
                    break;
                case MoonPhases.FirstQuarter:
                    if (k < 0)
                    { k = Math.Round(k) - 0.25; }
                    else
                    { k = Math.Floor(k) + 0.25; }
                    break;
                case MoonPhases.Full:
                    if (k < 0)
                    { k = Math.Round(k) - 0.50; }
                    else
                    { k = Math.Floor(k) + 0.50; }
                    break;
                case MoonPhases.LastQuarter:
                    if (k < 0)
                    { k = Math.Round(k) - 0.75; }
                    else
                    { k = Math.Floor(k) + 0.75; }
                    break;
            }

            Debug.WriteLine("k\t=\t" + Math.Round(k, 2));

            double T = k / 1236.85;
            double T2 = Math.Pow(T, 2);
            double T3 = Math.Pow(T, 3);
            double T4 = Math.Pow(T, 4);

            Debug.WriteLine("T\t=\t" + Math.Round(T, 5));

            double JDE = 2451550.09766 + 29.530588861 * k
            + 0.00015437 * T2
            - 0.000000150 * T3
            + 0.00000000073 * T4;

            Debug.WriteLine("JDE\t=\t" + Math.Round(JDE, 5));

            //Corrections

            double E = 1 - 0.002516 * T - 0.0000074 * T2;

            double M = 2.5534 + 29.10535670 * k
                        - 0.0000014 * T2
                        - 0.00000011 * T3;

            Debug.Write("M\t=\t" + Math.Round(M, 4));
            M = AstroMath.Mod(M, 360);
            Debug.WriteLine("\t=\t" + Math.Round(M, 4));

            double Mp = 201.5643 + (385.81693528 * k)
                        + (0.0107582 * T2)
                        + (0.00001238 * T3)
                        - (0.000000058 * T4);


            Debug.Write("M′\t=\t" + Math.Round(Mp, 4));
            Mp = AstroMath.Mod(Mp, 360);
            Debug.WriteLine("\t=\t" + Math.Round(Mp, 4));

            double F = 160.7180 + 390.67050284 * k
                        - 0.0016118 * T2
                        - 0.00000227 * T3
                        + 0.000000011 * T4;

            Debug.Write("F\t=\t" + Math.Round(F, 4));
            F = AstroMath.Mod(F, 360);
            Debug.WriteLine("\t=\t" + Math.Round(F, 4));

            double Ω = 124.7746 - (1.56375588 * k)
                        + (0.0020672 * T2)
                        + (0.00000215 * T3);

            Debug.Write("Ω\t=\t" + Math.Round(Ω, 4));
            Ω = AstroMath.Mod(Ω, 360);
            Debug.WriteLine("\t=\t" + Math.Round(Ω, 4));

            double correction = 0;
            //switch (phase)
            //{
            //    case MoonPhases.Full:
            //        correction = FullMoonCorrections(E, M, Mp, F, Ω);
            //        break;
            //}

            //PlanetaryArguments
            double additionalCorrection = 0;
            for (int i = 0; i <= 13; i++)
            {
                IList<double> S = PlanetaryArguments[i];
                additionalCorrection += AdditionalCorrections[i] * AstroMath.Sin(S[0] + (S[1] * k) - (S[2] * T2));
            }
            
            Debug.WriteLine("Additional Corrections:\t" + Math.Round(additionalCorrection, 5));
            JDE = JDE + correction + additionalCorrection;

            return new JulianDay(JDE);
        }

        private static double FullMoonCorrections(double E, double M, double Mp, double F, double Ω)
        {
            throw new NotImplementedException();
            return Correction(-0.40614, Mp)
                    + Correction(0.17302 * E, M)
                    + Correction(0.01614, 2 * Mp)
                    + Correction(0.01043, 2 * F);
        }
        private static double Correction(double Term1, double Term2)
        { return Term1 * AstroMath.Sin(Term2); }
    }

    public enum MoonPhases
    {
        New,
        FirstQuarter,
        Full,
        LastQuarter
    }
}
