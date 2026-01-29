using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class SendClassMessageSymbol(Selector selector, Maybe<LambdaSymbol> _lambda, Maybe<Operation> _operation, params Expression[] arguments) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      if (arguments.Any(a => a.Symbols[0] is AnySymbol))
      {
         List<Expression> argumentsList = [];
         List<Parameter> parametersList = [];
         foreach (var argument in arguments)
         {
            if (argument.Symbols[0] is AnySymbol)
            {
               var parameterName = $"__${parametersList.Count}";
               parametersList.Add(Parameter.New(false, false, parameterName));
               argumentsList.Add(new Expression(new FieldSymbol(parameterName)));
            }
            else
            {
               argumentsList.Add(argument);
            }
         }

         var newSendMessageSymbol = new SendMessageSymbol(selector, Precedence.SendMessage, false, _lambda, _operation, [.. argumentsList]);
         var parameters = new Parameters([.. parametersList]);
         var returnStatement = new Statements.Return(new Expression(newSendMessageSymbol), nil);
         var block = new Block(returnStatement);
         var newLambda = new LambdaSymbol(parameters, block);
         newLambda.Generate(builder);

         return;
      }

      var endLabel = newLabel("end");

      builder.GetClass();

      if (_operation)
      {
         builder.Dup();
         var getter = selector.Name.Drop(-1).get();
         builder.SendMessage(getter, 0);
      }

      var index = 0;
      foreach (var argument in arguments)
      {
         selector.Generate(index++, argument, builder);
      }

      if (_operation is (true, var operation))
      {
         builder.AddRaw(operation);
      }

      int count;
      if (_lambda is (true, var lambda))
      {
         lambda.Generate(builder);
         count = arguments.Length + 1;
      }
      else
      {
         count = arguments.Length;
      }

      builder.SendMessage(selector, count);

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $"{selector}({arguments.Select(e => e.ToString()).ToString(" ")}";
}