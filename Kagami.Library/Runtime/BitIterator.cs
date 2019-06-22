using System.Collections;
using System.Collections.Generic;

namespace Kagami.Library.Runtime
{
   public class BitIterator : IEnumerable<bool[]>
   {
      static bool isBitSet(int value, int position) => (value & 1 << position) != 0;

      int limit;
      bool[] digits;

      public BitIterator(int digitCount)
      {
         digits = new bool[digitCount];
         limit = (int)System.Math.Pow(2, digitCount);
      }

      public IEnumerator<bool[]> GetEnumerator()
      {
         for (var i = 0; i < limit; i++)
         {
            for (var j = 0; j < digits.Length; j++)
            {
	            digits[j] = isBitSet(i, j);
            }

            yield return digits;
         }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}