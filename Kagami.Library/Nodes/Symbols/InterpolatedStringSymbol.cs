using System;
using System.Linq;
using Kagami.Library.Operations;
using Standard.Types.Enumerables;
using Standard.Types.Strings;

namespace Kagami.Library.Nodes.Symbols
{
   public class InterpolatedStringSymbol : Symbol
   {
      string prefix;
      Expression[] expressions;
      string[] suffixes;

      public InterpolatedStringSymbol(string prefix, Expression[] expressions, string[] suffixes)
      {
         this.prefix = prefix;
         this.expressions = expressions;
         this.suffixes = suffixes;
      }

      public override void Generate(OperationsBuilder builder)
      {
         builder.PushString(prefix);

         var length = Math.Min(expressions.Length, suffixes.Length);
         for (var i = 0; i < length; i++)
         {
            expressions[i].Generate(builder);
            builder.String();
            builder.SendMessage("~", 1);
            builder.PushString(suffixes[i]);
            builder.SendMessage("~", 1);
         }
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString()
      {
         return (StringStream)"$\"" / prefix / expressions.Zip(suffixes, (e, s) => $"({e}){s}").Listify("");
      }
   }
}