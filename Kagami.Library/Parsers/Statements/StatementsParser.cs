using Kagami.Library.Objects;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class StatementsParser : MultiParser
{
   protected EndOfLineParser endOfLineParser = new();

   public override IEnumerable<Parser> Parsers
   {
      get
      {
         yield return new MultilineCommentParser();
         yield return new NullStatementParser();
         yield return new CommentParser();
         yield return new ClassParser();
         yield return new NamedStaticParser();
         yield return new InclusionParser();
         yield return new IncludeParser();
         yield return new ModuleParser();
         yield return new RecordParser();
         yield return new PatternParser();
         yield return new ConditionalAssignParser();
         yield return new ConditionalWhileParser();
         yield return new IfParser();
         yield return new GuardParser();
         yield return new WhileParser();
         yield return new ForParser();
         yield return new RepeatParser();
         yield return new ExitContinueParser();

         yield return new MatchParser();
         yield return new YieldParser();
         yield return new ReturnParser();
         yield return new ReturnNothingParser();
         yield return new StopParser();
         yield return new DeferParser();
         yield return new AssignFromBlockParser();
         yield return new AssignFromLoopParser();
         yield return new MatchAssignParser();
         yield return new AssignToNewFieldParser();

         yield return new AssignToMatchParser();
         yield return new DefAssignParser();
         yield return new DataTypeParser();
         yield return new AliasParser();
         yield return new LoopParser();
         yield return new BlockStatementParser();
         yield return new ImportPackageParser();
         yield return new OpenPackageParser();
         yield return new UsePackageParser();

         yield return new AssignToFieldParser();

         yield return new FunctionParser();

         yield return new ExpressionStatementParser(ReturnExpression, TypeConstraint);
      }
   }

   public bool ReturnExpression { get; set; }

   public Maybe<TypeConstraint> TypeConstraint { get; set; } = nil;

   public override Optional<Unit> Parse(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();
      var _result = endOfLineParser.Scan(state);
      if (_result)
      {
         state.CommitTransaction();
      }
      else if (_result.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         state.CommitTransaction();
      }

      return base.Parse(state, tokens);
   }
}