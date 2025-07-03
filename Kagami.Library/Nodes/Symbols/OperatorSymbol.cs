using Kagami.Library.Operations;
using Kagami.Library.Parsers;

namespace Kagami.Library.Nodes.Symbols;

public class OperatorSymbol : Symbol
{
   protected OperatorType operatorType;

   public OperatorSymbol(OperatorType operatorType)
   {
      this.operatorType = operatorType;
   }

   public override void Generate(OperationsBuilder builder)
   {
      switch (operatorType)
      {
         case OperatorType.Infix infix:
            builder.Invoke(infix.FunctionName, 2);
            break;
         case OperatorType.Prefix prefix:
            builder.Invoke(prefix.FunctionName, 1);
            break;
         case OperatorType.Postfix postfix:
            builder.Invoke(postfix.FunctionName, 1);
            break;
      }
   }

   public override Precedence Precedence => operatorType switch
   {
      OperatorType.Infix infix => infix.Precedence,
      OperatorType.Prefix => Precedence.PrefixOperator,
      OperatorType.Postfix => Precedence.PostfixOperator,
      _ => Precedence.ChainedOperator
   };

   public override Arity Arity => operatorType switch
   {
      OperatorType.Infix => Arity.Binary,
      OperatorType.Postfix => Arity.Postfix,
      OperatorType.Prefix => Arity.Prefix,
      _ => Arity.Binary
   };

   public override string ToString() => operatorType.FunctionName;
}