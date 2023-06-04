using System.Collections;

namespace TheCrew.Shared.Extensions;

public class RandomEnumerator<T> : IEnumerator<T> where T : notnull
{
   private readonly IReadOnlyList<T> _collection;
   private readonly List<int> _collectionIndecies;
   private int _current;

   public RandomEnumerator(IEnumerable<T> items)
   {
      _collection = items is IReadOnlyList<T> collection
          ? collection
          : items.ToList();

      _collectionIndecies = new List<int>(_collection.Count);

      Reset();
   }

   public T Current => _collection[_collectionIndecies[_current]];

   object IEnumerator.Current => this.Current;

   void IDisposable.Dispose()
   { }

   public bool MoveNext()
   {
      return ++_current < _collection.Count;
   }

   public void Reset()
   {
      _collectionIndecies.Clear();
      _collectionIndecies.AddRange(Enumerable.Range(0, _collection.Count));
      _collectionIndecies.Randomize();

      _current = -1;
   }
}