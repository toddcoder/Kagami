using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class SendMessageSymbol : Symbol, IHasExpressions
{
   protected Selector selector;
   protected Precedence precedence;
   protected bool optional;
   protected Maybe<LambdaSymbol> _lambda;
   protected Maybe<Operation> _operation;
   protected Expression[] arguments;

   public SendMessageSymbol(Selector selector, Precedence precedence, bool optional, Maybe<LambdaSymbol> _lambda, Maybe<Operation> _operation,
      params Expression[] arguments)
   {
      this.selector = selector;
      this.precedence = precedence;
      this.optional = optional;
      this._lambda = _lambda;
      this._operation = _operation;
      this.arguments = arguments;
   }

   public SendMessageSymbol(Selector selector, bool optional, Maybe<LambdaSymbol> _lambda, Maybe<Operation> _operation,
      params Expression[] arguments) : this(selector, Precedence.SendMessage, optional, _lambda, _operation, arguments)
   {
   }

   public SendMessageSymbol(Selector selector, Precedence precedence, bool optional, params Expression[] arguments) : this(selector, precedence,
      optional, nil, nil, arguments)
   {
   }

   public SendMessageSymbol(Selector selector, bool optional, params Expression[] arguments) : this(selector, Precedence.SendMessage,
      optional, nil, nil, arguments)
   {
   }

   public SendMessageSymbol(Selector selector, Precedence precedence, bool optional, Maybe<Operation> _operation, params Expression[] arguments) :
      this(selector, precedence, optional, nil, _operation, arguments)
   {
   }

   public SendMessageSymbol(Selector selector, bool optional, Maybe<Operation> _operation, params Expression[] arguments) :
      this(selector, Precedence.SendMessage, optional, nil, _operation, arguments)
   {
   }

   public SendMessageSymbol(Selector selector, Precedence precedence, bool optional, Maybe<LambdaSymbol> _lambda, params Expression[] arguments) :
      this(selector, precedence, optional, _lambda, nil, arguments)
   {
   }

   public SendMessageSymbol(Selector selector, bool optional, Maybe<LambdaSymbol> _lambda, params Expression[] arguments) :
      this(selector, Precedence.SendMessage, optional, _lambda, nil, arguments)
   {
   }

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
               parametersList.Add(Parameter.New(false, parameterName));
               argumentsList.Add(new Expression(new FieldSymbol(parameterName)));
            }
            else
            {
               argumentsList.Add(argument);
            }
         }

         var newSendMessageSymbol = new SendMessageSymbol(selector, precedence, optional, _lambda, _operation, [.. argumentsList]);
         var parameters = new Parameters([.. parametersList]);
         var returnStatement = new Statements.Return(new Expression(newSendMessageSymbol), nil);
         var block = new Block(returnStatement);
         var newLambda = new LambdaSymbol(parameters, block);
         newLambda.Generate(builder);

         return;
      }

      var endLabel = newLabel("end");

      if (optional)
      {
         builder.Dup();
         builder.IsMonad(MonadType.None | MonadType.Failure);
         builder.GoToIfTrue(endLabel);
         builder.SendMessage("value".get(), 0);
      }

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

   public override Precedence Precedence => precedence;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $".{selector.Image}({arguments.ToString(", ")})";

   public SendMessageSymbol AsChainOperator()
   {
      return new SendMessageSymbol(selector, Precedence.ChainedOperator, optional, _lambda, _operation, arguments);
   }

   public Expression[] Expressions => arguments;
}