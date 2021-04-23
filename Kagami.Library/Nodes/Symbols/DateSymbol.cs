using System;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class DateSymbol : Symbol
   {
      protected Date date;

      public DateSymbol(DateTime date)
      {
         this.date = date;
      }

      public override void Generate(OperationsBuilder builder) => builder.PushObject(date);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => date.Image;
   }
}