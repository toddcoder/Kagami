﻿namespace Kagami.Library.Objects;

public interface IMutableCollection : ICollection
{
   IObject Append(IObject obj);

   IObject Remove(IObject obj);

   IObject RemoveAt(int index);

   IObject RemoveAll(IObject obj);

   IObject InsertAt(int index, IObject obj);

   KBoolean IsEmpty { get; }

   IObject Assign(SkipTake skipTake, IEnumerable<IObject> values);
}