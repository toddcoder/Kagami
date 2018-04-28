using Standard.Types.Collections;

namespace Kagami.Library.Classes
{
   public class KeyCollectingHash<TKey, TValue> : Hash<TKey, TValue>
   {
      protected Set<TKey> collectedKeys;
      protected bool collecting;

      public KeyCollectingHash()
      {
         collectedKeys = new Set<TKey>();
         collecting = false;
      }

      public bool Collecting
      {
         get => collecting;
         set
         {
            collecting = value;
            if (collecting)
               collectedKeys.Clear();
         }
      }

      public new TValue this[TKey key]
      {
         get => base[key];
         set
         {
            base[key] = value;
            if (collecting)
               collectedKeys.Add(key);
         }
      }

      public Set<TKey> CollectedKeys => collectedKeys;
   }
}