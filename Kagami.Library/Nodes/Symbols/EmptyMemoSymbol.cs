using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class EmptyMemoSymbol(LambdaSymbol lambdaSymbol) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var emptyDictionarySymbol = new EmptyDictionarySymbol();
      emptyDictionarySymbol.Generate(builder);
      lambdaSymbol.Generate(builder);
      builder.SendMessage("memo(_)", 1);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"{{:{lambdaSymbol}}}";
}