namespace Kagami.Library.Parsers;

public abstract record OperatorType(string FunctionName)
{
   public sealed record Infix(string FunctionName) : OperatorType(FunctionName);

   public sealed record Prefix(string FunctionName) : OperatorType(FunctionName);

   public sealed record Postfix(string FunctionName) : OperatorType(FunctionName);
}