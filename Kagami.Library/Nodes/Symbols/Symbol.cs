using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public abstract class Symbol : Node, IOperationsGenerator
{
   public abstract void Generate(OperationsBuilder builder);

   public abstract Precedence Precedence { get; }

   public abstract Arity Arity { get; }

   public virtual bool LeftToRight => true;
}