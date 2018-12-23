using System;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Packages;
using Standard.Types.Monads;

namespace Kagami.Library.Nodes.Symbols
{
   public class CallSysFunctionSymbol1 : Symbol
   {
      Func<Sys, IObject, IResult<IObject>> func;
      string image;

      public CallSysFunctionSymbol1(Func<Sys, IObject, IResult<IObject>> func, string image)
      {
         this.func = func;
         this.image = image;
      }

      public override void Generate(OperationsBuilder builder) => builder.CallSysFunction1(func, image);

      public override Precedence Precedence => Precedence.PrefixOperator;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => image;
   }
}