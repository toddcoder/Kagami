using System;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Packages;
using Standard.Types.Monads;

namespace Kagami.Library.Nodes.Symbols
{
   public class CallSysFunctionSymbol0 : Symbol
   {
      Func<Sys, IResult<IObject>> func;
      string image;

      public CallSysFunctionSymbol0(Func<Sys, IResult<IObject>> func, string image)
      {
         this.func = func;
         this.image = image;
      }

      public override void Generate(OperationsBuilder builder) => builder.CallSysFunction0(func, image);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => image;
   }
}