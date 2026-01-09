using Core.Enumerables;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class JunctionSymbol(string junctionType, Expression[] expressions) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      switch (expressions.Length)
      {
         case 1:
            expressions[0].Generate(builder);
            break;
         case > 1:
         {
            expressions[0].Generate(builder);
            expressions[1].Generate(builder);
            builder.NewSequence();

            foreach (var expression in expressions.Skip(2))
            {
               expression.Generate(builder);
               builder.NewSequence();
            }

            break;
         }
      }

      builder.NewJunction(junctionType);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"{junctionType}[{expressions.ToString(", ")}]";
}