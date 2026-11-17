using UnityEngine;

public static class QuantityFormat
{
    public static int Gcd(int a, int b)
    {
        a = Mathf.Abs(a); b = Mathf.Abs(b);
        while (b != 0) { int t = b; b = a % b; a = t; }
        return a == 0 ? 1 : a;
    }

    // Fracción simplificada: 3/4, 1, 5/4
    public static string Fraction(int units, int whole)
    {
        if (units == 0) return "0";
        int g = Gcd(units, whole);
        int n = units / g, d = whole / g;
        return d == 1 ? n.ToString() : $"<sup>{n}</sup>/<sub>{d}</sub>";
    }

    // "4/8 = 1/2" cuando se puede simplificar
    public static string FractionWithEquivalent(int units, int whole)
    {
        int g = Gcd(units, whole);
        if (units == 0 || g == 1) return Fraction(units, whole);
        return $"<sup>{units}</sup>/<sub>{whole}</sub> = {Fraction(units, whole)}";
    }

    // Texto corto (para la etiqueta de cada vaso)
    public static string Short(int units, Challenge c)
    {
        return c.displayMode == DisplayMode.Fraction
            ? Fraction(units, c.unitsPerWhole)
            : units + " ml";
    }

    // Texto con unidad (para mensajes)
    public static string Amount(int units, Challenge c)
    {
        return c.displayMode == DisplayMode.Fraction
            ? Fraction(units, c.unitsPerWhole) + " L"
            : units + " ml";
    }

    public static string Counter(int current, Challenge c)
    {
        if (c.displayMode == DisplayMode.Fraction)
            return $"{Fraction(current, c.unitsPerWhole)} de {Fraction(c.targetMl, c.unitsPerWhole)} L";
        return $"{current} / {c.targetMl} ml";
    }
}