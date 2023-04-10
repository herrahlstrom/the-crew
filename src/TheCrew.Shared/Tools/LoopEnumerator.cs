using System.Collections;

namespace TheCrew.Shared.Extensions;

public class LoopEnumerator<T> : IEnumerator<T> where T : notnull
{
    private readonly IReadOnlyList<T> _collection;
    private int _current;

     public LoopEnumerator(IEnumerable<T> items)
    {
        _collection = items is IReadOnlyList<T> collection
            ? collection
            : items.ToList();

        Reset();
    }

   public T Current => _collection[_current];

   object IEnumerator.Current => _collection[_current];

   public void Dispose()
   {   }

   public bool MoveNext()
   {
      if(_collection.Count==0)
      {
        return false;
      }

      _current++;
      if(_current >= _collection.Count)
      {
        _current = 0;
      }
      return true;
   }

   public void Reset()
   {
      _current = -1;
   }
}
