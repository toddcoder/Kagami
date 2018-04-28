using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public abstract class Statement : Node, IOperationsGenerator
   {
      public static implicit operator Statement(Symbol symbol) => new ExpressionStatement(new Expression(symbol), true);

      public abstract void Generate(OperationsBuilder builder);
   }
}