using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Parsers;

public abstract record OperatorType(string FunctionName)
{
   public sealed record Infix(string FunctionName, Precedence Precedence) : OperatorType(FunctionName);

   public sealed record Prefix(string FunctionName) : OperatorType(FunctionName);

   public sealed record Postfix(string FunctionName) : OperatorType(FunctionName);
}