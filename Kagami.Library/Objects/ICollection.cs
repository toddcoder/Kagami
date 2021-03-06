﻿using Core.Monads;

namespace Kagami.Library.Objects
{
   public interface ICollection : ISkipTakeable
   {
      IIterator GetIterator(bool lazy);

      IMaybe<IObject> Next(int index);

      IMaybe<IObject> Peek(int index);

      Int Length { get; }

      bool ExpandForArray { get; }

      Boolean In(IObject item);

      Boolean NotIn(IObject item);

      IObject Times(int count);

      String MakeString(string connector);

      IIterator GetIndexedIterator();
   }
}