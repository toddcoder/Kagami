using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class AsSymbol(string toClassName) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.Dup();
      builder.ClassName();
      builder.PushString(toClassName);
      builder.Convert();
   }

   public override Precedence Precedence => Precedence.PostfixOperator;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $"as {toClassName}";
}