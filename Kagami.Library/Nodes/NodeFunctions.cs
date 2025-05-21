using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.CommonFunctions;

namespace Kagami.Library.Nodes;

public static class NodeFunctions
{
   private static int uniqueId;

   public static string newLabel(string name) => mangled(name, uniqueId++);

   public static void ResetFieldUniqueID() => uniqueId = 0;

   public static string id() => uniqueId++.ToString();

   public static Result<Lambda> operatorLambda(Symbol operatorSymbol, OperationsBuilder builder)
   {
      var exBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
      exBuilder.Add(new FieldSymbol("__$0"));
      exBuilder.Add(operatorSymbol);
      exBuilder.Add(new FieldSymbol("__$1"));

      return exBuilder.ToExpression().Map(expression =>
      {
         var invokable = new LambdaInvokable(new Parameters(2), $"$0 {operatorSymbol} $1");
         return builder.RegisterInvokable(invokable, expression, true).Map(_ => new Lambda(invokable));
      });
   }
}