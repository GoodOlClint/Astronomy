using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Astronomy
{
    public class Instant
    {

        public double TotalSeconds { get; protected set; }
        public int Hour { get { return (int)(Math.Floor(this.TotalSeconds) / 3600) % 24; } }
        public int Minute { get { return (int)(Math.Floor(this.TotalSeconds / 60)) % 60; } }

        public double Second { get { return this.TotalSeconds % 60; } }

        public Instant(int Hour, int Minute, double Second)
        {
            this.AddSeconds(Second + (Minute * 60) + (Hour * 3600));
        }

        public Instant(double Seconds)
        { this.AddSeconds(Seconds); }

        public void AddSeconds(double Second)
        {
            this.TotalSeconds = (this.TotalSeconds + Second) % 86400;
            if (this.TotalSeconds < 0)
            { this.TotalSeconds += 86400; }
        }
    }
}
