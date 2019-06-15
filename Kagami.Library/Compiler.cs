using Kagami.Library.Operations;
using Kagami.Library.Parsers;
using Kagami.Library.Parsers.Statements;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library
{
	public class Compiler
	{
		string source;
		CompilerConfiguration configuration;
		IContext context;

		public Compiler(string source, CompilerConfiguration configuration, IContext context)
		{
			this.source = source;
			this.configuration = configuration;
			this.context = context;
		}

		public IResult<Machine> Generate()
		{
			Module.Global = new Module();
			Module.Global.LoadBuiltinClasses();

			var state = new ParseState(source);
			var statementsParser = new StatementsParser();

			ResetUniqueID();

			resetUniqueID();

			while (state.More)
				if (statementsParser.Scan(state).If(out _, out var anyException)) { }
				else if (anyException.If(out var innerException))
				{
					ExceptionIndex = state.ExceptionIndex;
					return failure<Machine>(innerException);
				}
				else
				{
					ExceptionIndex = state.CurrentSource.Length.Some();
					return $"Didn't understand {state.CurrentSource}".Failure<Machine>();
				}

			Tokens = state.Tokens;

			var statements = state.Statements();
			var builder = new OperationsBuilder();
			foreach (var statement in statements)
			{
				statement.Generate(builder);
				statement.AddBreak(builder);
			}

			if (builder.ToOperations(state).If(out var operations, out var exception))
			{
				var machine = new Machine(context) { Tracing = configuration.Tracing };
				machine.Load(operations);
				Machine.Current = machine;
				Operations = operations.Some();
				return machine.Success();
			}
			else
				return failure<Machine>(exception);
		}

		public IMaybe<int> ExceptionIndex { get; set; } = none<int>();

		public Token[] Tokens { get; set; } = new Token[0];

		public IMaybe<Operations.Operations> Operations { get; set; } = none<Operations.Operations>();
	}
}