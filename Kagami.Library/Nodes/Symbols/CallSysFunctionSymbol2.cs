using System;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Packages;
using Standard.Types.Monads;

namespace Kagami.Library.Nodes.Symbols
{
   public class CallSysFunctionSymbol2 : Symbol
   {
      Func<Sys, IObject, IObject, IResult<IObject>> func;
      string image;

      public CallSysFunctionSymbol2(Func<Sys, IObject, IObject, IResult<IObject>> func, string image)
      {
         this.func = func;
         this.image = image;
      }

      public override void Generate(OperationsBuilder builder) => builder.CallSysFunction2(func, image);

      public override Precedence Precedence => Precedence.ChainedOperator;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => image;
   }
}