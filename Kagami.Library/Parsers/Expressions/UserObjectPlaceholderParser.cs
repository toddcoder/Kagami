using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class UserObjectPlaceholderParser : SymbolParser
{
   public UserObjectPlaceholderParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex($@"^(\s*)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var name = tokens[2].Text;
      var hasArguments = tokens[3].Text == "(";
      state.Colorize(tokens, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      if (Module.IsBuiltInClass(name))
      {
         builder.Add(new ClassSymbol(name));
         return unit;
      }

      if (hasArguments)
      {
         var _arguments = getArguments(state, builder.Flags);
         if (_arguments is (true, var arguments))
         {
            var userObjectPlaceholder = new UserObjectPlaceholder(name);
            builder.Add(new PushUserObjectPlaceholder(userObjectPlaceholder, arguments));

            return unit;
         }
         else
         {
            return _arguments.Exception;
         }
      }
      else
      {
         var userObjectPlaceholder = new UserObjectPlaceholder(name);
         builder.Add(new PushUserObjectPlaceholder(userObjectPlaceholder, []));

         return unit;
      }
   }
}