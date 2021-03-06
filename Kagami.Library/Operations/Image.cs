﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class Image : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => String.StringObject(value.Image).Matched();

      public override string ToString() => "image";
   }
}