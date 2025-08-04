using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class NewDelegateStatement(string className, string delegateClassName, Expression delegateConstructor) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      delegateConstructor.Generate(builder);
      builder.NewDelegate(className, delegateClassName);
   }

   public override string ToString() => $"delegate {delegateClassName} = {delegateConstructor}";
}