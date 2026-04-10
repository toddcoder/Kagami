using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class InheritedProtocolsParser(ProtocolBuilder builder) : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(:)(\s*)({REGEX_CLASS})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var protocolName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Structure, Color.Whitespace, Color.Class);

      if (Protocols.Protocols.Get(protocolName) is (true, var otherProtocol))
      {
         builder.AddProtocol(otherProtocol);
      }
      else
      {
         return protocolNotFound(protocolName);
      }

      while (state.More)
      {
         var _nextProtocolName =
            from prefix in state.Scan(@"^(\s*)(,)(\s*)", Color.Whitespace, Color.Structure, Color.Whitespace)
            from name in state.Scan(@$"^({REGEX_CLASS})\b", Color.Class)
            select name;
         if (_nextProtocolName is (true, var nextProtocolName))
         {
            if (Protocols.Protocols.Get(nextProtocolName) is (true, var nextProtocol))
            {
               builder.AddProtocol(nextProtocol);
            }
            else
            {
               return protocolNotFound(protocolName);
            }
         }
         else if (_nextProtocolName.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      return unit;
   }
}