using System.Collections;
using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Statements
{
	public class Block : Statement, IEnumerable<Statement>
	{
		public static Block Getter(string fieldName)
		{
			var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
			expressionBuilder.Add(new FieldSymbol(fieldName));
			if (expressionBuilder.ToExpression().If(out var expression, out var exception))
				return new Block(new Return(expression, none<TypeConstraint>()));
			else
				throw exception;
		}

		public static Block Setter(string fieldName, string parameterName)
		{
			var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
			expressionBuilder.Add(new FieldSymbol(parameterName));
			if (expressionBuilder.ToExpression().If(out var expression, out var exception))
			{
				var assignToField = new AssignToField(fieldName, none<Operation>(), expression);
				return new Block(assignToField);
			}
			else
				throw exception;
		}

		List<Statement> statements;
		IMaybe<TypeConstraint> typeConstraint;

		public Block(List<Statement> statements, IMaybe<TypeConstraint> typeConstraint)
		{
			this.statements = statements;
			this.typeConstraint = typeConstraint;
		}

		public Block(List<Statement> statements)
		{
			this.statements = statements;
			typeConstraint = none<TypeConstraint>();
		}

		public Block(Statement statement, IMaybe<TypeConstraint> typeConstraint)
		{
			statements = new List<Statement> { statement };
			this.typeConstraint = typeConstraint;
		}

		public Block(Statement statement)
		{
			statements = new List<Statement> { statement };
			typeConstraint = none<TypeConstraint>();
		}

		public Block(Expression expression) : this(new Return(expression, none<TypeConstraint>())) { }

		public Block()
		{
			statements = new List<Statement>();
			typeConstraint = none<TypeConstraint>();
		}

		public bool Yielding { get; set; }

		public override void Generate(OperationsBuilder builder)
		{
			foreach (var statement in statements)
				statement.Generate(builder);

			if (Yielding)
			{
				builder.PushNone();
				builder.Return(true);
			}
			else if (typeConstraint.If(out var tc))
				builder.ReturnType(true, tc);
		}

		public IEnumerator<Statement> GetEnumerator() => statements.GetEnumerator();

		public override string ToString() => statements.Stringify("\r\n");

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(Statement statement) => statements.Add(statement);

		public void AddReturnIf()
		{
			if (!(statements[statements.Count - 1] is ReturnNothing))
				statements.Add(new ReturnNothing());
		}

		public void AddReturnIf(Symbol symbol)
		{
			if (!(statements[statements.Count - 1] is Return))
			{
				var expression = new Expression(symbol);
				statements.Add(new Return(expression, none<TypeConstraint>()));
			}
		}

		public IMaybe<Expression> ExpressionStatement(bool returns)
		{
			if (statements.Count > 0 && statements[0] is ExpressionStatement expressionStatement &&
				expressionStatement.ReturnExpression == returns)
				return expressionStatement.Expression.Some();
			else
				return none<Expression>();
		}
	}
}