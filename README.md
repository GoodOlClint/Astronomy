#C# Astronomy Library

##Julian Day
Allows calculations of any date after noon on January 1, 4713 BC.

```charp
    JulianDay JD = new JulianDay(333, 1, 27, 12);
    Debug.Assert(JD.JulianDayNumber == 1842713.0);

    JulianDay JD1 = new JulianDay(1957, 10, 4).AddDays(0.81);
    Debug.Assert(JD1.JulianDayNumber == 2436116.31);
```