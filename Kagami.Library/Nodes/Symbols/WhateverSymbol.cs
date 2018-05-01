using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class WhateverSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.GetField(Count.ToString().get());

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => Count.ToString().get();

      public int Count { get; set; } = 0;
   }
}