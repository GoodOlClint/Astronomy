#C# Astronomy Library

##Julian Day
Allows calculations of any date after noon on 1 January, 4713 BC.

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

##Easter
Used to calculate the date of Easter.
Valid for all years since the Julian Epoch (1 January 4713 BC);

```csharp
    Easter easter = new Easter(179);
    Debug.Assert(easter.Date.Month == 04);
    Debug.Assert(easter.Date.Day == 12);

    Easter easter2 = new Easter(1991);
    Debug.Assert(easter2.Date.Month == 03);
    Debug.Assert(easter2.Date.Day == 31);
```

##Islamic Calendar
Used to calculate dates on the Islamic calendar.
Valid for all dates since 16 July 622 on the Julian calendar.
```csharp
    IslamicCalendar MC = new IslamicCalendar(1421, 1, 1);
    JulianDay JD = MC.ToJulianDay();

    Debug.Assert(JD.Year == 2000);
    Debug.Assert(JD.Month == 4);
    Debug.Assert(JD.Day == 6);

    JulianDay JD1 = new JulianDay(1991, 8, 13);
    IslamicCalendar MC1 = IslamicCalendar.FromJulianDay(JD1);

    Debug.Assert(MC1.Year == 1412);
    Debug.Assert(MC1.Month == 2);
    Debug.Assert(MC1.Day == 2);
```


##Equinox Calendar
Used to calculate dates of the equinoxes
Valid for all years since the Julian Epoch (1 January 4713 BC);
```csharp
    double JDE = Equinox.Approximate(1962, Season.Summer);
    Debug.Assert(JDE == 2437837.39245);
```
