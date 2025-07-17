using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Parsers;

public abstract record PossibleIfExpression
{
   public sealed record If(Expression Expression) : PossibleIfExpression;

   public sealed record IfNot(Expression Expression) : PossibleIfExpression;

   public sealed record None : PossibleIfExpression;
}