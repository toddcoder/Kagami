using Core.Arrays;
using Kagami.Library.Operations;
using Core.Enumerables;
using Kagami.Library.Objects;

namespace Kagami.Library.Nodes.Symbols;

public class IndexerSymbol : Symbol
{
   public static void Get(OperationsBuilder builder, Expression[] arguments)
   {
      GetIndex(builder, arguments);
      Selector selector = $"[]({arguments.Length.Repeat("_")})";
      builder.SendMessage(selector, arguments.Length);
   }

   public static void GetIndex(OperationsBuilder builder, Expression[] arguments)
   {
      foreach (var expression in arguments)
      {
         expression.Generate(builder);
      }
   }

   protected Expression[] arguments;

   public IndexerSymbol(Expression[] arguments)
   {
      this.arguments = arguments;
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override void Generate(OperationsBuilder builder)
   {
      Get(builder, arguments);
   }

   public override string ToString() => $"[{arguments.ToString(", ")}]";
}