using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class WhateverSymbol(int count = -1) : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.GetField(ToString());

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"__${(count == -1 ? Count : count)}";

   public int Count { get; set; }
}