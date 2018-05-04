using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.CommonFunctions;

namespace Kagami.Library.Nodes
{
   public static class NodeFunctions
   {
      static int uniqueID;

      public static string newLabel(string name) => mangled(name, uniqueID++);

      public static void ResetUniqueID() => uniqueID = 0;

      public static string id() => uniqueID++.ToString();

      public static IResult<Lambda> operatorLambda(Symbol operatorSymbol, OperationsBuilder builder)
      {
         var exBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
         exBuilder.Add(new FieldSymbol("0".get()));
         exBuilder.Add(operatorSymbol);
         exBuilder.Add(new FieldSymbol("1".get()));

         return exBuilder.ToExpression().Map(expression =>
         {
            var invokable = new LambdaInvokable(new Parameters(2), $"$0 {operatorSymbol} $1");
            return builder.RegisterInvokable(invokable, expression, true).Map(i => new Lambda(invokable));
         });
      }
   }
}