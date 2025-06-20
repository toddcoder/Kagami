﻿using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class StringSymbol : Symbol, IConstant
{
   protected string value;
   protected bool isFailure;

   public StringSymbol(string value, bool isFailure = false)
   {
      this.value = value;
      this.isFailure = isFailure;
   }

   public override void Generate(OperationsBuilder builder)
   {
      builder.PushString(value);
      if (isFailure)
      {
         builder.Failure();
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => value;

   public IObject Object => (KString)value;
}