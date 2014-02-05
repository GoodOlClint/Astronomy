#C# Astronomy Library

##Julian Day
Allows calculations of any date after noon on January 1, 4713 BC.

```csharp
    JulianDay JD = new JulianDay(333, 1, 27, 12);
    Debug.Assert(JD.JulianDayNumber == 1842713.0);

    JulianDay JD1 = new JulianDay(1957, 10, 4) + 0.81;
    Debug.Assert(JD1.JulianDayNumber == 2436116.31);
```

##Instant
Helper class for calculating Sidereal time

```csharp
	JulianDay JD = new JulianDay(1987, 04, 10, 0);

    double T = (JD.JulianDayNumber - 2451545.0) / 36525;

    Instant MeanSiderealDate = new Instant(6, 41, 50.54841);
    MeanSiderealDate += 8640184.812866 * T;
    MeanSiderealDate += 0.093104 * Math.Pow(T, 2);
    MeanSiderealDate += 0.0000062 * Math.Pow(T, 3);

    Debug.Assert(MeanSiderealDate.Hour == 13);
    Debug.Assert(MeanSiderealDate.Minute == 10);
    Debug.Assert(Math.Round(MeanSiderealDate.Second, 4) == 46.3668);

    Instant MeanSiderealInstant = new Instant(19, 21, 00);
    MeanSiderealInstant = MeanSiderealInstant * 1.00273790935;
    MeanSiderealInstant += MeanSiderealDate;
    Debug.Assert(MeanSiderealInstant.Hour == 8);
    Debug.Assert(MeanSiderealInstant.Minute == 34);
    Debug.Assert(Math.Round(MeanSiderealInstant.Second, 4) == 57.0896);
```
