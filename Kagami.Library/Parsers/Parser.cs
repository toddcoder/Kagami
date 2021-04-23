using System;
using System.Linq;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers
{
   public abstract class Parser
   {
      protected bool updateLastStatement;

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
         IMaybe<Exception> _exception;

         if (Pattern.IsEmpty())
         {
            var index = state.Index;
            var parsed = Parse(state, new Token[0]);
            if (parsed.If(out _, out _exception))
            {
               if (UpdateIndexOnParseOnly)
               {
                  state.UpdateStatement(index, 1);
               }
            }
            else if (_exception.IsSome)
            {
               state.SetExceptionIndex();
            }

            return parsed;
         }

         if (new Matcher().MatchOne(state.CurrentSource, state.RealizePattern(Pattern), IgnoreCase, Multiline).If(out var match, out _exception))
         {
            var index = state.Index;
            var parsed = Parse(state, GetTokens(state, match));
            if (parsed.IsMatched && updateLastStatement)
            {
               state.UpdateStatement(index, match.Length);
            }

            if (parsed.IsFailedMatch)
            {
               state.SetExceptionIndex();
            }

            return parsed;
         }
         else if (_exception.If(out var exception))
         {
            state.SetExceptionIndex();
            return failedMatch<Unit>(exception);
         }
         else
         {
            return notMatched<Unit>();
         }
      }

      public virtual bool UpdateIndexOnParseOnly => false;
   }
}