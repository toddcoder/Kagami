using System.Collections;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Statements;

public class Block : Statement, IEnumerable<Statement>
{
   public static Block Getter(string fieldName)
   {
      var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
      expressionBuilder.Add(new FieldSymbol(fieldName));
      var _expression = expressionBuilder.ToExpression();
      if (_expression is (true, var expression))
      {
         return new Block(new Return(expression, nil));
      }
      else
      {
         throw _expression.Exception;
      }
   }

   public static Block Setter(string fieldName, string parameterName)
   {
      var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
      expressionBuilder.Add(new FieldSymbol(parameterName));
      var _expression = expressionBuilder.ToExpression();
      if (_expression is (true, var expression))
      {
         var assignToField = new AssignToField(fieldName, nil, expression);
         return new Block(assignToField);
      }
      else
      {
         throw _expression.Exception;
      }
   }

   protected List<Statement> statements;
   protected Maybe<TypeConstraint> _typeConstraint;

   public Block(List<Statement> statements, Maybe<TypeConstraint> _typeConstraint)
   {
      this.statements = statements;
      this._typeConstraint = _typeConstraint;
   }

   public Block(List<Statement> statements)
   {
      this.statements = statements;
      _typeConstraint = nil;
   }

   public Block(Statement statement, Maybe<TypeConstraint> _typeConstraint)
   {
      statements = [statement];
      this._typeConstraint = _typeConstraint;
   }

   public Block(Statement statement)
   {
      statements = [statement];
      _typeConstraint = nil;
   }

   public Block(Expression expression) : this(new Return(expression, nil))
   {
   }

   public Block()
   {
      statements = [];
      _typeConstraint = nil;
   }

   public bool Yielding { get; set; }

   public override void Generate(OperationsBuilder builder)
   {
      foreach (var statement in statements)
      {
         statement.Generate(builder);
      }

      if (Yielding)
      {
         builder.PushNone();
         builder.Return(true);
      }
      else if (_typeConstraint is (true, var typeConstraint))
      {
         builder.ReturnType(true, typeConstraint);
      }
   }

   public IEnumerator<Statement> GetEnumerator() => statements.GetEnumerator();

   public override string ToString() => statements.ToString("\r\n");

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Add(Statement statement) => statements.Add(statement);

   public void AddReturnIf()
   {
      if (statements.Count > 0 && statements[^1] is not ReturnNothing)
      {
         statements.Add(new ReturnNothing());
      }
   }

   public void AddReturnIf(Symbol symbol)
   {
      if (statements.Count > 0 && statements[^1] is not Return)
      {
         var expression = new Expression(symbol);
         statements.Add(new Return(expression, nil));
      }
   }

   public Maybe<Expression> ExpressionStatement(bool returns)
   {
      if (statements.Count > 0 && statements[0] is ExpressionStatement expressionStatement &&
          expressionStatement.ReturnExpression == returns)
      {
         return expressionStatement.Expression;
      }
      else
      {
         return nil;
      }
   }
}