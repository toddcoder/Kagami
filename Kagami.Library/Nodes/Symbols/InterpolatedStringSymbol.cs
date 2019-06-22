using System;
using System.Linq;
using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Strings;

namespace Kagami.Library.Nodes.Symbols
{
   public class InterpolatedStringSymbol : Symbol
   {
      string prefix;
      Expression[] expressions;
      string[] suffixes;
	   bool isFailure;

      public InterpolatedStringSymbol(string prefix, Expression[] expressions, string[] suffixes, bool isFailure)
      {
         this.prefix = prefix;
         this.expressions = expressions;
         this.suffixes = suffixes;
	      this.isFailure = isFailure;
      }

      public override void Generate(OperationsBuilder builder)
      {
         builder.PushString(prefix);

         var length = Math.Min(expressions.Length, suffixes.Length);
         for (var i = 0; i < length; i++)
         {
            expressions[i].Generate(builder);
            builder.String();
            builder.SendMessage("~(_)", 1);
            builder.PushString(suffixes[i]);
            builder.SendMessage("~(_)", 1);
         }

			if (isFailure)
			{
				builder.Failure();
			}
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString()
      {
         return (StringStream)"$\"" / prefix / expressions.Zip(suffixes, (e, s) => $"({e}){s}").Stringify("");
      }
   }
}