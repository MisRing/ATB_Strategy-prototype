using System;
using System.Collections.Generic;

public static class CollectionExtensions
{
    private static Random rng = new Random();

    public static bool IsValidIndex<T>(this IList<T> collection, int index)
    {
        return index >= 0 && index < collection.Count;
    }
    
    public static bool IsValidIndex<T>(this T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }
    
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
