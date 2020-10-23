using Kagami.Library.Operations;
using Core.Enumerables;
using static Core.Arrays.ArrayFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class SkipTakeOperatorSymbol : Symbol
   {
      SkipTakeItem[] arguments;

      public SkipTakeOperatorSymbol(SkipTakeItem[] arguments) => this.arguments = arguments;

      public SkipTakeOperatorSymbol(SkipTakeItem skipTakeItem) : this(array(skipTakeItem)) { }

      public override void Generate(OperationsBuilder builder)
      {
         foreach (var skipTakeItem in arguments)
         {
	         skipTakeItem.Generate(builder);
         }
      }

      public override Precedence Precedence => Precedence.PostfixOperator;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => $"{{{arguments.ToString(", ")}}}";
   }
}