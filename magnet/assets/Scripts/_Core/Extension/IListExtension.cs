using UnityEngine;
using System.Collections.Generic;

public static class IListExtension
{
    public static IList<T> Shuffle<T>(this IList<T> input)
    {
        for (var top = input.Count - 1; top > 1; --top)
        {
            var swap = Random.Range( 0, top );
            T tmp = input[top];
            input[top] = input[swap];
            input[swap] = tmp;
        }

        return input;
    }
}
