using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ProtocolParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(protocol)(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var protocolName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);
      var builder = new ProtocolBuilder(protocolName);
      Module.Global.Value.ForwardReference(protocolName);

      var inheritedInclusionsParser = new InheritedProtocolsParser(builder);
      Optional<Unit> _result;
      while (state.More)
      {
         _result = inheritedInclusionsParser.Scan(state);
         if (_result)
         {
         }
         else if (_result.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      _result = state.BeginBlock();
      if (!_result)
      {
         return _result.Exception;
      }

      var inclusionMembersParser = new ProtocolMembersParser(builder);
      while (state.More)
      {
         _result = inclusionMembersParser.Scan(state);
         if (_result)
         {
         }
         else if (_result.Exception is (true, var exception2))
         {
            return exception2;
         }
         else
         {
            break;
         }
      }

      _result = state.EndBlock();
      if (_result)
      {
         var _protocol = builder.Build();
         if (_protocol is (true, var protocol))
         {
            Protocols.Protocols.Set(protocolName, protocol);
         }
         else if (_protocol.Exception is (true, var exception))
         {
            return exception;
         }

         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}