using Core.Monads;

namespace Kagami.Library.Objects
{
   public interface ICollection : ISkipTakeable
   {
      IIterator GetIterator(bool lazy);

      Maybe<IObject> Next(int index);

      Maybe<IObject> Peek(int index);

      Int Length { get; }

      bool ExpandForArray { get; }

      Boolean In(IObject item);

      Boolean NotIn(IObject item);

      IObject Times(int count);

      String MakeString(string connector);

      IIterator GetIndexedIterator();
   }
}