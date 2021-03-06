﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Astronomy
{
    /// <summary>
    /// The JDN the number of whole and fractional days since Noon, 1st January -4712 UTC.
    /// </summary>
    public class JulianDay
    {
        #region Public Static Methods
        public static JulianDay Now()
        {
            return new JulianDay(DateTime.UtcNow);
        }
        #endregion

        #region Public Properties
        public int Year { get; protected set; }
        public int Month { get; protected set; }
        public int Day { get; protected set; }
        public int Hour { get; protected set; }
        public int Minute { get; protected set; }
        public int Second { get; protected set; }
        public int Millisecond { get; protected set; }

        public double JulianDayNumber { get; protected set; }

        public DayOfWeek DayOfWeek
        {
            get
            {
                int JD = INT(this.JulianDayNumber + 1.5);
                return (DayOfWeek)(JD % 7);
            }
        }

        public int DayOfYear
        {
            get
            {
                int K;
                if (this.IsLeapYear)
                { K = 1; }
                else
                { K = 2; }

                Debug.WriteLine("K\t= " + K);
                Debug.WriteLine("M\t= " + this.Month);
                Debug.WriteLine("D\t= " + this.Day);
                int N = INT((275 * this.Month) / 9) - K * INT((this.Month + 9) / 12) + this.Day - 30;
                Debug.WriteLine("N\t= " + N);

                return N;
            }
        }

        /// <summary>
        /// On the Julian Calendar a year divisible by 4 is a leap year
        /// On the Gregorian Calendar a year divisible by 4, except centuries (divisible by 100) not divisible by 400, is a leap year.
        /// </summary>
        public bool IsLeapYear
        {
            get
            {
                bool isCentury = (this.Year % 100 == 0);
                if (isCentury && this.Calendar == Astronomy.Calendar.Gregorian)
                { return ((this.Year % 400) == 0); }
                else
                { return ((this.Year % 4) == 0); }
            }

        }

        /// <summary>
        /// The Gregorian calendar reform moved changed the calendar from the Julian to Gregorian standard.
        /// This means that the day immediately after 4th October 1582 is 15th October 1582.
        /// </summary>
        public Calendar Calendar
        {
            get
            {
                if (this.JulianDayNumber != 0)
                {
                    if (this.JulianDayNumber < 2299161)
                    { return Astronomy.Calendar.Julian; }
                    else
                    { return Astronomy.Calendar.Gregorian; }
                }
                else
                {
                    if (this.Year > 1582)
                    { return Astronomy.Calendar.Gregorian; }
                    else if (this.Year < 1582)
                    { return Astronomy.Calendar.Julian; }
                    else if ((this.Year == 1582) && (this.Month < 10))
                    { return Astronomy.Calendar.Julian; }
                    else if ((this.Year == 1582) && (this.Month > 10))
                    { return Astronomy.Calendar.Gregorian; }
                    else if ((this.Year == 1582) && (this.Month == 10) && (this.Day < 5))
                    { return Astronomy.Calendar.Julian; }
                    else if ((this.Year == 1582) && (this.Month > 10) && (this.Day > 14))
                    { return Astronomy.Calendar.Gregorian; }
                    else
                    { throw new IndexOutOfRangeException("The dates October 5th - 14th 1582 are not valid"); }
                }
            }
        }
        #endregion

        public JulianDay AddDays(double Days)
        { return new JulianDay(this.JulianDayNumber + Days); }

        public JulianDay AddMinutes(double minutes)
        { return this.AddDays((minutes / 60) / 24); }

        public JulianDay AddSeconds(double seconds)
        { return this.AddMinutes((seconds / 60) / 60); }

        private int INT(double input)
        { return Convert.ToInt32(Math.Floor(input)); }

        #region Constructors
        public JulianDay()
        {
            this.Year = -4712;
            this.Month = 1;
            this.Day = 1;
            this.Hour = 12;
            this.JulianDayNumber = 0;
        }

        public JulianDay(DateTime dateTime)
            : this(2451544.5 + ((dateTime.Ticks - 630822816000000000) / 10000d / 1000d) / 60d / 60d / 24d)
        {
            //DateTime objects are always Gregorian Calendar based. Throw an error to prevent unexpected results.
            if (this.Calendar == Calendar.Julian)
            { throw new ArgumentOutOfRangeException("The System.DateTime class should not be use for dates prior to October 15th 1582. Invalid calculations will result."); }
        }

        public JulianDay(int Year, int Month, int Day)
            : this(Year, Month, Day, 0)
        { }

        public JulianDay(int Year, int Month, int Day, int Hour)
            : this(Year, Month, Day, Hour, 0)
        { }

        public JulianDay(int Year, int Month, int Day, int Hour, int Minute)
            : this(Year, Month, Day, Hour, Minute, 0)
        { }

        public JulianDay(int Year, int Month, int Day, int Hour, int Minute, int Second)
            : this(Year, Month, Day, Hour, Minute, Second, 0)
        { }

        public JulianDay(int Year, int Month, int Day, int Hour, int Minute, int Second, int Millisecond)
        {

            this.Year = Year;
            this.Month = Month;
            this.Day = Day;
            this.Hour = Hour;
            this.Minute = Minute;
            this.Second = Second;
            this.Millisecond = Millisecond;
            this.CalculateFromDate();
        }

        public JulianDay(double JDN)
        {
            this.JulianDayNumber = JDN;
            this.CalculateDate();
        }
        #endregion

        private void CalculateFromDate()
        {
            this.JulianDayNumber = 0;
            int y = this.Year;
            int m = this.Month;
            double d = this.Day;
            d += ((double)this.Hour / 24d);
            d += ((double)this.Minute / 60d) / 24d;
            d += ((double)this.Second / 60d) / 60d / 24d;
            d += ((double)this.Millisecond / 1000d) / 60d / 60d / 24d;
            if (m <= 2)
            {
                y = y - 1;
                m = m + 12;
            }
            int B = 0;
            if (this.Calendar == Astronomy.Calendar.Gregorian)
            {
                int A = INT(y / 100);
                B = 2 - A + INT(A / 4);
                Debug.WriteLine("A\t= " + A);
            }
            
            double JD = INT(365.25 * (y + 4716)) + INT(30.6001 * (m + 1)) + d + B - 1524.5;
            
            Debug.WriteLine("B\t= " + B);
            Debug.WriteLine("JD\t= " + JD);
            this.JulianDayNumber = JD;
        }

        private void CalculateDate()
        {
            double JD = this.JulianDayNumber + 0.5;
            int Z = (int)Math.Truncate(JD);
            double F = JD - Z;
            int A;
            if (Z < 2299161)
            { A = Z; }
            else
            {
                int a = INT((Z - 1867216.25) / 36524.25);
                A = Z + 1 + a - INT(a / 4);
                Debug.WriteLine("a\t= " + a);
            }
            int B = A + 1524;
            int C = INT((B - 122.1) / 365.25);
            int D = INT(365.25 * C);
            int E = INT((B - D) / 30.6001);

            this.Day = B - D - INT(30.6001 * E);

            if (E < 14)
            { this.Month = E - 1; }
            else
            { this.Month = E - 13; }

            if (this.Month > 2)
            { this.Year = C - 4716; }
            else
            { this.Year = C - 4715; }

            this.Hour = INT(F * 24);
            this.Minute = INT(((F * 24) - this.Hour) * 60);
            this.Second = INT(((((F * 24) - this.Hour) * 60) - this.Minute) * 60);
            this.Millisecond = INT(((((((F * 24) - this.Hour) * 60) - this.Minute) * 60) - this.Second) * 1000);

            Debug.WriteLine("A\t= " + A);
            Debug.WriteLine("B\t= " + B);
            Debug.WriteLine("C\t= " + C);
            Debug.WriteLine("D\t= " + D);
            Debug.WriteLine("E\t= " + E);
            Debug.WriteLine("Day\t= " + this.Day);
            Debug.WriteLine("Month\t= " + this.Month);
            Debug.WriteLine("Year\t= " + this.Year);

        }

        #region Mathematical Operators
        public static JulianDay operator +(JulianDay JDE1, JulianDay JDE2)
        { return new JulianDay(JDE1.JulianDayNumber + JDE2.JulianDayNumber); }

        //public static JulianDay operator +(JulianDay JDE1, double Days)
        //{ return new JulianDay(JDE1.JulianDayNumber + Days); }

        public static double operator +(JulianDay JDE1, double Days)
        { return JDE1.JulianDayNumber + Days; }

        public static JulianDay operator -(JulianDay JDE1, JulianDay JDE2)
        { return new JulianDay(JDE1.JulianDayNumber - JDE2.JulianDayNumber); }

        //public static JulianDay operator -(JulianDay JDE1, double Days)
        //{ return new JulianDay(JDE1.JulianDayNumber - Days); }

        public static double operator -(JulianDay JDE1, double Days)
        { return JDE1.JulianDayNumber - Days; }

        public static JulianDay operator *(JulianDay JDE1, JulianDay JDE2)
        { return new JulianDay(JDE1.JulianDayNumber * JDE2.JulianDayNumber); }

        public static JulianDay operator *(JulianDay JDE1, double Days)
        { return new JulianDay(JDE1.JulianDayNumber * Days); }

        public static JulianDay operator /(JulianDay JDE1, JulianDay JDE2)
        { return new JulianDay(JDE1.JulianDayNumber / JDE2.JulianDayNumber); }

        public static JulianDay operator /(JulianDay JDE1, double Days)
        { return new JulianDay(JDE1.JulianDayNumber / Days); }
        #endregion

        public override string ToString()
        {
            int year = this.Year;
            string era = "CE";
            if (year <= 0)
            {
                era = "BCE";
                year = Math.Abs(year) + 1;
            }
            return string.Format("{0}, {1} {2}, {3} {4}", this.DayOfWeek, this.Day, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(this.Month), year, era);
        }

        public override int GetHashCode()
        {
            return AstroMath.Int(this.JulianDayNumber);
        }

        public override bool Equals(object obj)
        {
            return (this.JulianDayNumber == ((JulianDay)obj).JulianDayNumber);
        }
    }

    public enum Calendar
    {
        Julian,
        Gregorian
    }
}
