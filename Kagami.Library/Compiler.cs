using Kagami.Library.Operations;
using Kagami.Library.Parsers;
using Kagami.Library.Parsers.Statements;
using Kagami.Library.Runtime;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using Pattern = Core.Matching.Pattern;

namespace Kagami.Library;

public class Compiler
{
   static Compiler()
   {
      Pattern.IsFriendly = true;
   }

   protected string source;
   protected CompilerConfiguration configuration;
   protected IContext context;

   public Compiler(string source, CompilerConfiguration configuration, IContext context)
   {
      this.source = source;
      this.configuration = configuration;
      this.context = context;
   }

   public Result<Machine> Generate()
   {
      Module.Global.ActivateWith(() => new Module());

      var state = new ParseState(source)
      {
         AllowPrintStatement = configuration.AllowPrintStatement
      };
      var statementsParser = new StatementsParser();

      ResetFieldUniqueID();

      while (state.More)
      {
         var _scan = statementsParser.Scan(state);
         if (_scan)
         {
         }
         else if (_scan.Exception is (true, var exception))
         {
            ExceptionIndex = state.ExceptionIndex;
            ErrorLocation = (state.Line, state.Character);

            return state.Exception | exception;
         }
         else
         {
            ExceptionIndex = state.CurrentSource.Length;
            ErrorLocation = (state.Line, state.Character);

            return state.Exception | (() => fail($"Didn't understand {state.CurrentSource}"));
         }
      }

      Tokens = state.Tokens;

      var statements = reorderStatements(state.Statements());
      var builder = new OperationsBuilder();
      foreach (var statement in statements)
      {
         statement.Generate(builder);
         statement.AddBreak(builder);
      }

      var _operations = builder.ToOperations(state);
      if (_operations is (true, var operations))
      {
         var machine = new Machine(context) { Tracing = configuration.Tracing };
         machine.Load(operations);
         Machine.Current.ActivateWith(() => machine);
         Operations = operations;

         return machine;
      }
      else
      {
         return _operations.Exception;
      }
   }

   protected IEnumerable<Statement> reorderStatements(IEnumerable<Statement> statements)
   {
      List<Statement> earlyStatements = [];
      List<Statement> others = [];

      foreach (var statement in statements)
      {
         switch (statement)
         {
            case Function { IsFixed: false } function:
               earlyStatements.Add(function);
               break;
            case MatchFunction { IsFixed: false } matchFunction:
               earlyStatements.Add(matchFunction);
               break;
            case Class cls:
               earlyStatements.Add(cls);
               break;
            default:
               others.Add(statement);
               break;
         }
      }

      foreach (var statement in earlyStatements)
      {
         yield return statement;
      }

      foreach (var statement in others)
      {
         yield return statement;
      }
   }

   public Result<Unit> Colorize()
   {
      Module.Global.ActivateWith(() => new Module());

      var state = new ParseState(source) { AllowPrintStatement = configuration.AllowPrintStatement };
      var statementsParser = new StatementsParser();

      ResetFieldUniqueID();

      while (state.More)
      {
         var _scan = statementsParser.Scan(state);
         if (_scan)
         {
         }
         else if (_scan.Exception is (true, var exception))
         {
            ExceptionIndex = state.ExceptionIndex;
            ErrorLocation = (state.Line, state.Character);

            return exception;
         }
         else
         {
            ExceptionIndex = state.CurrentSource.Length;
            ErrorLocation = (state.Line, state.Character);

            return fail($"Didn't understand {state.CurrentSource}");
         }
      }

      Tokens = state.Tokens;

      return unit;
   }

   public Maybe<int> ExceptionIndex { get; set; } = nil;

   public Token[] Tokens { get; set; } = [];

   public Maybe<Operations.Operations> Operations { get; set; } = nil;

   public Maybe<(int line, int character)> ErrorLocation { get; set; } = nil;
}