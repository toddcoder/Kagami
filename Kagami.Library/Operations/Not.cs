﻿using Core.Monads;

namespace Kagami.Library.Operations
{
   public class Not : OneBooleanOperation
   {
      public override IMatched<bool> Execute(bool boolean) => (!boolean).Matched();

      public override string ToString() => "not";
   }
}