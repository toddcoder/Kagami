﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class EmptyTuple : Operation
   {
      public override IMatched<IObject> Execute(Machine machine) => Tuple.Empty.Matched<IObject>();

      public override string ToString() => "empty.tuple";
   }
}