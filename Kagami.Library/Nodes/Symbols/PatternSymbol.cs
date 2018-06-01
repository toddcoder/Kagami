using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Standard.Types.Enumerables;

namespace Kagami.Library.Nodes.Symbols
{
   public class PatternSymbol : Symbol
   {
      (Expression comparisand, Expression lambda)[] items;

      public PatternSymbol((Expression, Expression)[] items) => this.items = items;

      public override void Generate(OperationsBuilder builder)
      {
         foreach (var (expression1, expression2) in items)
         {
            expression1.Generate(builder);
            var invokable = new LambdaInvokable(Parameters.Empty, expression2.ToString());
            if (builder.RegisterInvokable(invokable, expression2, true).If(out _, out var exception))
               builder.PushObject(new Lambda(invokable));
            else
               throw exception;
         }

         builder.ToArguments(items.Length * 2);
         builder.NewValue("Pattern", arguments =>
         {
            var list = new List<(IObject, Lambda)>();
            for (var i = 0; i < arguments.Length; i += 2)
            {
               var comparisand = arguments[i];
               var lambda = (Lambda)arguments[i + 1];
               list.Add((comparisand, lambda));
            }

            return new Pattern(list.ToArray());
         });
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => items.Select(i => $"{i.comparisand} = {i.lambda}").Listify("|");
   }
}