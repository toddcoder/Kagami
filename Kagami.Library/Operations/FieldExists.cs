﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class FieldExists : Operation
   {
      string fieldName;

      public FieldExists(string fieldName) => this.fieldName = fieldName;

      public override IMatched<IObject> Execute(Machine machine)
      {
         return machine.Find(fieldName, true)
            .FlatMap(f => Boolean.True.Matched(), () => Boolean.False.Matched(), failedMatch<IObject>);
      }

      public override string ToString() => $"field.exists({fieldName})";
   }
}