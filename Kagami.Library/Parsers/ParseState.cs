using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Expressions;
using Core.Arrays;
using Core.Collections;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers
{
	public class ParseState : IEnumerable<Statement>
	{
		string source;
		int index;
		Stack<string> indentations;
		string indentation;
		List<Statement> statements;
		Stack<List<Statement>> statementStack;
		List<Token> tokens;
		IndexTransaction indexTransaction;
		TokenTransaction tokenTransaction;
		List<Symbol> postGenerationSymbols;
		IMaybe<int> exceptionIndex;
		Stack<bool> yieldingStack;
		Stack<IMaybe<TypeConstraint>> returnTypesStack;
		Hash<string, Expression> defExpressions;
		Hash<string, Function> macros;

		public ParseState(string source)
		{
			this.source = source;
			index = 0;
			indentations = new Stack<string>();
			indentation = "";
			statements = new List<Statement>();
			statementStack = new Stack<List<Statement>>();
			tokens = new List<Token>();
			indexTransaction = new IndexTransaction();
			tokenTransaction = new TokenTransaction(tokens);
			postGenerationSymbols = new List<Symbol>();
			exceptionIndex = none<int>();
			yieldingStack = new Stack<bool>();
			returnTypesStack = new Stack<IMaybe<TypeConstraint>>();
			defExpressions = new Hash<string, Expression>();
			macros = new Hash<string, Function>();
		}

		public IMaybe<int> ExceptionIndex
		{
			get => exceptionIndex;
			set => exceptionIndex = value;
		}

		public void SetExceptionIndex() => exceptionIndex = index.Some();

		public Statement[] Statements() => statements.ToArray();

		public void BeginTransaction()
		{
			indexTransaction.Begin(index);
			tokenTransaction.Begin();
		}

		public void RollBackTransaction()
		{
			index = indexTransaction.RollBack();
			tokenTransaction.RollBack();
		}

		public void CommitTransaction()
		{
			indexTransaction.Commit();
			tokenTransaction.Commit();
		}

		public IMatched<Unit> RollBackTransactionIf<T>(bool enabled, IMatched<T> previousMatch)
		{
			if (enabled && !previousMatch.IsMatched)
				RollBackTransaction();

			return previousMatch.Map(t => Unit.Value);
		}

		public void PushStatements()
		{
			statementStack.Push(statements);
			statements = new List<Statement>();
		}

		public IResult<List<Statement>> PopStatements() => tryTo(() =>
		{
			var returnStatements = statements;
			statements = new List<Statement>(statementStack.Pop());
			return returnStatements;
		});

		public bool More => index < source.Length;

		public void AddStatement(Statement statement)
		{
			statement.Index = index;

			if (ForExpression.If(out var tuple))
			{
				var (fieldName, expression) = tuple;
				statements.Add(new For2(new PlaceholderSymbol("+" + fieldName), expression, new Block(statement)));
				ForExpression = none<(string, Expression)>();
			}
			else
				statements.Add(statement);
		}

		public void AddToken(int index, int length, Color color)
		{
			tokens.Add(new Token(index, length, " ".Repeat(length)) { Color = color });
		}

		public void AddToken(Color color, int length = 1) => AddToken(index, length, color);

		public string Indentation => indentation;

		public IMatched<Unit> Advance()
		{
			SkipEndOfLine();
			if (Scan($"{Indentation.FriendlyString()} /(/s+)").If(out var newIndentation, out var mbException))
			{
				PushIndentation(newIndentation);
				return Unit.Matched();
			}
			else if (mbException.If(out var exception))
			{
				exceptionIndex = index.Some();
				return failedMatch<Unit>(exception);
			}
			else
			{
				exceptionIndex = index.Some();
				return failedMatch<Unit>(badIndentation());
			}
		}

		public void Regress() => PopIndentation();

		public IMatched<Unit> Retreat()
		{
			try
			{
				PopIndentation();
				return Unit.Matched();
			}
			catch (Exception exception)
			{
				return failedMatch<Unit>(exception);
			}
		}

		public void PushIndentation(string text)
		{
			indentations.Push(indentation);
			indentation = text;
		}

		public void PopIndentation() => indentation = indentations.Pop();

		public int Index => index;

		public string Source => source;

		public string CurrentSource => source.Drop(index);

		public string RealizePattern(string pattern)
		{
			var previous = indentations.Count > 0 ? indentations.Peek() : "";
			return pattern.Replace(("|i|", indentation), ("|i-|", previous), ("|s|", "[' ' /t]*"), ("|s+|", "[' ' /t]+"),
				("|tl|", "[/t /r /n]*"), ("|tl+|", "[/t /r /n]+"));
		}

		public IEnumerator<Statement> GetEnumerator() => statements.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Move(int length) => index += length;

		public void Move(Token[] tokens)
		{
			if (tokens.Length != 0)
				Move(tokens[0].Length);
		}

		public Token[] Tokens => tokens.ToArray();

		public IMatched<string> Scan(string pattern, params Color[] colors)
		{
			return CurrentSource.MatchOne(RealizePattern(pattern)).Map(m =>
			{
				var groupArray = m.Groups.Slice(1, m.Groups.Length - 1);
				for (var i = 0; i < Math.Min(groupArray.Length, colors.Length); i++)
				{
					var length = groupArray[i].Length;
					AddToken(colors[i], length);
					Move(length);
				}

				return m.Text.Matched();
			});
		}

		public IMatched<string> SkipEndOfLine() => Scan("/(^ /r /n | ^ /r | ^ /n)", Color.Whitespace);

		public void Colorize(Token[] tokens, params Color[] colors)
		{
			for (var i = 0; i < colors.Length; i++)
				tokens[i + 1].Color = colors[i];
			foreach (var token in tokens.Skip(1))
				this.tokens.Add(token);
			Move(tokens);
		}

		public void AddSymbol(Symbol symbol) => postGenerationSymbols.Add(symbol);

		public List<Symbol> PostGenerationSymbols => postGenerationSymbols;

		public void UpdateStatement(int index, int length)
		{
			var count = statements.Count;

			if (count > 0)
			{
				var statement = statements[count - 1];
				statement.Index = index;
				statement.Length = length;
			}
		}

		public Statement LastStatement => statements[statements.Count - 1];

		public IMatched<IMaybe<Expression>> getAnd()
		{
			var builder = new ExpressionBuilder(ExpressionFlags.OmitIf);
			var parser = new IfAsAndParser(builder);
			if (parser.Scan(this).If(out _, out var mbException))
				if (builder.ToExpression().If(out var expression, out var exception))
					return expression.Some().Matched();
				else
					return failedMatch<IMaybe<Expression>>(exception);
			else if (mbException.If(out var exception))
				return failedMatch<IMaybe<Expression>>(exception);
			else
				return none<Expression>().Matched();
		}

		public void CreateYieldFlag() => yieldingStack.Push(false);

		public void CreateReturnType() => returnTypesStack.Push(none<TypeConstraint>());

		public void SetYieldFlag()
		{
			yieldingStack.Pop();
			yieldingStack.Push(true);
		}

		public void SetReturnType(IMaybe<TypeConstraint> typeConstraint)
		{
			returnTypesStack.Pop();
			returnTypesStack.Push(typeConstraint);
		}

		public bool RemoveYieldFlag() => yieldingStack.Pop();

		public IMaybe<TypeConstraint> GetReturnType() => returnTypesStack.Peek();

		public void RemoveReturnType() => returnTypesStack.Pop();

		public void RegisterDefExpression(string fieldName, Expression expression) => defExpressions[fieldName] = expression;

		public IMaybe<Expression> DefExpression(string fieldName) => defExpressions.Map(fieldName);

		public void RegisterMacro(Function function) => macros[function.Selector] = function;

		public IMaybe<Function> Macro(string fullFunctionName) => macros.Map(fullFunctionName);

		public bool BlockFollows() => CurrentSource.IsMatch($"^ ':' (/r /n | /r | /n) '{indentation}' [' /t']+", multiline: true);

		public IMaybe<(string, Expression)> ForExpression { get; set; } = none<(string, Expression)>();

		public IMaybe<(string, Symbol)> MapExpression { get; set; } = none<(string, Symbol)>();

		public IMaybe<(string, Symbol)> IfExpression { get; set; } = none<(string, Symbol)>();

		public IMaybe<(string, Symbol)> LeftZipExpression { get; set; } = none<(string, Symbol)>();

		public IMaybe<(string, Symbol)> RightZipExpression { get; set; } = none<(string, Symbol)>();

		public IMaybe<(string, Symbol)> FoldExpression { get; set; } = none<(string, Symbol)>();

		public IMaybe<(string, Symbol)> BindExpression { get; set; } = none<(string, Symbol)>();
	}
}