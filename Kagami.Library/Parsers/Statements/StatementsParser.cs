using System.Collections.Generic;

namespace Kagami.Library.Parsers.Statements
{
   public class StatementsParser : MultiParser
   {
      bool singleLine;

      public StatementsParser(bool singleLine = false) => this.singleLine = singleLine;

      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new EndOfLineParser();
            if (!singleLine)
            {
               yield return new ClassParser();
               yield return new TraitParser();
               yield return new RecordParser();
               yield return new ConditionalAssignParser();
               yield return new ConditionalWhileParser();
               yield return new IfParser();
               yield return new WhileParser();
               yield return new ForParser2();
               yield return new MatchFunctionParser();
               yield return new MatchParser();
               yield return new YieldParser();
               yield return new ReturnNothingParser();
               yield return new ReturnParser();
               yield return new DeferParser();
               yield return new AssignToNewFieldParser();
               yield return new AssignToNewFieldParser2();
               yield return new AssignToMatchParser();
               yield return new DefAssignParser();
               yield return new DataTypeParser();
               yield return new AliasParser();
            }

            yield return new AssignToFieldParser { SingleLine = singleLine };

            if (!singleLine)
               yield return new FunctionParser();

            yield return new PrintParser { SingleLine = singleLine };
            yield return new PrintLnParser { SingleLine = singleLine };
            yield return new PutParser { SingleLine = singleLine };

            if (!singleLine)
            {
               yield return new PassParser();
               yield return new ExitParser();
               yield return new SkipParser();
            }

            yield return new ExpressionStatementParser(ReturnExpression) { SingleLine = singleLine };
         }
      }

      public bool ReturnExpression { get; set; }
   }
}