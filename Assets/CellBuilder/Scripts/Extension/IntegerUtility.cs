using UnityEngine;

public static class IntegerUtility
{
    public static bool IsBetween(this int value, int min, int max) => value >= min && value <= max;
    public static bool IsBetween(this int value, int max) => value >= 0 && value <= max;
    public static bool IsBetween(this int? value, int max) => (value >= 0 && value <= max) && value.HasValue;
    public static int PositiveRandom(int max) => UnityEngine.Random.Range(0, max);
}
