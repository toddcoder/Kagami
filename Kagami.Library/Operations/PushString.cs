﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class PushString : Operation
   {
      protected String value;

      public PushString(string value) => this.value = value;

      public override IMatched<IObject> Execute(Machine machine) => value.Matched<IObject>();

      public override string ToString() => $"push.string({value.Image})";
   }
}