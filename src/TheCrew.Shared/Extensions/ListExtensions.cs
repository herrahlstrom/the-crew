using System.Collections;

namespace TheCrew.Shared.Extensions;

public static class ListExtensions
{
    public static void Randomize(this IList list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Shared.Next(0, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
