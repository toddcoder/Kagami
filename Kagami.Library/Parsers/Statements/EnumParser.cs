using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class EnumParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(type)(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      Module.Global.Value.ForwardReference(className);

      var _beginBlock = state.BeginBlock();
      if (!_beginBlock)
      {
         return _beginBlock.Exception;
      }

      List<EnumMemberData> enumMembers = [];
      Optional<Unit> _endBlock = nil;
      Maybe<IObject> _ordinal = nil;
      var scanning = true;

      while (state.More && scanning)
      {
         _endBlock = state.EndBlock();
         if (_endBlock)
         {
            break;
         }
         else if (_endBlock.Exception is (true, var exception))
         {
            return exception;
         }

         var enumMemberParser = new EnumMemberParser(className, _ordinal);
         var _enumMember = enumMemberParser.Scan(state);
         if (_enumMember)
         {
            if (enumMemberParser.EnumMemberData is (true, var enumMemberData))
            {
               enumMembers.Add(enumMemberData);
            }

            _ordinal = enumMemberParser.Ordinal;
         }
         else if (_enumMember.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            while (state.More && scanning)
            {
               var enumNextMemberParser = new EnumNextMemberParser(className, _ordinal);
               _enumMember = enumNextMemberParser.Scan(state);
               if (_enumMember)
               {
                  if (enumNextMemberParser.EnumMemberData is (true, var enumMemberData))
                  {
                     enumMembers.Add(enumMemberData);
                  }

                  _ordinal = enumNextMemberParser.Ordinal;
               }
               else if (_enumMember.Exception is (true, var exception2))
               {
                  return exception2;
               }
               else
               {
                  scanning = false;
               }
            }
         }
      }

      Maybe<Block> _block;
      if (_endBlock)
      {
         _block = nil;
      }
      else
      {
         _block = getPartialBlock(state).Maybe();
      }

      var commonBlock = _block | (() => new Block());

      var enumCreator = new EnumCreator(className, [.. enumMembers], commonBlock);
      var _result = enumCreator.Create();
      if (_result)
      {
         state.AddStatement(enumCreator);
      }

      return _result;
   }
}