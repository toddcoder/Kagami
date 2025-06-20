﻿using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class RationalSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.NewRational();

   public override Precedence Precedence => Precedence.MultiplyDivide;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => " r ";
}