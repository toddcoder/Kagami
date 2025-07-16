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

      var state = new ParseState(source);
      var statementsParser = new StatementsParser();

      ResetFieldUniqueID();
      ResetObjectUniqueID();

      while (state.More)
      {
         var _scan = statementsParser.Scan(state);
         if (_scan)
         {
         }
         else if (_scan.Exception is (true, var exception))
         {
            ExceptionIndex = state.ExceptionIndex;
            return exception;
         }
         else
         {
            ExceptionIndex = state.CurrentSource.Length;
            return fail($"Didn't understand {state.CurrentSource}");
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
      List<Function> functions = [];
      List<Class> classes = [];
      List<Statement> others = [];

      foreach (var statement in statements)
      {
         switch (statement)
         {
            case Function function:
               functions.Add(function);
               break;
            case Class cls:
               classes.Add(cls);
               break;
            default:
               others.Add(statement);
               break;
         }
      }

      foreach (var function in functions)
      {
         yield return function;
      }

      foreach (var @class in classes)
      {
         yield return @class;
      }

      foreach (var statement in others)
      {
         yield return statement;
      }
   }

   public Result<Unit> Colorize()
   {
      Module.Global.ActivateWith(() => new Module());

      var state = new ParseState(source);
      var statementsParser = new StatementsParser();

      ResetFieldUniqueID();
      ResetObjectUniqueID();

      while (state.More)
      {
         var _scan = statementsParser.Scan(state);
         if (_scan)
         {
         }
         else if (_scan.Exception is (true, var exception))
         {
            ExceptionIndex = state.ExceptionIndex;
            return exception;
         }
         else
         {
            ExceptionIndex = state.CurrentSource.Length;
            return fail($"Didn't understand {state.CurrentSource}");
         }
      }

      Tokens = state.Tokens;

      return unit;
   }

   public Maybe<int> ExceptionIndex { get; set; } = nil;

   public Token[] Tokens { get; set; } = [];

   public Maybe<Operations.Operations> Operations { get; set; } = nil;
}