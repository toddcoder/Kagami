using Core.Collections;
using Kagami.Library.Objects;

namespace Kagami.Library.Packages;

public class HashCode : IObject
{
   protected uint length;
   protected uint seed;
   protected uint value1;
   protected uint value2;
   protected uint value3;
   protected uint value4;
   protected uint queue1;
   protected uint queue2;
   protected uint queue3;

   public HashCode()
   {
      length = 0;
      seed = 1073741824U;
      value1 = 0U;
      value2 = 0U;
      value3 = 0U;
      queue1 = 0U;
      queue2 = 0U;
      queue3 = 0U;
   }

   public IObject Add(IObject value)
   {
      var input = (uint)value.Hash;
      var count = length++;

      switch (count)
      {
         case 0:
            queue1 = input;
            break;
         case 1:
            queue2 = input;
            break;
         case 2:
            queue3 = input;
            break;
         default:
            if (count == 3U)
            {
               initialize();
               value1 = round(value1, queue1);
               value2 = round(value2, queue2);
               value3 = round(value3, queue3);
               value4 = round(value4, input);
            }

            break;

      }

      return this;
   }

   protected void initialize()
   {
      value1 = (uint)((int)seed - 1640531535 - 2048144777);
      value2 = seed + 2246822519U;
      value3 = seed;
      value4 = seed - 2654435761U;
   }

   protected static uint rotateLeft(uint value, int offset) => value << offset | value >> 32 - offset;

   protected static uint round(uint hash, uint input) => rotateLeft(hash + input * 2246822519U, 13) * 2654435761U;

   protected uint queueRound(uint hash, uint input) => rotateLeft(hash + input * 3266489917U, 17) * 668265263U;

   protected uint empty() => seed + 374761393U;

   protected uint state() => rotateLeft(value1, 1) + rotateLeft(value2, 7) + rotateLeft(value3, 12) + rotateLeft(value4, 18);

   protected static uint final(uint hash)
   {
      hash ^= hash >> 15;
      hash *= 2246822519U;
      hash ^= hash >> 13;
      hash *= 3266489917U;
      hash ^= hash >> 16;

      return hash;
   }

   public string ClassName => "HashCode";

   public string AsString => $"HashCode({length})";

   public string Image =>
      $"HashCode(length = {length}, seed = {seed}, value1 = {value1}, value2 = {value2}, value3 = {value3}, value4 = {value4}, queue1 = {queue1}, queue2 = {queue2}, queue3 = {queue3})";

   public int Hash
   {
      get
      {
         var num = length % 4U;
         var hash = (length < 4U ? empty() : state()) + length * 4U;
         if (num > 0U)
         {
            hash = queueRound(hash, queue1);
            if (num > 1U)
            {
               hash = num switch
               {
                  > 2U => queueRound(hash, queue3),
                  _ => queueRound(hash, queue2)
               };
            }
         }

         return (int)final(hash);
      }
   }

   public bool IsEqualTo(IObject obj) => obj is HashCode hashCode && hashCode.Image == Image;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => IsEqualTo(comparisand);

   public bool IsTrue => length > 0;

   public Guid Id { get; init; } = Guid.NewGuid();
}