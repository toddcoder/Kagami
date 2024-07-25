using System;
using Kagami.Library.Operations;
using Kagami.Library.Parsers;
using Kagami.Library.Parsers.Statements;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library;

public class Compiler(string source, CompilerConfiguration configuration, IContext context)
{
   public Result<Machine> Generate()
   {
      Module.Global = new Module();
      Module.Global.LoadBuiltinClasses();

      var state = new ParseState(source);
      var statementsParser = new StatementsParser();

      ResetUniqueID();

      resetUniqueID();

      while (state.More)
      {
         if (statementsParser.Scan(state).If(out _, out var _exception))
         {
         }
         else if (_exception.If(out var innerException))
         {
            ExceptionIndex = state.ExceptionIndex;
            return innerException;
         }
         else
         {
            ExceptionIndex = state.CurrentSource.Length;
            return fail($"Didn't understand {state.CurrentSource}");
         }
      }

      Tokens = state.Tokens;

      var statements = state.Statements();
      var builder = new OperationsBuilder();
      foreach (var statement in statements)
      {
         statement.Generate(builder);
         statement.AddBreak(builder);
      }

      if (builder.ToOperations(state).Map(out var operations, out var exception))
      {
         var machine = new Machine(context) { Tracing = configuration.Tracing };
         machine.Load(operations);
         Machine.Current = machine;
         Operations = operations;
         return machine;
      }
      else
      {
         return exception;
      }
   }

   public Maybe<int> ExceptionIndex { get; set; } = nil;

   public Token[] Tokens { get; set; } = Array.Empty<Token>();

   public Maybe<Operations.Operations> Operations { get; set; } = nil;
}