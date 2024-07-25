using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class WhateverSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.GetField(ToString());

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

	   public override string ToString() => $"__${Count}";

      public int Count { get; set; }
   }
}