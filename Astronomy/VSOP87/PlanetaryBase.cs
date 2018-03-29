using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
namespace Astronomy.VSOP87
{
    public abstract class PlanetaryBase
    {
        protected ConcurrentBag<PeriodicTerm> PeriodicTerms = new ConcurrentBag<PeriodicTerm>();
        protected ConcurrentBag<PeriodicNutationTerm> NutationTerms = new ConcurrentBag<PeriodicNutationTerm>();
        protected ConcurrentBag<PeriodicNutationTerm> NutationCorrectionsTerms = new ConcurrentBag<PeriodicNutationTerm>();
        public abstract string PlanetName { get; }

        public PlanetaryBase()
        {
            this.BuildLatitude();
            this.BuildLongitude();
            this.BuildRadiusVector();
            this.BuildNutationTables();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Planet"></param>
        /// <param name="JDE">Julian Ephemeris Day</param>
        /// <param name="acuracyLevel">the Level of accuracy required for the given planet</param>
        /// <returns></returns>
        public double CalculateHelocentricLongitude(double JDE, int acuracyLevel)
        {
            int count = (from PeriodicTerm term in PeriodicTerms
                         where term.Series.StartsWith("L")
                         select term.Series).Distinct().Count() - 1;
            if (acuracyLevel < 0 || acuracyLevel > count)
            { throw new IndexOutOfRangeException("acuracyLevel must be greater then zero and less then " + count + " for the Helocentric Longitude of " + PlanetName); }
            return AstroMath.Rev(AstroMath.RadDeg * (QueryDatabase(JDE, acuracyLevel, SeriesType.L)));
        }
        /// <summary>
        /// Corrects the Helocentric Longitude <paramref name="L"/> and the Helocentric Latitude <paramref name="B"/> to the FK5 System
        /// </summary>
        /// <param name="L">The helocentric Longitude, obtained through CalculateHelocentricLongitude</param>
        /// <param name="B">The helocentric Latitude, obtained through CalculateHelocentricLatitude</param>
        /// <param name="T">The time in centuries from 2000.0</param>
        /// <param name="dL">The delta of correction to <paramref name="L"/></param>
        /// <param name="dB">The delta of correction to <paramref name="B"/></param>
        public void CorrectLB(double L, double B, double JDE, out double dL, out double dB)
        {
            double T = 10 * ((JDE - 2451545.0) / 365250);
            double Lp = L - AstroMath.DegRad * ((1.397 * T) - (0.00031 * T));
            dL = (-0.09033 + 0.03916 * (AstroMath.Cos(Lp) + AstroMath.Sin(Lp)) * AstroMath.Tan(B));
            dB = 0.03916 * (AstroMath.Cos(Lp) - AstroMath.Sin(Lp));
        }

        public double CalculateHelocentricLatitude(double JDE, int acuracyLevel)
        {
            int count = (from PeriodicTerm term in PeriodicTerms
                         where term.Series.StartsWith("B")
                         select term).Distinct().Count() - 1;
            if (acuracyLevel < 0 || acuracyLevel > count)
            { throw new IndexOutOfRangeException("acuracyLevel must be greater then zero and less then " + count + " for the Helocentric Latitude of " + PlanetName); }
            return AstroMath.Rev(AstroMath.RadDeg * (QueryDatabase(JDE, acuracyLevel, SeriesType.B)));
        }

        public double CalculateRadiusVector(double JDE, int acuracyLevel)
        {
            int count = (from PeriodicTerm term in PeriodicTerms
                         where term.Series.StartsWith("R")
                         select term).Distinct().Count() - 1;
            if (acuracyLevel < 0 || acuracyLevel > count)
            { throw new IndexOutOfRangeException("acuracyLevel must be greater then zero and less then " + count + " for the Radius Vector of " + PlanetName); }
            return QueryDatabase(JDE, acuracyLevel, SeriesType.R);
        }

        public double CalculateNutation(double JDE)
        {
            double dP = 0;
            double dE = 0;
            double D, M, Mp, F, O, T;
            T = (JDE - 2451545.0) / 36525;
            T = Math.Round(T, 12);
            D = 297.850363055 + (307.11148 * T) + AstroMath.Rev(444960 * T) - (0.001914166667 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 189473.6842);
            M = 357.527723333 + (359.05034 * T) + AstroMath.Rev(35640 * T) - (0.0001602777778 * Math.Pow(T, 2)) - (Math.Pow(T, 3) / 300000);
            Mp = 134.962981389 + (198.867398056 * T) + AstroMath.Rev(477000 * T) + (0.008697222222 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 56250);
            F = 93.271910277 + (82.017538055 * T) + AstroMath.Rev(483120 * T) - (0.0036825 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 327272.7273);
            O = 125.044522222 - (134.136260833 * T) - AstroMath.Rev(1800 * T) + (0.002070833333 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 450000);
            var query = from PeriodicNutationTerm term in NutationTerms
                        select term;
            foreach (PeriodicNutationTerm PNT in query)
            {
                double Sine, Cos;
                Sine = AstroMath.Sin(PNT.D * D + PNT.M * M + PNT.Mp * Mp + PNT.F * F + PNT.O * O);
                Cos = AstroMath.Cos((PNT.D * D) + (PNT.M * M) + (PNT.Mp * Mp) + (PNT.F * F) + (PNT.O * O));
                dP += (PNT.Coef1 + T * PNT.Coef2) * Sine;
                dE += (PNT.Coef3 + T * PNT.Coef4) * Cos;
            }
            return Math.Round(3600000 * (dP / 36000000)) / 1000;
        }
        protected double QueryDatabase(double JDE, int acuracyLevel, SeriesType series)
        {
            List<long> lst = new List<long>();
            double T = (JDE - 2451545.0) / 365250;
            double ret = 0.0;
            for (int i = 0; i <= acuracyLevel; i++)
            {
                lst.Add(0);
                var query = from PeriodicTerm term in PeriodicTerms
                            where term.Series == (series.ToString() + i)
                            select term;
                foreach (PeriodicTerm Term in query) { lst[i] += (long)Term.Calculate(T); }
                if (i == 0)
                { ret = lst[i]; }
                else if (i == 1)
                { ret += lst[i] * T; }
                else
                { ret += lst[i] * Math.Pow(T, i); }

            }
            return ret / Math.Pow(10, 8);
        }

        protected abstract void BuildLatitude();
        protected abstract void BuildLongitude();
        protected abstract void BuildRadiusVector();

        protected enum SeriesType
        {
            L,
            B,
            R
        }
        protected class PeriodicTerm
        {
            private Dictionary<string, double> Values = new Dictionary<string, double>();
            private string series;
            public PeriodicTerm() { }
            public PeriodicTerm(string Series, double A, double B, double C) { this.series = Series; this.A = A; this.B = B; this.C = C; }
            public double Calculate(double T) { return AstroMath.CalculatePeriodicTerms(A * Math.Pow(10, 8), B, C, T); }
            public double A { get { return this.Values["A"]; } set { this.Values["A"] = value; } }
            public double B { get { return this.Values["B"]; } set { this.Values["B"] = value; } }
            public double C { get { return this.Values["C"]; } set { this.Values["C"] = value; } }
            public string Series { get { return this.series; } set { this.series = value; } }
        }

        protected class PeriodicNutationTerm
        {
            private Dictionary<string, double> Values = new Dictionary<string, double>();
            public PeriodicNutationTerm() { }
            public PeriodicNutationTerm(double M, double Mp, double F, double D, double O, double Coef1, double Coef2, double Coef3, double Coef4)
            { this.D = D; this.M = M; this.Mp = Mp; this.F = F; this.O = O; this.Coef1 = Coef1; this.Coef2 = Coef2; this.Coef3 = Coef3; this.Coef4 = Coef4; }
            public double D { get { return this.Values["D"]; } set { this.Values["D"] = value; } }
            public double M { get { return this.Values["M"]; } set { this.Values["M"] = value; } }
            public double Mp { get { return this.Values["Mp"]; } set { this.Values["Mp"] = value; } }
            public double F { get { return this.Values["F"]; } set { this.Values["F"] = value; } }
            public double O { get { return this.Values["O"]; } set { this.Values["O"] = value; } }
            public double Coef1 { get { return this.Values["Coef1"]; } set { this.Values["Coef1"] = value; } }
            public double Coef2 { get { return this.Values["Coef2"]; } set { this.Values["Coef2"] = value; } }
            public double Coef3 { get { return this.Values["Coef3"]; } set { this.Values["Coef3"] = value; } }
            public double Coef4 { get { return this.Values["Coef4"]; } set { this.Values["Coef4"] = value; } }
            public double CalculateLongitude(double JDE)
            {
                double T = (JDE - 2451545.0) / 36525;
                double D, M, Mp, F, O;
                D = AstroMath.Rev(297.85036 + (445267.111480 * T) - (0.0019142 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 189474));
                M = AstroMath.Rev(357.52772 + (35999.050340 * T) - (0.0001603 * Math.Pow(T, 2)) - (Math.Pow(T, 3) / 300000));
                Mp = AstroMath.Rev(134.96298 + (477198.867389 * T) + (0.0087972 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 56270));
                F = AstroMath.Rev(93.27191 + (483202.017538 * T) - (0.0036825 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 327270));
                O = AstroMath.Rev(125.04452 - (1934.136261 * T) + (0.0020708 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 450000));
                double Sine = AstroMath.Sin((this.D * D) + (this.M * M) + (this.Mp * Mp) + (this.F * F) + (this.O * O));
                return (Coef1 + Coef2 * T) * Sine;
            }
            public double CalculateLongitudeCoefficients(double T, double D, double M, double Mp, double F, double O)
            {
                //double Sine = AstroMath.Sin((this.D * D) + (this.M * M) + (this.Mp * Mp) + (this.F * F) + (this.O * O));
                return (Coef1 + Coef2 * T);
            }

            public double CalculateObliquity(double JDE)
            {
                double T = (JDE - 2451545.0) / 365250;
                double D, M, Mp, F, O;
                D = AstroMath.Rev(297.85036 + (445267.111480 * T) - (0.0019142 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 189474));
                M = AstroMath.Rev(357.52772 + (35999.050340 * T) - (0.0001603 * Math.Pow(T, 2)) - (Math.Pow(T, 3) / 300000));
                Mp = AstroMath.Rev(134.96298 + (477198.867389 * T) + (0.0087972 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 56270));
                F = AstroMath.Rev(93.27191 + (483202.017538 * T) - (0.0036825 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 327270));
                O = AstroMath.Rev(125.04452 - (1934.136261 * T) + (0.0020708 * Math.Pow(T, 2)) + (Math.Pow(T, 3) / 450000));
                double Cos = AstroMath.Cos((this.D * D) + (this.M * M) + (this.Mp * Mp) + (this.F * F) + (this.O * O));
                return (0.0001 * (Coef3 + Coef4 * T)) * Cos;
            }
        }
        protected void BuildNutationTables()
        {
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 0, 0, 1, -171996, -174.2, 92025, 8.9));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, -2, 2, -13187, -1.6, 5736, -3.1));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, 0, 2, -2274, -0.2, 977, -0.5));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 0, 0, 2, 2062, 0.2, -895, 0.5));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, 0, 0, 0, 1426, -3.4, 54, -0.1));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 0, 0, 0, 712, 0.1, -7, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, 2, -2, 2, -517, 1.2, 224, -0.6));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, 0, 1, -386, -0.4, 200, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 2, 0, 2, -301, 0, 129, -0.1));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, -1, 2, -2, 2, 217, -0.5, -95, 0.3));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 0, -2, 0, -158, 0, -1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, -2, 1, 129, 0.1, -70, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, 0, 2, 0, 2, 123, 0, -53, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 0, 0, 1, 63, 0.1, -33, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 0, 2, 0, 63, 0, -2, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, 0, 2, 2, 2, -59, 0, 26, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, 0, 0, 0, 1, -58, -0.1, 32, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 2, 0, 1, -51, 0, 27, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, 0, -2, 0, 48, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-2, 0, 2, 0, 1, 46, 0, -24, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, 2, 2, -38, 0, 16, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, 2, 0, 2, -31, 0, 13, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, 0, 0, 0, 29, 0, -1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 2, -2, 2, 29, 0, -12, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, 0, 0, 26, 0, -1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, -2, 0, -22, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, 0, 2, 0, 1, 21, 0, -10, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 2, 0, 0, 0, 17, -0.1, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 2, 2, -2, 2, -16, 0.1, 7, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, 0, 0, 2, 1, 16, 0, -8, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, 0, 0, 1, -15, 0, 9, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 0, -2, 1, -13, 0, 7, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, -1, 0, 0, 1, -12, 0, 6, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, -2, 0, 0, 11, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, 0, 2, 2, 1, -10, 0, 5, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 2, 2, 2, -8, 0, 3, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, -1, 2, 0, 2, -7, 0, 3, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, 2, 1, -7, 0, 3, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 1, 0, -2, 0, -7, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, 2, 0, 2, 7, 0, -3, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, 1, -6, 0, 3, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 0, 2, 1, -6, 0, 3, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, 2, -2, 2, 6, 0, -3, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 0, 2, 0, 6, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 2, -2, 1, 6, 0, -3, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 0, -2, 1, -5, 0, 3, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, -1, 2, -2, 1, -5, 0, 3, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, 2, 0, 1, -5, 0, 3, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, -1, 0, 0, 0, 5, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 0, -1, 0, -4, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 0, 1, 0, -4, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, 0, -2, 0, -4, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, -2, 0, 0, 4, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, 0, -2, 1, 4, 0, -2, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, 2, -2, 1, 4, 0, -2, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 1, 0, 0, 0, -3, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, -1, 0, -1, 0, -3, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, -1, 2, 2, 2, -3, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, -1, 2, 2, 2, -3, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, -1, 2, 0, 2, -3, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(3, 0, 2, 0, 2, -3, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-2, 0, 2, 0, 2, -3, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 2, 0, 0, 3, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, 0, 2, 4, 2, -2, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 0, 0, 2, -2, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, 0, 2, -2, 1, -2, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, -2, 2, -2, 1, -2, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-2, 0, 0, 0, 1, -2, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, 0, 0, 1, 2, 0, -1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(3, 0, 0, 0, 0, 2, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 1, 2, 0, 2, 2, 0, -1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, 1, 2, 2, 0, -1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 0, 2, 1, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 2, 2, 1, -1, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 1, 0, -2, 1, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, 0, 2, 0, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, 2, -2, 0, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, -2, 2, 0, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, -2, 2, 0, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, -2, -2, 0, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 2, -2, 0, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 0, 0, -4, 0, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, 0, -4, 0, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, 4, 2, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 2, -1, 2, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-2, 0, 2, 4, 2, -1, 0, 1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, 2, 2, 2, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, -1, 2, 0, 1, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, -2, 0, 1, -1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, 4, -2, 2, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, 0, 0, 2, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, 1, 2, -2, 2, 1, 0, -1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(3, 0, 2, -2, 2, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-2, 0, 2, 2, 2, 1, 0, -1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, 0, 0, 0, 2, 1, 0, -1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 0, -2, 2, 1, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, 2, 0, 1, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, 0, 4, 0, 2, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 1, 0, -2, 0, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, 0, 2, 0, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, 2, -2, 1, 1, 0, -1, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(2, 0, -2, 0, 1, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(1, -1, 0, -2, 0, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, 0, 0, 1, 1, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(-1, -1, 0, 2, 1, 1, 0, 0, 0));
            this.NutationTerms.Add(new PeriodicNutationTerm(0, 1, 0, 1, 0, 1, 0, 0, 0));
            this.NutationCorrectionsTerms.Add(new PeriodicNutationTerm(0, 0, 0, 0, 1, -725, 417, 414, 224));
            this.NutationCorrectionsTerms.Add(new PeriodicNutationTerm(0, 1, 0, 0, 0, 523, 61, 208, -24));
            this.NutationCorrectionsTerms.Add(new PeriodicNutationTerm(0, 0, 2, -2, 2, 102, -118, -41, -47));
            this.NutationCorrectionsTerms.Add(new PeriodicNutationTerm(0, 0, 2, 0, 2, -81, 0, 32, 0));
        }
    }
}