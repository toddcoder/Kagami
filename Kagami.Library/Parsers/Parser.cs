using System.Text.RegularExpressions;
using Core.Monads;

namespace Kagami.Library.Parsers;

public abstract class Parser
{
   protected bool updateLastStatement;

   protected Parser(bool updateLastStatement) => this.updateLastStatement = updateLastStatement;

   public static Token[] GetTokens(ParseState state, Match match)
   {
      return [.. match.AllGroups().Select(g => new Token(state.Index + g.Index, g.Length, g.Value))];
   }

   [Obsolete("Use Regex method")]
   public virtual string Pattern => "";

   public virtual bool IgnoreCase => false;

   public virtual bool Multiline => false;

   public abstract Optional<Unit> Parse(ParseState state, Token[] tokens);

   public abstract Optional<Unit> Scan(ParseState state);

   public virtual bool UpdateIndexOnParseOnly => false;
}