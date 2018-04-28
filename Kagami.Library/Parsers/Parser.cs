using System;
using System.Linq;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers
{
   public abstract class Parser
   {
      bool updateLastStatement;

      protected Parser(bool updateLastStatement) => this.updateLastStatement = updateLastStatement;

      public static Token[] GetTokens(ParseState state, Matcher.Match match)
      {
         return match.Groups.Select(g => new Token(state.Index + g.Index, g.Length, g.Text)).ToArray();
      }

      public virtual string Pattern => "";

      public virtual bool IgnoreCase => false;

      public virtual bool Multiline => false;

      public abstract IMatched<Unit> Parse(ParseState state, Token[] tokens);

      public virtual IMatched<Unit> Scan(ParseState state)
      {
         if (Pattern.IsEmpty())
         {
            var index = state.Index;
            var parsed = Parse(state, new Token[0]);
            (var parsedType, _, _) = parsed.Values;
            switch (parsedType)
            {
               case MatchType.Matched:
                  if (UpdateIndexOnParseOnly)
                     state.UpdateStatement(index, 1);
                  break;
               case MatchType.FailedMatch:
                  state.SetExceptionIndex();
                  break;
            }

            return parsed;
         }

         var matcher = new Matcher();
         (var type, var match, var exception) = matcher.MatchOne(state.CurrentSource, state.RealizePattern(Pattern), IgnoreCase,
            Multiline).Values;
         switch (type)
         {
            case MatchType.Matched:
               var index = state.Index;
               var parsed = Parse(state, GetTokens(state, match));
               if (parsed.IsMatched && updateLastStatement)
                  state.UpdateStatement(index, match.Length);
               if (parsed.IsFailedMatch)
                  state.SetExceptionIndex();
               return parsed;
            case MatchType.NotMatched:
               return notMatched<Unit>();
            case MatchType.FailedMatch:
               state.SetExceptionIndex();
               return failedMatch<Unit>(exception);
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      public virtual bool UpdateIndexOnParseOnly => false;
   }
}