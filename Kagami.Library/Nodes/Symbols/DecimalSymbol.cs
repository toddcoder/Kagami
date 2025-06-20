﻿using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class DecimalSymbol : Symbol, IConstant
{
   protected decimal value;

   public DecimalSymbol(decimal value)
   {
      this.value = value;
   }

   public override void Generate(OperationsBuilder builder)
   {
      builder.PushDecimal(value);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => value.ToString();

   public IObject Object => new XDecimal(value);
}