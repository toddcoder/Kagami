using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class TypeParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(type|error)(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isError = tokens[2].Text == "error";
      var className = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      Module.Global.Value.ForwardReference(className);

      var _beginBlock = state.Scan(@"^(\s*)(\{)", Color.Whitespace, Color.Block);
      if (!_beginBlock)
      {
         return _beginBlock.Exception;
      }

      List<TypeMemberData> typeMembers = [];
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

         var typeMemberParser = new TypeMemberParser(_ordinal);
         var _typeMember = typeMemberParser.Scan(state);
         if (_typeMember)
         {
            if (typeMemberParser.TypeMemberData is (true, var enumMemberData))
            {
               typeMembers.Add(enumMemberData);
            }

            _ordinal = typeMemberParser.Ordinal;
         }
         else if (_typeMember.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            while (state.More && scanning)
            {
               var typeNextMemberParser = new TypeNextMemberParser(_ordinal);
               _typeMember = typeNextMemberParser.Scan(state);
               if (_typeMember)
               {
                  if (typeNextMemberParser.TypeMemberData is (true, var enumMemberData))
                  {
                     typeMembers.Add(enumMemberData);
                  }

                  _ordinal = typeNextMemberParser.Ordinal;
               }
               else if (_typeMember.Exception is (true, var exception2))
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
         _block = getPartialBlock(state, nil).Maybe();
      }

      var commonBlock = _block | [];

      if (isError)
      {
         commonBlock.Insert(new AssignToNewField(false, "message", Expression.FromSymbol(new StringSymbol("")), false, false));
         commonBlock.Insert(new AssignToNewField(false, "callStack", Expression.FromSymbol(new StringSymbol("")), false, false));
      }

      var enumCreator = new TypeCreator(className, [.. typeMembers], commonBlock, isError);
      var _result = enumCreator.Create();
      if (_result)
      {
         state.AddStatement(enumCreator);
      }

      return _result;
   }
}