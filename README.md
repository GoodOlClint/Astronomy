#C# Astronomy Library

##Julian Day
Allows calculations of any date after noon on January 1, 4713 BC.

```csharp
    JulianDay JD = new JulianDay(333, 1, 27, 12);
    Debug.Assert(JD.JulianDayNumber == 1842713.0);

    JulianDay JD1 = new JulianDay(1957, 10, 4).AddDays(0.81);
    Debug.Assert(JD1.JulianDayNumber == 2436116.31);
```

##Instant
Helper class for calculating Sidereal time

```csharp
	JulianDay JD = new JulianDay(2446895.5);
    double T = (JD.JulianDayNumber - 2451545.0) / 36525;


    Instant sidereal = new Instant(6, 41, 50.54841);
    sidereal.AddSeconds(8640184.812866 * T);
    sidereal.AddSeconds(0.093104 * Math.Pow(T,2));
    sidereal.AddSeconds(0.0000062 * Math.Pow(T, 3));

    Instant sidereal1 = new Instant(1.00273790935 * new Instant(19, 21, 0).TotalSeconds);

    Instant sidereal2 = new Instant(sidereal1.TotalSeconds + sidereal.TotalSeconds);
```
