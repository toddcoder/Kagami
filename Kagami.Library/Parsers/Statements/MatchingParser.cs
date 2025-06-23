using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class MatchingParser(string containerName, Block commonBlock) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(when)(\s+)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      var hasParameters = tokens[5].Text == "(";
      HasParameters = hasParameters;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      className = $"{containerName}${className}";
      Module.Global.Value.ForwardReference(className);

      Parameters parameters;

      if (hasParameters)
      {
         var _parameters = getParameters(state);
         if (_parameters)
         {
            parameters = _parameters;
         }
         else if (_parameters.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            parameters = new Parameters(0);
         }
      }
      else
      {
         parameters = Parameters.Empty;
      }

      var builder = new ClassBuilder(className, parameters, "", [], false, commonBlock);
      var _registered = builder.Register();
      if (_registered)
      {
         var cls = new Class(builder);
         state.AddStatement(cls);

         Matching = cls;
      }
      else
      {
         return _registered.Exception;
      }

      return unit;
   }

   public Maybe<Class> Matching { get; set; } = nil;

   public bool HasParameters { get; set; }
}