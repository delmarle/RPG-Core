using System;

public static class Int64Extensions
{
    // IF-CHAIN:
    public static int Digits_IfChain(this long n)
    {
        if (n >= 0)
        {
            if (n < 10L) return 1;
            if (n < 100L) return 2;
            if (n < 1000L) return 3;
            if (n < 10000L) return 4;
            if (n < 100000L) return 5;
            if (n < 1000000L) return 6;
            if (n < 10000000L) return 7;
            if (n < 100000000L) return 8;
            if (n < 1000000000L) return 9;
            if (n < 10000000000L) return 10;
            if (n < 100000000000L) return 11;
            if (n < 1000000000000L) return 12;
            if (n < 10000000000000L) return 13;
            if (n < 100000000000000L) return 14;
            if (n < 1000000000000000L) return 15;
            if (n < 10000000000000000L) return 16;
            if (n < 100000000000000000L) return 17;
            if (n < 1000000000000000000L) return 18;
            return 19;
        }
        else
        {
            if (n > -10L) return 2;
            if (n > -100L) return 3;
            if (n > -1000L) return 4;
            if (n > -10000L) return 5;
            if (n > -100000L) return 6;
            if (n > -1000000L) return 7;
            if (n > -10000000L) return 8;
            if (n > -100000000L) return 9;
            if (n > -1000000000L) return 10;
            if (n > -10000000000L) return 11;
            if (n > -100000000000L) return 12;
            if (n > -1000000000000L) return 13;
            if (n > -10000000000000L) return 14;
            if (n > -100000000000000L) return 15;
            if (n > -1000000000000000L) return 16;
            if (n > -10000000000000000L) return 17;
            if (n > -100000000000000000L) return 18;
            if (n > -1000000000000000000L) return 19;
            return 20;
        }
    }

    // USING LOG10:
    public static int Digits_Log10(this long n) =>
        n == 0L ? 1 : (n > 0L ? 1 : 2) + (int)Math.Log10(Math.Abs((double)n));

    // WHILE LOOP:
    public static int Digits_While(this long n)
    {
        int digits = n < 0 ? 2 : 1;
        while ((n /= 10L) != 0L) ++digits;
        return digits;
    }

    // STRING CONVERSION:
    public static int Digits_String(this long n) =>
        n.ToString().Length;
}