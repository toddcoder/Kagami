﻿using System;
using System.Linq;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers
{
   public abstract class Parser
   {
      protected bool updateLastStatement;

      protected Parser(bool updateLastStatement) => this.updateLastStatement = updateLastStatement;

      public static Token[] GetTokens(ParseState state, Match match)
      {
         return match.Groups.Select(g => new Token(state.Index + g.Index, g.Length, g.Text)).ToArray();
      }

      public virtual string Pattern => "";

      public virtual bool IgnoreCase => false;

      public virtual bool Multiline => false;

      public abstract Responding<Unit> Parse(ParseState state, Token[] tokens);

      public virtual Responding<Unit> Scan(ParseState state)
      {
         if (Pattern.IsEmpty())
         {
            var index = state.Index;
            var _parsed = Parse(state, Array.Empty<Token>());
            if (_parsed)
            {
               if (UpdateIndexOnParseOnly)
               {
                  state.UpdateStatement(index, 1);
               }
            }
            else if (_parsed.AnyException)
            {
               state.SetExceptionIndex();
            }

            return _parsed;
         }

         Pattern pattern = state.RealizePattern(Pattern);
         pattern = pattern.WithIgnoreCase(IgnoreCase).WithMultiline(Multiline);
         var _result = state.CurrentSource.Matches(pattern);

         if (_result)
         {
            var match = _result.Value.Matches[0];
            var index = state.Index;
            var _parsed = Parse(state, GetTokens(state, match));
            if (_parsed && updateLastStatement)
            {
               state.UpdateStatement(index, match.Length);
            }

            if (_parsed.AnyException)
            {
               state.SetExceptionIndex();
            }

            return _parsed;
         }
         else
         {
            return nil;
         }
      }

      public virtual bool UpdateIndexOnParseOnly => false;
   }
}