﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class PushNil : Operation
   {
      public override IMatched<IObject> Execute(Machine machine) => Nil.NilValue.Matched();

      public override string ToString() => "push.nil";
   }
}