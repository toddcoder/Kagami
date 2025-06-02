using Kagami.Library.Operations;
using Core.Enumerables;

namespace Kagami.Library.Nodes.Symbols;

public class IndexerSymbol : Symbol
{
   public static void Get(OperationsBuilder builder, Expression[] arguments)
   {
      GetIndex(builder, arguments);
      builder.SendMessage("[](_)", 1);
   }

   public static void GetIndex(OperationsBuilder builder, Expression[] arguments)
   {
      if (arguments.Length == 1)
      {
         arguments[0].Generate(builder);
      }
      else
      {
         arguments[0].Generate(builder);
         arguments[1].Generate(builder);
         builder.NewContainer();

         foreach (var argument in arguments.Skip(2))
         {
            argument.Generate(builder);
            builder.NewContainer();
         }
      }
   }

   protected Expression[] arguments;

   public IndexerSymbol(Expression[] arguments) => this.arguments = arguments;

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override void Generate(OperationsBuilder builder) => Get(builder, arguments);

   public override string ToString() => $"[{arguments.ToString(", ")}]";
}