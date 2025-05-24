using Core.Monads;

namespace Kagami.Library.Parsers;

public abstract class PatternlessParser : Parser
{
   public PatternlessParser(bool updateLastStatement) : base(updateLastStatement)
   {
   }

   public override Optional<Unit> Scan(ParseState state)
   {
      var index = state.Index;
      var _parsed = Parse(state, []);
      if (_parsed)
      {
         if (UpdateIndexOnParseOnly)
         {
            state.UpdateStatement(index, 1);
         }
      }
      else if (_parsed.Exception)
      {
         state.SetExceptionIndex();
      }

      return _parsed;
   }
}