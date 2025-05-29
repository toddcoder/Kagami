using Core.Monads;

namespace Kagami.Library.Objects;

public interface ICollection : ISkipTakeable
{
   IIterator GetIterator(bool lazy);

   Maybe<IObject> Next(int index);

   Maybe<IObject> Peek(int index);

   Int Length { get; }

   bool ExpandForArray { get; }

   KBoolean In(IObject item);

   KBoolean NotIn(IObject item);

   IObject Times(int count);

   KString MakeString(string connector);

   IIterator GetIndexedIterator();
}