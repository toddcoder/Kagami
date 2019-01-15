using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Standard.Types.Enumerables;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;
using Return = Kagami.Library.Nodes.Statements.Return;

namespace Kagami.Library.Nodes.Symbols
{
   public class InvokeSymbol : Symbol
   {
      protected string functionName;
      protected Expression[] arguments;
      protected IMaybe<LambdaSymbol> lambda;

      public InvokeSymbol(string functionName, Expression[] arguments, IMaybe<LambdaSymbol> lambda)
      {
         this.functionName = functionName;
         this.arguments = arguments;
         this.lambda = lambda;
      }

      public override void Generate(OperationsBuilder builder)
      {
	      if (arguments.Any(a => a.Symbols[0] is AnySymbol))
	      {
		      var argumentsList = new List<Expression>();
				var parametersList = new List<Parameter>();
		      foreach (var argument in arguments)
			      if (argument.Symbols[0] is AnySymbol)
			      {
				      var parameterName = $"__${parametersList.Count}";
				      parametersList.Add(Parameter.New(false, parameterName));
				      argumentsList.Add(new Expression(new FieldSymbol(parameterName)));
			      }
			      else
				      argumentsList.Add(argument);

		      var newInvokeSymbol = new InvokeSymbol(functionName, argumentsList.ToArray(), lambda);
		      var parameters = new Parameters(parametersList.ToArray());
		      var returnStatement = new Return(new Expression(newInvokeSymbol), none<TypeConstraint>());
		      var block = new Block(returnStatement);
		      var newLambda = new LambdaSymbol(parameters, block);
		      newLambda.Generate(builder);
	      }
			else
	      {
		      foreach (var argument in arguments)
			      argument.Generate(builder);
		      int count;
		      if (lambda.If(out var l))
		      {
			      l.Generate(builder);
			      count = arguments.Length + 1;
		      }
		      else
			      count = arguments.Length;

		      builder.Invoke(functionName, count);
	      }
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"{functionName}({arguments.Listify()})";
   }
}