namespace TheCrew.Shared.Extensions;

public static class EnumerableExtensions
{
   public static IEnumerable<T> AtRandomOrder<T>(this IEnumerable<T> items) where T : notnull
   {
      var enumerator = new RandomEnumerator<T>(items);
      while (enumerator.MoveNext())
      {
         yield return enumerator.Current;
      }
   }

   public static LoopEnumerator<T> GetLoopEnumerator<T>(this IEnumerable<T> items) where T : notnull
   {
      return new LoopEnumerator<T>(items);
   }

   public static RandomEnumerator<T> GetRandomEnumerator<T>(this IEnumerable<T> items) where T : notnull
   {
      return new RandomEnumerator<T>(items);
   }

   public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this IEnumerable<T> items) where T : notnull
   {
      if (items is IReadOnlyCollection<T> collection)
      {
         return collection;
      }
      return items.ToList();
   }

   public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
   {
      foreach(var item in items)
      {
         action.Invoke(item);
      }
   }
}
